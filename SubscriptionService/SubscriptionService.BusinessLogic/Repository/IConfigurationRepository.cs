using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SubscriptionService.DataTransferObjects;

namespace SubscriptionService.BusinessLogic.Repository
{
    public interface IConfigurationRepository
    {
        /// <summary>
        /// Gets the subscriptions.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<SubscriptionGroup>> GetSubscriptions(CancellationToken cancellationToken);

        /// <summary>
        /// Resets the subscription stream position.
        /// </summary>
        /// <param name="persistentSubscriptionId">The persistent subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task ResetSubscriptionStreamPosition(Guid persistentSubscriptionId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the end point for subscription group.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<EndPoint> GetEndPointForSubscriptionGroup(Guid subscriptionGroupId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the end point for catch up subscription.
        /// </summary>
        /// <param name="catchupSubscriptionId">The catchup subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<EndPoint> GetEndPointForCatchUpSubscription(Guid catchupSubscriptionId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the next catch up subscription.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<CatchUpSubscription> GetNextCatchUpSubscription(CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the catch up subscription.
        /// </summary>
        /// <param name="catchUpSubscriptionId">The catch up subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task DeleteCatchUpSubscription(Guid catchUpSubscriptionId, CancellationToken cancellationToken);
    }
}
