using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.EventStore;
using Shared.General;
using SubscriptionService.BusinessLogic.Repository;
using SubscriptionService.BusinessLogic.Subscription;
using SubscriptionService.BusinessLogic.SubscriptionCache;
using SubscriptionService.DataTransferObjects;

namespace SubscriptionService.BusinessLogic
{
    using Microsoft.Extensions.Options;

    public class SubscriptionService : ISubscriptionService
    {
        #region Fields

        /// <summary>
        /// The subscription cache
        /// </summary>
        private readonly ISubscriptionCache<SubscriptionConfiguration> SubscriptionCache;

        /// <summary>
        /// The subscription resolver
        /// </summary>
        private readonly Func<ISubscription> SubscriptionResolver;

        /// <summary>
        /// The repository resolver
        /// </summary>
        private readonly Func<INewConfigurationRepository> RepositoryResolver;

        private readonly IOptions<ServiceSettings> ServiceSettings;

        /// <summary>
        /// The subscription list
        /// </summary>
        private readonly List<ISubscription> SubscriptionList = new List<ISubscription>();

        /// <summary>
        /// The timer
        /// </summary>
        private Timer Timer;

        #endregion

        #region Constructors        

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionService" /> class.
        /// </summary>
        /// <param name="subscriptionCache">The subscription cache.</param>
        /// <param name="subscriptionResolver">The subscription resolver.</param>
        /// <param name="repositoryResolver">The repository resolver.</param>
        public SubscriptionService(ISubscriptionCache<SubscriptionConfiguration> subscriptionCache, Func<ISubscription> subscriptionResolver,
            Func<INewConfigurationRepository> repositoryResolver, IOptions<ServiceSettings> serviceSettings)
        {
            this.SubscriptionCache = subscriptionCache;
            this.SubscriptionResolver = subscriptionResolver;
            this.RepositoryResolver = repositoryResolver;
            this.ServiceSettings = serviceSettings;
        }

        #endregion

        #region Public Methods

        #region public void StartService()        
        /// <summary>
        /// Starts the service.
        /// </summary>
        public void StartService()
        {
            Logger.LogInformation("Starting Service");

            // Setup the configuration cache
            this.SubscriptionCache.InitialiseCache();

            // Wire up the Added/Updated/Removed events on the cache
            this.SubscriptionCache.ItemAdded += this.SubscriptionCache_ItemAdded;
            this.SubscriptionCache.ItemRemoved += this.SubscriptionCache_ItemRemoved;
            this.SubscriptionCache.ItemUpdated += this.SubscriptionCache_ItemUpdated;

            // Monitor for catch up subscriptions
            this.StartMonitoringForSubscriptionCatchUp();
        }
        #endregion

        #region public void StopService()        
        /// <summary>
        /// Stops the service.
        /// </summary>
        public void StopService()
        {
            Logger.LogInformation("Stopping Service");

            // Detach up the Added/Updated/Removed events on the cache
            this.SubscriptionCache.ItemAdded -= this.SubscriptionCache_ItemAdded;
            this.SubscriptionCache.ItemRemoved -= this.SubscriptionCache_ItemRemoved;
            this.SubscriptionCache.ItemUpdated -= this.SubscriptionCache_ItemUpdated;

            // Stop all the subscriptions
            this.SubscriptionList.ForEach(x => x.StopSubscription());
        }
        #endregion

        #endregion

        #region Private Methods

        #region private async void SubscriptionCache_ItemUpdated(Object sender, SubscriptionGroup e)        
        /// <summary>
        /// Subscriptions the cache item updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private async void SubscriptionCache_ItemUpdated(Object sender, SubscriptionConfiguration e)
        {
            await ProcessCacheEvent(CacheEventType.Updated, e);
        }
        #endregion

        #region private async void SubscriptionCache_ItemRemoved(Object sender, SubscriptionGroup e)        
        /// <summary>
        /// Subscriptions the cache item removed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private async void SubscriptionCache_ItemRemoved(Object sender, SubscriptionConfiguration e)
        {
            await ProcessCacheEvent(CacheEventType.Removed, e);
        }
        #endregion

        #region private async void SubscriptionCache_ItemAdded(Object sender, SubscriptionGroup e)        
        /// <summary>
        /// Subscriptions the cache item added.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private async void SubscriptionCache_ItemAdded(Object sender, SubscriptionConfiguration e)
        {
            await ProcessCacheEvent(CacheEventType.Added, e);
        }
        #endregion

        #region private async Task ProcessCacheEvent(CacheEventType cacheEventType, SubscriptionGroup subscriptionGroup)        
        /// <summary>
        /// Processes the cache event.
        /// </summary>
        /// <param name="cacheEventType">Type of the cache event.</param>
        /// <param name="subscriptionConfiguration">The subscription group.</param>
        /// <returns></returns>
        private async Task ProcessCacheEvent(CacheEventType cacheEventType, SubscriptionConfiguration subscriptionConfiguration)
        {
            if (cacheEventType == CacheEventType.Added)
            {
                // Create a new subscription
                ISubscription subscription = this.SubscriptionResolver();

                // Start this subscription
                await subscription.StartSubscription(subscriptionConfiguration.SubscriptionId,
                    subscriptionConfiguration.StreamName,
                    subscriptionConfiguration.GroupName);

                // Add this to the cached subscription list
                this.SubscriptionList.Add(subscription);
            }
            else if (cacheEventType == CacheEventType.Removed)
            {
                // Find the item that has been removed based on the event argument
                ISubscription subscription = this.SubscriptionList.Single(x => x.SubscriptionId == subscriptionConfiguration.SubscriptionId);

                // Stop the subscription                
                await subscription.StopSubscription();

                // Now remove the item
                this.SubscriptionList.Remove(subscription);
            }
            else if (cacheEventType == CacheEventType.Updated)
            {
                // Find the item that has been updated based on the event argument
                ISubscription subscription = this.SubscriptionList.SingleOrDefault(x => x.SubscriptionId == subscriptionConfiguration.SubscriptionId);

                if (subscription != null)
                {
                    // Stop the current subscription
                    await subscription.StopSubscription();

                    // Re-Create the subscription
                    // Start this subscription 
                    await subscription.StartSubscription(subscriptionConfiguration.SubscriptionId,
                        subscriptionConfiguration.StreamName,
                        subscriptionConfiguration.GroupName,
                        subscriptionConfiguration.StreamPositionToRestartFrom);

                    // Update the repository position
                    INewConfigurationRepository configRepository = this.RepositoryResolver();
                    await configRepository.ResetSubscriptionStreamPosition(subscriptionConfiguration.SubscriptionId,
                        CancellationToken.None);
                }
            }
        }
        #endregion

        private void StartMonitoringForSubscriptionCatchUp()
        {
            TimerCallback timerCallback = state => this.CheckForCatchUpSubscriptions();
            this.Timer = new Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Checks for catch up subscriptions.
        /// </summary>
        /// <returns></returns>
        private async Task CheckForCatchUpSubscriptions()
        {
            try
            {
                Logger.LogInformation("Checking for CatchUp subscriptions");

                // Get a handle to the repository
                INewConfigurationRepository configRepository = this.RepositoryResolver();

                // Get the event store server id from the config
                Guid eventStoreServerId = this.ServiceSettings.Value.EventStoreServerId;

                // Get the subscription 
                CatchupSubscriptionConfiguration catchUpSubscriptionGroup = await configRepository.GetNextCatchupSubscriptionConfiguration(eventStoreServerId, CancellationToken.None);

                if (catchUpSubscriptionGroup != null)
                {
                    Logger.LogInformation($"About connect to stream for catch up {catchUpSubscriptionGroup.StreamName}");
                    Logger.LogInformation($"CatchUp retrieved with Id {catchUpSubscriptionGroup.SubscriptionId}");

                    // Create a new subscription
                    ISubscription subscription = this.SubscriptionResolver();

                    // Start this subscription
                    await subscription.StartSubscription(catchUpSubscriptionGroup.SubscriptionId,
                        catchUpSubscriptionGroup.StreamName, catchUpSubscriptionGroup.Name, catchUpSubscriptionGroup.Position, 
                        catchUpSubscriptionGroup.EndPointUri, subscriptionType: SubscriptionType.CatchUp);
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        #endregion
    }
}