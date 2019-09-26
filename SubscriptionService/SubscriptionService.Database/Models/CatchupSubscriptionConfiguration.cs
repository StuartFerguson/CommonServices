namespace SubscriptionService.Database.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CatchupSubscriptionConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the create date time.
        /// </summary>
        /// <value>
        /// The create date time.
        /// </value>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// Gets or sets the end point URI.
        /// </summary>
        /// <value>
        /// The end point URI.
        /// </value>
        public String EndPointUri { get; set; }

        /// <summary>
        /// Gets or sets the event store server.
        /// </summary>
        /// <value>
        /// The event store server.
        /// </value>
        [ForeignKey(nameof(CatchupSubscriptionConfiguration.EventStoreServerId))]
        public EventStoreServer EventStoreServer { get; set; }

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
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Int32 Position { get; set; }

        /// <summary>
        /// Gets or sets the name of the stream.
        /// </summary>
        /// <value>
        /// The name of the stream.
        /// </value>
        public String StreamName { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        [Key]
        public Guid SubscriptionId { get; set; }

        #endregion
    }
}