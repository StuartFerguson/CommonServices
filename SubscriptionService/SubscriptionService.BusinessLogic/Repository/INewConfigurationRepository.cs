using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubscriptionService.BusinessLogic.Repository
{
    using System.Linq;
    using Database;
    using Database.Models;
    using Microsoft.EntityFrameworkCore;
    using Shared.Exceptions;
    using Shared.General;
    using CatchupSubscriptionConfiguration = global::SubscriptionService.DataTransferObjects.CatchupSubscriptionConfiguration;
    using EventStoreServer = global::SubscriptionService.DataTransferObjects.EventStoreServer;
    using SubscriptionConfiguration = global::SubscriptionService.DataTransferObjects.SubscriptionConfiguration;

    /// <summary>
    /// 
    /// </summary>
    public interface INewConfigurationRepository
    {
        /// <summary>
        /// Gets the subscription configurations.
        /// </summary>
        /// <param name="eventStoreServerId">The event store server identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<SubscriptionConfiguration>> GetSubscriptionConfigurations(Guid eventStoreServerId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the subscription configuration.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<SubscriptionConfiguration> GetSubscriptionConfiguration(Guid subscriptionId,
                                                                     CancellationToken cancellationToken);

        /// <summary>
        /// Gets the event store server.
        /// </summary>
        /// <param name="eventStoreServerId">The event store server identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<EventStoreServer> GetEventStoreServer(Guid eventStoreServerId,
                                                   CancellationToken cancellationToken);

        /// <summary>
        /// Resets the subscription stream position.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task ResetSubscriptionStreamPosition(Guid subscriptionId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the next catchup subscription configuration.
        /// </summary>
        /// <param name="eventStoreServerId">The event store server identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<CatchupSubscriptionConfiguration> GetNextCatchupSubscriptionConfiguration(Guid eventStoreServerId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the catchup subscription configuration.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<CatchupSubscriptionConfiguration> GetCatchupSubscriptionConfiguration(Guid subscriptionId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the catch up subscription.
        /// </summary>
        /// <param name="catchUpSubscriptionId">The catch up subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task DeleteCatchUpSubscription(Guid catchUpSubscriptionId, CancellationToken cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="INewConfigurationRepository" />
    public class NewConfigurationRepository : INewConfigurationRepository
    {
        /// <summary>
        /// The context resolver
        /// </summary>
        private readonly Func<SubscriptionServiceConfigurationContext> ContextResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewConfigurationRepository" /> class.
        /// </summary>
        /// <param name="contextResolver">The context resolver.</param>
        public NewConfigurationRepository(Func<SubscriptionServiceConfigurationContext> contextResolver)
        {
            this.ContextResolver = contextResolver;
        }

        /// <summary>
        /// Gets the subscription configurations.
        /// </summary>
        /// <param name="eventStoreServerId">The event store server identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<SubscriptionConfiguration>> GetSubscriptionConfigurations(Guid eventStoreServerId,
                                                        CancellationToken cancellationToken)
        {
            List<SubscriptionConfiguration> result = new List<SubscriptionConfiguration>();

            using (SubscriptionServiceConfigurationContext context = this.ContextResolver())
            {
                List<Database.Models.SubscriptionConfiguration> subscriptionConfigurations = await context.SubscriptionConfigurations.Where(s => s.EventStoreServerId == eventStoreServerId).ToListAsync(cancellationToken);

                foreach (Database.Models.SubscriptionConfiguration subscriptionConfiguration in subscriptionConfigurations)
                {
                    result.Add(new SubscriptionConfiguration
                               {
                                   EventStoreServerId = subscriptionConfiguration.EventStoreServerId,
                                   EndPointUri = subscriptionConfiguration.EndPointUri,
                                   GroupName = subscriptionConfiguration.GroupName,
                                   StreamName = subscriptionConfiguration.StreamName,
                                   StreamPositionToRestartFrom = subscriptionConfiguration.StreamPosition,
                                   SubscriptionId = subscriptionConfiguration.SubscriptionId
                               });
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the subscription configuration.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<SubscriptionConfiguration> GetSubscriptionConfiguration(Guid subscriptionId,
                                                                                         CancellationToken cancellationToken)
        {
            SubscriptionConfiguration result = new SubscriptionConfiguration();

            using(SubscriptionServiceConfigurationContext context = this.ContextResolver())
            {
                Database.Models.SubscriptionConfiguration subscriptionConfiguration =
                    await context.SubscriptionConfigurations.Where(s => s.SubscriptionId == subscriptionId).SingleOrDefaultAsync(cancellationToken);

                result = new SubscriptionConfiguration
                         {
                             EventStoreServerId = subscriptionConfiguration.EventStoreServerId,
                             EndPointUri = subscriptionConfiguration.EndPointUri,
                             GroupName = subscriptionConfiguration.GroupName,
                             StreamName = subscriptionConfiguration.StreamName,
                             StreamPositionToRestartFrom = subscriptionConfiguration.StreamPosition,
                             SubscriptionId = subscriptionConfiguration.SubscriptionId
                         };
            }

            return result;
        }

        /// <summary>
        /// Gets the event store server.
        /// </summary>
        /// <param name="eventStoreServerId">The event store server identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<EventStoreServer> GetEventStoreServer(Guid eventStoreServerId,
                                              CancellationToken cancellationToken)
        {
            EventStoreServer result = new EventStoreServer();

            using (SubscriptionServiceConfigurationContext context = this.ContextResolver())
            {
                Database.Models.EventStoreServer eventStoreServer = await context.EventStoreServers.Where(s => s.EventStoreServerId == eventStoreServerId).SingleOrDefaultAsync(cancellationToken);

                result.EventStoreServerId = eventStoreServer.EventStoreServerId;
                result.ConnectionString = eventStoreServer.ConnectionString;
                result.Name = eventStoreServer.Name;
            }

            return result;
        }

        /// <summary>
        /// Resets the subscription stream position.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="NotFoundException">Subscription Group with Id {subscriptionConfiguration} not found</exception>
        public async Task ResetSubscriptionStreamPosition(Guid subscriptionId,
                                                          CancellationToken cancellationToken)
        {
            Guard.ThrowIfInvalidGuid(subscriptionId, typeof(ArgumentNullException), "A subscription group Id must be provided");

            using (SubscriptionServiceConfigurationContext context = this.ContextResolver())
            {
                Database.Models.SubscriptionConfiguration subscriptionConfiguration = await context.SubscriptionConfigurations.Where(g => g.SubscriptionId== subscriptionId)
                                                     .SingleOrDefaultAsync(cancellationToken);

                if (subscriptionConfiguration == null)
                {
                    throw new NotFoundException($"Subscription Group with Id {subscriptionConfiguration} not found");
                }

                // Reset the Stream Position
                subscriptionConfiguration.StreamPosition = null;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Gets the next catchup subscription configuration.
        /// </summary>
        /// <param name="eventStoreServerId">The event store server identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<CatchupSubscriptionConfiguration> GetNextCatchupSubscriptionConfiguration(Guid eventStoreServerId, CancellationToken cancellationToken)
        {
            CatchupSubscriptionConfiguration result = null;

            using (SubscriptionServiceConfigurationContext context = this.ContextResolver())
            {
                // Find the next subscription based on Create date (only 1 is returned to simplify processing)
                Database.Models.CatchupSubscriptionConfiguration nextCatchUpSubscription = await context
                                                                                                 .CatchupSubscriptionConfigurations
                                                                                                 .Where(c => c.EventStoreServerId == eventStoreServerId)
                                                                                                 .OrderBy(c => c.CreateDateTime).FirstOrDefaultAsync(cancellationToken);

                // Check if we have found a subscription to process
                if (nextCatchUpSubscription != null)
                {
                    result = new CatchupSubscriptionConfiguration
                    {
                                 Name = nextCatchUpSubscription.Name,
                                 SubscriptionId = nextCatchUpSubscription.SubscriptionId,
                                 EventStoreServerId = nextCatchUpSubscription.EventStoreServerId,
                                 Position = nextCatchUpSubscription.Position,
                                 StreamName = nextCatchUpSubscription.StreamName,
                                 EndPointUri = nextCatchUpSubscription.EndPointUri,
                                 CreateDateTime = nextCatchUpSubscription.CreateDateTime
                             };
                }
            }

            return result;
        }

        public async Task<CatchupSubscriptionConfiguration> GetCatchupSubscriptionConfiguration(Guid subscriptionId,
                                                              CancellationToken cancellationToken)
        {
            CatchupSubscriptionConfiguration result = new CatchupSubscriptionConfiguration();

            using (SubscriptionServiceConfigurationContext context = this.ContextResolver())
            {
                Database.Models.CatchupSubscriptionConfiguration catchupSubscriptionConfiguration =
                    await context.CatchupSubscriptionConfigurations.Where(s => s.SubscriptionId == subscriptionId).SingleOrDefaultAsync(cancellationToken);

                result = new CatchupSubscriptionConfiguration
                         {
                             EventStoreServerId = catchupSubscriptionConfiguration.EventStoreServerId,
                             EndPointUri = catchupSubscriptionConfiguration.EndPointUri,
                             Name = catchupSubscriptionConfiguration.Name,
                             StreamName = catchupSubscriptionConfiguration.StreamName,
                             Position = catchupSubscriptionConfiguration.Position,
                             SubscriptionId = catchupSubscriptionConfiguration.SubscriptionId,
                             CreateDateTime = catchupSubscriptionConfiguration.CreateDateTime
                };
            }

            return result;
        }

        /// <summary>
        /// Deletes the catch up subscription.
        /// </summary>
        /// <param name="catchUpSubscriptionId">The catch up subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="NotFoundException">Catch Up Subscription Group with Id {catchUpSubscriptionId} not found</exception>
        public async Task DeleteCatchUpSubscription(Guid catchUpSubscriptionId,
                                                    CancellationToken cancellationToken)
        {
            Guard.ThrowIfInvalidGuid(catchUpSubscriptionId, typeof(ArgumentNullException), "A catchup subscription Id must be provided");

            using (SubscriptionServiceConfigurationContext context = this.ContextResolver())
            {
                Database.Models.CatchupSubscriptionConfiguration catchUpSubscriptionConfiguration = await context.CatchupSubscriptionConfigurations.Where(c => c.SubscriptionId == catchUpSubscriptionId)
                                                       .SingleOrDefaultAsync(cancellationToken);

                if (catchUpSubscriptionConfiguration == null)
                {
                    throw new NotFoundException($"Catch Up Subscription Group with Id {catchUpSubscriptionId} not found");
                }

                context.CatchupSubscriptionConfigurations.Remove(catchUpSubscriptionConfiguration);

                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
