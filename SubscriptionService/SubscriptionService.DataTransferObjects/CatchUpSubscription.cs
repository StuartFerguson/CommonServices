using System;

namespace SubscriptionService.DataTransferObjects
{
    public class CatchUpSubscription
    {
        /// <summary>
        /// Gets or sets the catch up subscription identifier.
        /// </summary>
        /// <value>
        /// The catch up subscription identifier.
        /// </value>
        public Guid CatchUpSubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the name of the stream.
        /// </summary>
        /// <value>
        /// The name of the stream.
        /// </value>
        public String StreamName { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Int32 Position { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the endpoint identifier.
        /// </summary>
        /// <value>
        /// The endpoint identifier.
        /// </value>
        public Guid EndpointId { get; set; }
    }
}