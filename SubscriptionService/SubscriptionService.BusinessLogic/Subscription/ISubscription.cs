using Shared.EventStore;
using System;
using System.Threading.Tasks;

namespace SubscriptionService.BusinessLogic.Subscription
{
    public interface ISubscription
    {
        /// <summary>
        /// Gets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        Guid SubscriptionId { get; }

        /// <summary>
        /// Gets the name of the stream.
        /// </summary>
        /// <value>
        /// The name of the stream.
        /// </value>
        String StreamName { get; }

        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        String GroupName { get; }

        /// <summary>
        /// Gets the start position.
        /// </summary>
        /// <value>
        /// The start position.
        /// </value>
        Int32? StartPosition { get; }

        /// <summary>
        /// Starts the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="streamName">Name of the stream.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="endPointId">The end point identifier.</param>
        /// <param name="startPosition">The start position.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="subscriptionType">Type of the subscription.</param>
        /// <returns></returns>
        Task StartSubscription(Guid subscriptionId, String streamName, String groupName, Int32? startPosition = null,Guid? endPointId = null, Int32 bufferSize = 10, SubscriptionType subscriptionType = SubscriptionType.Persistent);

        /// <summary>
        /// Stops the subscription.
        /// </summary>
        /// <returns></returns>
        Task StopSubscription();
    }
}
