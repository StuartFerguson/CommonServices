using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.EventStore;
using SubscriptionService.DataTransferObjects;
using SubscriptionServiceDTO = SubscriptionService.DataTransferObjects.SubscriptionService;

namespace SubscriptionService.BusinessLogic.Repository
{
    public interface IConfigurationRepository
    {
        /// <summary>
        /// Gets the subscriptions.
        /// </summary>
        /// <param name="subscriptionServiceId">The subscription service identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<SubscriptionGroup>> GetSubscriptions(Guid subscriptionServiceId, CancellationToken cancellationToken);

        /// <summary>
        /// Resets the subscription stream position.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task ResetSubscriptionStreamPosition(Guid subscriptionGroupId, CancellationToken cancellationToken);

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

        /// <summary>
        /// Creates the subscription stream.
        /// </summary>
        /// <param name="subscriptionStreamId">The subscription stream identifier.</param>
        /// <param name="streamName">Name of the stream.</param>
        /// <param name="subscriptionType">Type of the subscription.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<Guid> CreateSubscriptionStream(Guid subscriptionStreamId, String streamName, SubscriptionType subscriptionType, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the subscription streams.
        /// </summary>
        /// <param name="subscriptionType">Type of the subscription.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<SubscriptionStream>> GetSubscriptionStreams(SubscriptionType subscriptionType, CancellationToken cancellationToken);

        /// <summary>
        /// Creates the end point.
        /// </summary>
        /// <param name="endpointName">Name of the endpoint.</param>
        /// <param name="url">The URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<Guid> CreateEndPoint(String endpointName, String url, CancellationToken cancellationToken);

        /// <summary>
        /// Creates the subscription group.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="subscriptionStreamId">The subscription stream identifier.</param>
        /// <param name="endpointId">The endpoint identifier.</param>
        /// <param name="subscriptionGroupName">Name of the subscription group.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<Guid> CreateSubscriptionGroup(Guid subscriptionGroupId, Guid subscriptionStreamId, Guid endpointId,String subscriptionGroupName, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the subscription group.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<SubscriptionGroup> GetSubscriptionGroup(Guid subscriptionGroupId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the subscription groups.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<SubscriptionGroup>> GetSubscriptionGroups(CancellationToken cancellationToken);

        /// <summary>
        /// Removes the subscription group.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task RemoveSubscriptionGroup(Guid subscriptionGroupId, CancellationToken cancellationToken);

        /// <summary>
        /// Creates the subscription service.
        /// </summary>
        /// <param name="subscriptionServiceId">The subscription service identifier.</param>
        /// <param name="description">The description.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<Guid> CreateSubscriptionService(Guid subscriptionServiceId, String description, CancellationToken cancellationToken);

        /// <summary>
        /// Adds the subscription group to subscription service.
        /// </summary>
        /// <param name="subscriptionServiceId">The subscription service identifier.</param>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<Guid> AddSubscriptionGroupToSubscriptionService(Guid subscriptionServiceId, Guid subscriptionGroupId, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the subscription group from subscription service.
        /// </summary>
        /// <param name="subscriptionServiceId">The subscription service identifier.</param>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task RemoveSubscriptionGroupFromSubscriptionService(Guid subscriptionServiceId, Guid subscriptionGroupId,CancellationToken cancellationToken);

        /// <summary>
        /// Gets the subscription services.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<SubscriptionServiceDTO>> GetSubscriptionServices(CancellationToken cancellationToken);

        /// <summary>
        /// Gets the subscription service.
        /// </summary>
        /// <param name="subscriptionServiceId">The subscription service identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<SubscriptionServiceDTO> GetSubscriptionService(Guid subscriptionServiceId, CancellationToken cancellationToken);
    }
}
