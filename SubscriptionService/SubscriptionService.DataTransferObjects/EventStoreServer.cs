using System;
using System.Collections.Generic;
using System.Text;

namespace SubscriptionService.DataTransferObjects
{
    public class EventStoreServer
    {
        /// <summary>
        /// Gets or sets the event store server identifier.
        /// </summary>
        /// <value>
        /// The event store server identifier.
        /// </value>
        public Guid EventStoreServerId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public String ConnectionString { get; set; }
    }
}
