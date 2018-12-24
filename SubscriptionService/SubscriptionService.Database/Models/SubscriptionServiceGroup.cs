using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubscriptionService.Database.Models
{
    public class SubscriptionServiceGroup
    {
        /// <summary>
        /// Gets or sets the subscription service group identifier.
        /// </summary>
        /// <value>
        /// The subscription service group identifier.
        /// </value>
        [Key]
        public Guid SubscriptionServiceGroupId { get; set; }

        /// <summary>
        /// Gets or sets the subscription group.
        /// </summary>
        /// <value>
        /// The subscription group.
        /// </value>
        public virtual SubscriptionGroup SubscriptionGroup { get; set; }

        /// <summary>
        /// Gets or sets the subscription group identifier.
        /// </summary>
        /// <value>
        /// The subscription group identifier.
        /// </value>
        [ForeignKey("SubscriptionGroup")]
        public Guid SubscriptionGroupId { get; set; }

        /// <summary>
        /// Gets or sets the subscription service.
        /// </summary>
        /// <value>
        /// The subscription service.
        /// </value>
        public virtual SubscriptionService SubscriptionService { get; set; }

        /// <summary>
        /// Gets or sets the subscription service identifier.
        /// </summary>
        /// <value>
        /// The subscription service identifier.
        /// </value>
        [ForeignKey("SubscriptionService")]
        public Guid SubscriptionServiceId { get; set; }
    }
}