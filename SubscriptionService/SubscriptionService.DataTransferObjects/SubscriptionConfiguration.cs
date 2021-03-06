﻿namespace SubscriptionService.DataTransferObjects
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the end point URI.
        /// </summary>
        /// <value>
        /// The end point URI.
        /// </value>
        public String EndPointUri { get; set; }

        /// <summary>
        /// Gets or sets the event store server identifier.
        /// </summary>
        /// <value>
        /// The event store server identifier.
        /// </value>
        public Guid EventStoreServerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        public String GroupName { get; set; }

        /// <summary>
        /// Gets or sets the name of the stream.
        /// </summary>
        /// <value>
        /// The name of the stream.
        /// </value>
        public String StreamName { get; set; }

        /// <summary>
        /// Gets or sets the stream position.
        /// </summary>
        /// <value>
        /// The stream position.
        /// </value>
        public Int32? StreamPositionToRestartFrom { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        public Guid SubscriptionId { get; set; }

        #endregion
    }
}