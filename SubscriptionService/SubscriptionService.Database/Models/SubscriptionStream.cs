using System;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Database.Models
{
    public class SubscriptionStream
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the stream.
        /// </summary>
        /// <value>
        /// The name of the stream.
        /// </value>
        public String StreamName { get; set; }

        /// <summary>
        /// Gets or sets the type of the subscription.
        /// </summary>
        /// <value>
        /// The type of the subscription.
        /// </value>
        public Int32 SubscriptionType { get; set; }
    }
}
