using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubscriptionService.Database.Models
{
    public class CatchUpSubscription
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
        /// Gets or sets the create date time.
        /// </summary>
        /// <value>
        /// The create date time.
        /// </value>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public virtual EndPoint EndPoint { get; set; }

        /// <summary>
        /// Gets or sets the end point identifier.
        /// </summary>
        /// <value>
        /// The end point identifier.
        /// </value>
        [ForeignKey("EndPoint")]
        public Guid EndPointId { get; set; }
    }
}