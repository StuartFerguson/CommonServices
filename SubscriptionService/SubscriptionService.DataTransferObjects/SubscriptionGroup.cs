using System;

namespace SubscriptionService.DataTransferObjects
{
    public class SubscriptionGroup
    {
        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        public String GroupName { get; set; }

        /// <summary>
        /// Stream this Group belongs to.
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
        /// Gets or sets the subscription group identifier.
        /// </summary>
        /// <value>
        /// The subscription group identifier.
        /// </value>
        public Guid SubscriptionGroupId { get; set; }

        /// <summary>
        /// Gets or sets the size of the buffer.
        /// </summary>
        /// <value>
        /// The size of the buffer.
        /// </value>
        public Int32? BufferSize { get; set; }
    }

    //public class CreateSubscriptionStreamRequest
    //{
    //    public String StreamName { get; set; }
    //    public SubscriptionType SubscriptionType { get; set; }
    //}

    //public enum SubscriptionType
    //{
    //    Persistent = 0,
    //    CatchUp
    //}
}
