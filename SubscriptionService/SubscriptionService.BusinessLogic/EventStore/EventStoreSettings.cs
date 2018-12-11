using System;

namespace SubscriptionService.BusinessLogic.EventStore
{
    public class EventStoreSettings
    {
        // This class is a short term measure as the ES connections will in time be configured in the Config Database, 
        // but for a first release of the new SS this will suffice        

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public String ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the connection.
        /// </summary>
        /// <value>
        /// The name of the connection.
        /// </value>
        public String ConnectionName { get; set; }

        /// <summary>
        /// Gets or sets the HTTP port.
        /// </summary>
        /// <value>
        /// The HTTP port.
        /// </value>
        public Int32 HttpPort { get; set; }
    }
}
