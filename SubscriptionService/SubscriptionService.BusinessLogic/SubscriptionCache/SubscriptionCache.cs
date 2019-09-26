using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shared.General;
using SubscriptionService.BusinessLogic.Repository;
using SubscriptionService.DataTransferObjects;

namespace SubscriptionService.BusinessLogic.SubscriptionCache
{
    public class SubscriptionCache : ISubscriptionCache<SubscriptionConfiguration>
    {
        #region Fields

        /// <summary>
        /// The configuration repository
        /// </summary>
        private readonly INewConfigurationRepository ConfigurationRepository;

        /// <summary>
        /// The service settings
        /// </summary>
        private readonly IOptions<ServiceSettings> ServiceSettings;

        /// <summary>
        /// Gets or sets the timer.
        /// </summary>
        /// <value>
        /// The timer.
        /// </value>
        private Timer Timer;

        /// <summary>
        /// The subscription groups
        /// </summary>
        private Dictionary<Guid, SubscriptionConfiguration> SubscriptionConfigurations;

        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionCache" /> class.
        /// </summary>
        /// <param name="configurationRepository">The configuration repository.</param>
        /// <param name="serviceSettings">The service settings.</param>
        public SubscriptionCache(INewConfigurationRepository configurationRepository, IOptions<ServiceSettings> serviceSettings)
        {
            this.ConfigurationRepository = configurationRepository;
            this.ServiceSettings = serviceSettings;
            this.SubscriptionConfigurations = new Dictionary<Guid, SubscriptionConfiguration>();
        }
        #endregion

        #region Events

        /// <summary>
        /// Occurs when [item added].
        /// </summary>
        public event EventHandler<SubscriptionConfiguration> ItemAdded;

        /// <summary>
        /// Occurs when [item removed].
        /// </summary>
        public event EventHandler<SubscriptionConfiguration> ItemRemoved;

        /// <summary>
        /// Occurs when [item updated].
        /// </summary>
        public event EventHandler<SubscriptionConfiguration> ItemUpdated;

        #endregion

        #region Public Methods        

        #region public void InitialiseCache()
        /// <summary>
        /// Initialises the cache.
        /// </summary>
        public void InitialiseCache()
        {
            // Setup the callback for the timer
            TimerCallback callback = async callbackState =>
            {
                // handle subscription config
                await this.ProcessSubscriptions();

                Logger.LogInformation("Cache Initialised");
            };

            // Setup the timer
            this.Timer = new Timer(callback, null, TimeSpan.Zero, TimeSpan.FromSeconds(this.ServiceSettings.Value.CacheTimeout));
        }
        #endregion

        #endregion

        #region Private Methods

        #region private async Task ProcessSubscriptions()        
        /// <summary>
        /// Processes the subscriptions.
        /// </summary>
        /// <returns></returns>
        private async Task ProcessSubscriptions()
        {
            // Get the event store server id from the config
            Guid eventStoreServerId = this.ServiceSettings.Value.EventStoreServerId;

            // Check if we have any groups currently
            if (!this.SubscriptionConfigurations.Any())
            {
                Logger.LogInformation("First Initialisation of Cache");

                // Currently no groups loaded into memory

                // Read the groups from configuration
                List<SubscriptionConfiguration> subscriptionConfigurations = await this.ConfigurationRepository.GetSubscriptionConfigurations(eventStoreServerId, CancellationToken.None);
                //subscriptionGroups = subscriptionGroups.Take(1).ToList();
                foreach (SubscriptionConfiguration subscriptionConfiguration in subscriptionConfigurations)
                {
                    // Add to the list of groups
                    this.SubscriptionConfigurations.Add(subscriptionConfiguration.SubscriptionId, subscriptionConfiguration);

                    // Raise the added event 
                    if (this.ItemAdded != null)
                    {
                        Logger.LogInformation($"About to add subscription group {subscriptionConfiguration.GroupName}");

                        this.ItemAdded(this, subscriptionConfiguration);

                        Logger.LogInformation($"Subscription group {subscriptionConfiguration.GroupName} added");
                    }
                    else
                    {
                        Logger.LogWarning("Item Added Event Handler is null");
                    }
                }
            }
            else
            {
                // Read the groups from configuration to determine any new/updates/deletes
                List<SubscriptionConfiguration> subscriptionConfigurations = await this.ConfigurationRepository.GetSubscriptionConfigurations(eventStoreServerId, CancellationToken.None);

                List<SubscriptionConfiguration> cachedConfiguration = this.SubscriptionConfigurations.Select(x => x.Value).ToList();

                // Get the new config items
                IEnumerable<SubscriptionConfiguration> newConfigEntries = subscriptionConfigurations.Except(cachedConfiguration, new GenericCompare<SubscriptionConfiguration>(x => x.SubscriptionId));

                foreach (SubscriptionConfiguration item in newConfigEntries)
                {
                    if (this.ItemAdded != null)
                    {
                        // Add to the list of groups
                        this.SubscriptionConfigurations.Add(item.SubscriptionId, item);

                        Logger.LogInformation($"About to add subscription group {item.GroupName}");

                        this.ItemAdded(this, item);

                        Logger.LogInformation($"Subscription group {item.GroupName} added");
                    }
                    else
                    {
                        Logger.LogWarning("Item Added Event Handler is null");
                    }
                }

                // Handle the deleted items
                IEnumerable<SubscriptionConfiguration> deletedConfig = cachedConfiguration.Except(subscriptionConfigurations, new GenericCompare<SubscriptionConfiguration>(x => x.SubscriptionId));

                foreach (SubscriptionConfiguration item in deletedConfig)
                {
                    if (this.ItemRemoved != null)
                    {
                        Logger.LogInformation($"About to remove subscription group {item.GroupName}");

                        this.SubscriptionConfigurations.Remove(item.SubscriptionId);

                        this.ItemRemoved(this, item);

                        Logger.LogInformation($"Subscription group {item.GroupName} removed");
                    }
                    else
                    {
                        Logger.LogWarning("Item Removed Event Handler is null");
                    }
                }

                // Get the configs that match
                IEnumerable<SubscriptionConfiguration> matches = subscriptionConfigurations.Intersect(cachedConfiguration, new GenericCompare<SubscriptionConfiguration>(x => x.SubscriptionId));

                foreach (SubscriptionConfiguration item in matches)
                {
                    // If position is set to -1 we require to reset the subscription
                    if (item.StreamPositionToRestartFrom.HasValue)
                    {
                        if (this.ItemUpdated != null)
                        {
                            Logger.LogInformation($"About to update subscription group {item.GroupName}");

                            this.ItemUpdated(this, item);

                            Logger.LogInformation($"Subscription group {item.GroupName} update");
                        }
                        else
                        {
                            Logger.LogWarning("Item Updated Event Handler is null");
                        }
                    }
                }
            }
        }
        #endregion

        #endregion

    }
}