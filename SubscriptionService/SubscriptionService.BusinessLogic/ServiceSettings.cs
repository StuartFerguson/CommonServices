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
        /// Gets or sets the subscription service identifier.
        /// </summary>
        /// <value>
        /// The subscription service identifier.
        /// </value>
        public Guid SubscriptionServiceId { get; set; }
    }
}
