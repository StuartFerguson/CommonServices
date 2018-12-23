using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SubscriptionService.Database.Models
{
    public class SubscriptionService
    {
        /// <summary>
        /// Gets or sets the subscription service identifier.
        /// </summary>
        /// <value>
        /// The subscription service identifier.
        /// </value>
        [Key]
        public Guid SubscriptionServiceId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public String Description { get; set; }
    }
}
