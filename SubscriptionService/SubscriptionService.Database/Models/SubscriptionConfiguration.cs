using System;
using System.Collections.Generic;
using System.Text;

namespace SubscriptionService.Database.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;


    public class SubscriptionConfiguration
    {
        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        [Key]
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the event store server identifier.
        /// </summary>
        /// <value>
        /// The event store server identifier.
        /// </value>
        public Guid EventStoreServerId { get; set; }

        /// <summary>
        /// Gets or sets the event store server.
        /// </summary>
        /// <value>
        /// The event store server.
        /// </value>
        [ForeignKey(nameof(EventStoreServerId))]
        public EventStoreServer EventStoreServer { get; set; }

        /// <summary>
        /// Gets or sets the name of the stream.
        /// </summary>
        /// <value>
        /// The name of the stream.
        /// </value>
        public String StreamName { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        public String GroupName { get; set; }

        /// <summary>
        /// Gets or sets the end point URI.
        /// </summary>
        /// <value>
        /// The end point URI.
        /// </value>
        public String EndPointUri { get; set; }

        /// <summary>
        /// Gets or sets the stream position.
        /// </summary>
        /// <value>
        /// The stream position.
        /// </value>
        public Int32? StreamPosition { get; set; }
    }
}
