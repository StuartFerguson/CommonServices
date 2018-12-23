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
    public class SubscriptionCache : ISubscriptionCache<SubscriptionGroup>
    {
        #region Fields

        /// <summary>
        /// The configuration repository
        /// </summary>
        private readonly IConfigurationRepository ConfigurationRepository;

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
        private Dictionary<Guid, SubscriptionGroup> SubscriptionGroups;

        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionCache" /> class.
        /// </summary>
        /// <param name="configurationRepository">The configuration repository.</param>
        /// <param name="serviceSettings">The service settings.</param>
        public SubscriptionCache(IConfigurationRepository configurationRepository, IOptions<ServiceSettings> serviceSettings)
        {
            this.ConfigurationRepository = configurationRepository;
            this.ServiceSettings = serviceSettings;
            this.SubscriptionGroups = new Dictionary<Guid, SubscriptionGroup>();
        }
        #endregion

        #region Events

        /// <summary>
        /// Occurs when [item added].
        /// </summary>
        public event EventHandler<SubscriptionGroup> ItemAdded;

        /// <summary>
        /// Occurs when [item removed].
        /// </summary>
        public event EventHandler<SubscriptionGroup> ItemRemoved;

        /// <summary>
        /// Occurs when [item updated].
        /// </summary>
        public event EventHandler<SubscriptionGroup> ItemUpdated;

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
            // Get the subscription service id from the config
            var subscriptionServiceId = this.ServiceSettings.Value.SubscriptionServiceId;

            // Check if we have any groups currently
            if (!this.SubscriptionGroups.Any())
            {
                Logger.LogInformation("First Initialisation of Cache");

                // Currently no groups loaded into memory

                // Read the groups from configuration
                var subscriptionGroups = await this.ConfigurationRepository.GetSubscriptions(subscriptionServiceId, CancellationToken.None);
                //subscriptionGroups = subscriptionGroups.Take(1).ToList();
                foreach (var subscriptionGroup in subscriptionGroups)
                {
                    // Add to the list of groups
                    this.SubscriptionGroups.Add(subscriptionGroup.SubscriptionGroupId, subscriptionGroup);

                    // Raise the added event 
                    if (this.ItemAdded != null)
                    {
                        Logger.LogInformation($"About to add subscription group {subscriptionGroup.GroupName}");

                        this.ItemAdded(this, subscriptionGroup);

                        Logger.LogInformation($"Subscription group {subscriptionGroup.GroupName} added");
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
                var subscriptionGroups = await this.ConfigurationRepository.GetSubscriptions(subscriptionServiceId, CancellationToken.None);

                var cachedConfiguration = this.SubscriptionGroups.Select(x => x.Value).ToList();

                // Get the new config items
                var newConfigEntries = subscriptionGroups.Except(cachedConfiguration, new GenericCompare<SubscriptionGroup>(x => x.SubscriptionGroupId));

                foreach (var item in newConfigEntries)
                {
                    if (this.ItemAdded != null)
                    {
                        // Add to the list of groups
                        this.SubscriptionGroups.Add(item.SubscriptionGroupId, item);

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
                var deletedConfig = cachedConfiguration.Except(subscriptionGroups, new GenericCompare<SubscriptionGroup>(x => x.SubscriptionGroupId));

                foreach (var item in deletedConfig)
                {
                    if (this.ItemRemoved != null)
                    {
                        Logger.LogInformation($"About to remove subscription group {item.GroupName}");

                        this.SubscriptionGroups.Remove(item.SubscriptionGroupId);

                        this.ItemRemoved(this, item);

                        Logger.LogInformation($"Subscription group {item.GroupName} removed");
                    }
                    else
                    {
                        Logger.LogWarning("Item Removed Event Handler is null");
                    }
                }

                // Get the configs that match
                var matches = subscriptionGroups.Intersect(cachedConfiguration, new GenericCompare<SubscriptionGroup>(x => x.SubscriptionGroupId));

                foreach (var item in matches)
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