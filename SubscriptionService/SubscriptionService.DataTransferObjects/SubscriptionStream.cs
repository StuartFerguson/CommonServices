using System;

namespace SubscriptionService.DataTransferObjects
{
    public class SubscriptionStream
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
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

    public class SubscriptionService
    {
        /// <summary>
        /// Gets or sets the subscription service identifier.
        /// </summary>
        /// <value>
        /// The subscription service identifier.
        /// </value>
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