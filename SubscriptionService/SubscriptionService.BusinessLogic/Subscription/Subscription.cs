namespace SubscriptionService.BusinessLogic.Subscription
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTransferObjects;
    using EventStore;
    using global::EventStore.ClientAPI;
    using Repository;
    using Shared.EventStore;
    using Shared.Exceptions;
    using Shared.General;

    public class Subscription : ISubscription
    {
        #region Fields

        /// <summary>
        /// The configuration repository resolver
        /// </summary>
        private readonly Func<INewConfigurationRepository> ConfigurationRepositoryResolver;

        /// <summary>
        /// The event store manager
        /// </summary>
        private readonly IEventStoreManager EventStoreManager;

        /// <summary>
        /// The HTTP client
        /// </summary>
        private readonly HttpClient HttpClient;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription" /> class.
        /// </summary>
        /// <param name="eventStoreManager">The event store manager.</param>
        /// <param name="configurationRepositoryResolver">The configuration repository resolver.</param>
        public Subscription(IEventStoreManager eventStoreManager,
                            Func<INewConfigurationRepository> configurationRepositoryResolver)
        {
            this.EventStoreManager = eventStoreManager;
            this.ConfigurationRepositoryResolver = configurationRepositoryResolver;

            this.HttpClient = new HttpClient();
            this.HttpClient.DefaultRequestHeaders.Accept.Clear();
            this.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion

        #region Properties

        public String EndpointUri { get; }

        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        public String GroupName { get; private set; }

        /// <summary>
        /// Gets the start position.
        /// </summary>
        /// <value>
        /// The start position.
        /// </value>
        public Int32? StartPosition { get; private set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public SubscriptionStatus Status { get; private set; }

        /// <summary>
        /// Gets the name of the stream.
        /// </summary>
        /// <value>
        /// The name of the stream.
        /// </value>
        public String StreamName { get; private set; }

        /// <summary>
        /// Gets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        public Guid SubscriptionId { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public SubscriptionType Type { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="streamName">Name of the stream.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="startPosition">The start position.</param>
        /// <param name="endPointUri"></param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="subscriptionType">Type of the subscription.</param>
        public async Task StartSubscription(Guid subscriptionId,
                                            String streamName,
                                            String groupName,
                                            Int32? startPosition = null,
                                            String endPointUri = null,
                                            Int32 bufferSize = 10,
                                            SubscriptionType subscriptionType = SubscriptionType.Persistent)
        {
            Guard.ThrowIfInvalidGuid(subscriptionId, typeof(ArgumentNullException), "Subscription Id must not be an empty GUID");
            Guard.ThrowIfNullOrEmpty(streamName, typeof(ArgumentNullException), "Stream Name must not be null or empty");
            Guard.ThrowIfNullOrEmpty(groupName, typeof(ArgumentNullException), "Group Name must not be null or empty");

            try
            {
                this.SubscriptionId = subscriptionId;
                this.StreamName = streamName;
                this.GroupName = groupName;
                this.StartPosition = startPosition;
                this.Status = SubscriptionStatus.Started;
                this.Type = subscriptionType;

                // Get the Event Store context
                IEventStoreContext context = await this.EventStoreManager.GetEventStoreContext(this.EventAppeared, this.SubscriptionDropped, this.LiveProcessStarted);

                if (this.Type == SubscriptionType.Persistent)
                {
                    // Connect to the subscription
                    await context.ConnectToSubscription(streamName, groupName, this.SubscriptionId, bufferSize);
                }
                else if (this.Type == SubscriptionType.CatchUp)
                {
                    // Handle the fact that SubscribeToStreamFrom treats the lastCheckpoint parameter as the Start + 1
                    Int32? position = null;
                    if (this.StartPosition > -1)
                    {
                        position = this.StartPosition;
                    }

                    await context.SubscribeToStreamFrom(this.SubscriptionId, this.StreamName, position, Guid.Empty);
                }

                Logger.LogInformation("Subscription Started");
            }
            catch(Exception ex)
            {
                Exception exception = new Exception($"Error starting Subscription for Stream Name {streamName} Group Name {groupName}", ex);
                Logger.LogError(exception);
                throw;
            }
        }

        /// <summary>
        /// Stops the subscription.
        /// </summary>
        /// <returns></returns>
        public async Task StopSubscription()
        {
            if (this.Type == SubscriptionType.Persistent)
            {
                // Get the Event Store context
                IEventStoreContext context = await this.EventStoreManager.GetEventStoreContext(this.EventAppeared, this.SubscriptionDropped, this.LiveProcessStarted);

                await context.DeletePersistentSubscription(this.StreamName, this.GroupName);
            }

            Logger.LogInformation("Subscription Stopped");
            this.Status = SubscriptionStatus.Stopped;
        }

        /// <summary>
        /// Events the appeared.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">No Endpoint Uri found for Subscription Id {subscription.SubscriptionGroupId}</exception>
        /// <exception cref="Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="TimeoutException">Error processing Event Id {subscription.EventId}. Attempted to Send to Uri: {endpoint}, request Timed out</exception>
        private async Task<Boolean> EventAppeared(SubscriptionDataTransferObject subscription)
        {
            Boolean result = false;
            String endpoint = null;
            try
            {
                // Lookup the endpoint that we need to post this event to
                // Get the config repository
                INewConfigurationRepository repository = this.ConfigurationRepositoryResolver();

                // Get the endpoint from the repository
                if (this.Type == SubscriptionType.Persistent)
                {
                    SubscriptionConfiguration configuration =
                        await repository.GetSubscriptionConfiguration(Guid.Parse(subscription.SubscriptionGroupId), CancellationToken.None);

                    endpoint = configuration.EndPointUri;
                }
                else if (this.Type == SubscriptionType.CatchUp)
                {
                    CatchupSubscriptionConfiguration configuration =
                        await repository.GetCatchupSubscriptionConfiguration(Guid.Parse(subscription.SubscriptionGroupId), CancellationToken.None);
                    endpoint = configuration.EndPointUri;
                }

                // Check we have found an endpoint
                if (string.IsNullOrEmpty(endpoint))
                {
                    throw new NotFoundException($"No Endpoint Uri found for Subscription Id {subscription.SubscriptionGroupId}");
                }

                // Send the request to the endpoint
                using(HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint))
                {
                    request.Content = new StringContent(subscription.SerialisedData, Encoding.UTF8, "application/json");

                    // Set the http timeout from the config value            
                    CancellationTokenSource cts = new CancellationTokenSource();
                    cts.CancelAfter(100000);

                    HttpResponseMessage response = await this.HttpClient.SendAsync(request, cts.Token);

                    if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.InternalServerError)
                    {
                        // Get the message contents
                        String responseBody = await response.Content.ReadAsStringAsync();

                        // Construct an error message
                        String errorMessage = $"Failed Posting to Subscription Group Endpoint [{response.StatusCode}]. Response [{responseBody}]";

                        throw new Exception(errorMessage);
                    }

                    if (!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        String errorMessage = "Failed Posting to Subscription Group Endpoint - Internal Server Error";

                        throw new InvalidOperationException(errorMessage);
                    }
                }

                result = true;
            }
            catch(TaskCanceledException tcex)
            {
                // This transaction has timed out
                throw new TimeoutException($"Error processing Event Id {subscription.EventId}. Attempted to Send to Uri: {endpoint}, request Timed out");
            }
            catch(NotFoundException nex)
            {
                Logger.LogError(nex);
            }
            catch(Exception ex)
            {
                String errorMessage = $"Error processing Event Id {subscription.EventId}. Attempted to Send to Uri: {endpoint}";
                Exception exception = new Exception(errorMessage, ex);
                Logger.LogError(exception);
            }

            return result;
        }

        /// <summary>
        /// Lives the process started.
        /// </summary>
        /// <param name="catchUpSubscriptionId">The catch up subscription identifier.</param>
        private async void LiveProcessStarted(Guid catchUpSubscriptionId)
        {
            Logger.LogInformation($"LiveProcessStarted for catchUpSubscriptionId {catchUpSubscriptionId}");

            // Get the config repository
            INewConfigurationRepository repository = this.ConfigurationRepositoryResolver();

            try
            {
                // Remove this catchup subscription
                await repository.DeleteCatchUpSubscription(catchUpSubscriptionId, CancellationToken.None);

                await this.StopSubscription();
            }
            catch(Exception e)
            {
                Exception ex = new Exception($"Failed to delete CatchUpSubscription {catchUpSubscriptionId}", e);
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// Subscriptions the dropped.
        /// </summary>
        /// <param name="streamName">Name of the stream.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="subscriptionType">Type of the subscription.</param>
        /// <param name="subscriptionDropReason">The subscription drop reason.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        private async void SubscriptionDropped(String streamName,
                                               String groupName,
                                               SubscriptionType subscriptionType,
                                               SubscriptionDropReason subscriptionDropReason,
                                               Exception exception,
                                               Guid subscriptionGroupId)
        {
            Exception ex = new Exception($"{subscriptionType} Subscription Dropped for Stream {streamName} Group {groupName}, reason {subscriptionDropReason}");
            Logger.LogError(ex);

            if (subscriptionType == SubscriptionType.Persistent)
            {
                Logger.LogInformation($"About to start subscription as subscription dropped Stream {streamName} Group {groupName}");
                await this.StartSubscription(subscriptionGroupId, streamName, groupName);
            }
        }

        #endregion
    }
}