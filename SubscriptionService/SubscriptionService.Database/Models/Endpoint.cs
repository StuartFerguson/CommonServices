using System;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Database.Models
{
    public class EndPoint
    {
        /// <summary>
        /// Gets or sets the end point identifier.
        /// </summary>
        /// <value>
        /// The end point identifier.
        /// </value>
        [Key]
        public Guid EndPointId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public String Url { get; set; }
    }
}

