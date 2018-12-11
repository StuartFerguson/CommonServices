using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubscriptionService.Database.Models
{
    public class SubscriptionGroup
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
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the subscription stream identifier.
        /// </summary>
        /// <value>
        /// The subscription stream identifier.
        /// </value>
        public Guid SubscriptionStreamId { get; set; }

        /// <summary>
        /// Gets or sets the subscription stream.
        /// </summary>
        /// <value>
        /// The subscription stream.
        /// </value>
        [ForeignKey("SubscriptionStreamId")]
        public virtual SubscriptionStream SubscriptionStream { get; set; }

        /// <summary>
        /// Gets or sets the stream position.
        /// </summary>
        /// <value>
        /// The stream position.
        /// </value>
        public Int32? StreamPosition { get; set; }

        /// <summary>
        /// Gets or sets the size of the buffer.
        /// </summary>
        /// <value>
        /// The size of the buffer.
        /// </value>
        public Int32? BufferSize { get; set; }

        /// <summary>
        /// Gets or sets the end point identifier.
        /// </summary>
        /// <value>
        /// The end point identifier.
        /// </value>
        public Guid EndPointId { get; set; }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>
        /// The end point.
        /// </value>
        [ForeignKey("EndPointId")]
        public virtual EndPoint EndPoint { get; set; }
    }
}
