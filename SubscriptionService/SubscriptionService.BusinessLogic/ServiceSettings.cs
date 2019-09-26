using System;

namespace SubscriptionService.BusinessLogic
{
    public class ServiceSettings
    {
        /// <summary>
        /// Gets or sets the cache timeout.
        /// </summary>
        /// <value>
        /// The cache timeout.
        /// </value>
        public Int32 CacheTimeout { get; set; }

        /// <summary>
        /// Gets or sets the event store server identifier.
        /// </summary>
        /// <value>
        /// The event store server identifier.
        /// </value>
        public Guid EventStoreServerId { get; set; }
    }
}
