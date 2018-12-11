using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.EventStore;
using Shared.General;
using SubscriptionService.Database;
using SubscriptionService.DataTransferObjects;

namespace SubscriptionService.BusinessLogic.Repository
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        #region Fields

        /// <summary>
        /// The context resolver
        /// </summary>
        private readonly Func<SubscriptionServiceConfigurationContext> ContextResolver;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationRepository"/> class.
        /// </summary>
        /// <param name="contextResolver">The context resolver.</param>
        public ConfigurationRepository(Func<SubscriptionServiceConfigurationContext> contextResolver)
        {
            this.ContextResolver = contextResolver;
        }

        #endregion

        #region Public Methods

        #region public async Task<List<SubscriptionGroup>> GetSubscriptions(CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the subscriptions.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<SubscriptionGroup>> GetSubscriptions(CancellationToken cancellationToken)
        {
            List<SubscriptionGroup> result = new List<SubscriptionGroup>();

            using (var context = this.ContextResolver())
            {
                var subscriptionGroups = await context.SubscriptionGroups.Include(g => g.SubscriptionStream).ToListAsync(cancellationToken);

                foreach (var subscriptionGroup in subscriptionGroups)
                {
                    result.Add(new SubscriptionGroup
                    {
                        SubscriptionGroupId = subscriptionGroup.Id,
                        GroupName = subscriptionGroup.Name,
                        StreamPositionToRestartFrom = subscriptionGroup.StreamPosition,
                        StreamName = subscriptionGroup.SubscriptionStream.StreamName,
                        BufferSize = subscriptionGroup.BufferSize
                    });
                }
            }

            return result;
        }
        #endregion

        #region public async Task ResetSubscriptionStreamPosition(Guid subscriptionId, CancellationToken cancellationToken)        
        /// <summary>
        /// Resets the subscription stream position.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task ResetSubscriptionStreamPosition(Guid subscriptionId, CancellationToken cancellationToken)
        {
            Guard.ThrowIfInvalidGuid(subscriptionId, typeof(ArgumentNullException), "A subscription Id must be provided");

            using (var context = this.ContextResolver())
            {
                var subscriptionGroup = await context.SubscriptionGroups.Where(g => g.Id == subscriptionId)
                    .SingleOrDefaultAsync(cancellationToken);

                if (subscriptionGroup == null)
                {
                    throw new NotFoundException($"Subscription Group with Id {subscriptionId} not found");
                }

                // Reset the Stream Position
                subscriptionGroup.StreamPosition = null;

                await context.SaveChangesAsync(cancellationToken);
            }
        }
        #endregion

        #region public async Task<EndPoint> GetEndPointForSubscriptionGroup(Guid subscriptionGroupId, CancellationToken cancellationToken)
        /// <summary>
        /// Gets the end point for subscription group.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<EndPoint> GetEndPointForSubscriptionGroup(Guid subscriptionGroupId, CancellationToken cancellationToken)
        {
            Guard.ThrowIfInvalidGuid(subscriptionGroupId, typeof(ArgumentNullException), "A subscription group Id must be provided");

            EndPoint result = null;

            using (var context = this.ContextResolver())
            {
                var endpoint = await context.SubscriptionGroups.Include(s => s.EndPoint)
                    .Where(s => s.Id == subscriptionGroupId).SingleOrDefaultAsync(cancellationToken);

                if (endpoint == null)
                {
                    throw new NotFoundException($"End Point for Subscription Group with Id {subscriptionGroupId} not found");
                }

                result = new EndPoint
                {
                    EndPointId = endpoint.EndPointId,
                    Name = endpoint.Name,
                    Url = endpoint.EndPoint.Url
                };
            }

            return result;
        }
        #endregion

        #region public async Task<EndPoint> GetEndPointForCatchUpSubscription(Guid catchupSubscriptionId, CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the end point for catch up subscription.
        /// </summary>
        /// <param name="catchupSubscriptionId">The catchup subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<EndPoint> GetEndPointForCatchUpSubscription(Guid catchupSubscriptionId, CancellationToken cancellationToken)
        {
            Guard.ThrowIfInvalidGuid(catchupSubscriptionId, typeof(ArgumentNullException), "A catchup subscription Id must be provided");

            EndPoint result = null;

            using (var context = this.ContextResolver())
            {
                var endpoint = await context.CatchUpSubscriptions.Include(c => c.EndPoint)
                    .Where(c => c.Id == catchupSubscriptionId).SingleOrDefaultAsync(cancellationToken);

                if (endpoint == null)
                {
                    throw new NotFoundException($"End Point for Catch Up Subscription Group with Id {catchupSubscriptionId} not found");
                }

                result = new EndPoint
                {
                    EndPointId = endpoint.EndPointId,
                    Name = endpoint.Name,
                    Url = endpoint.EndPoint.Url
                };
            }

            return result;
        }
        #endregion

        #region public async Task<CatchUpSubscription> GetNextCatchUpSubscription(CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the next catch up subscription.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<CatchUpSubscription> GetNextCatchUpSubscription(CancellationToken cancellationToken)
        {
            CatchUpSubscription result = null;

            using (var context = this.ContextResolver())
            {
                // Find the next subscription based on Create date (only 1 is returned to simplify processing)
                var nextCatchUpSubscription = await context.CatchUpSubscriptions.OrderBy(c => c.CreateDateTime).FirstOrDefaultAsync(cancellationToken);

                // Check if we have found a subscription to process
                if (nextCatchUpSubscription != null)
                {
                    result = new CatchUpSubscription
                    {
                        Name = nextCatchUpSubscription.Name,
                        CatchUpSubscriptionId = nextCatchUpSubscription.Id,
                        EndpointId = nextCatchUpSubscription.EndPointId,
                        Position = nextCatchUpSubscription.Position,
                        StreamName = nextCatchUpSubscription.StreamName
                    };
                }
            }

            return result;
        }
        #endregion

        #region public async Task DeleteCatchUpSubscription(Guid catchUpSubscriptionId, CancellationToken cancellationToken)        
        /// <summary>
        /// Deletes the catch up subscription.
        /// </summary>
        /// <param name="catchUpSubscriptionId">The catch up subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task DeleteCatchUpSubscription(Guid catchUpSubscriptionId, CancellationToken cancellationToken)
        {
            Guard.ThrowIfInvalidGuid(catchUpSubscriptionId, typeof(ArgumentNullException), "A catchup subscription Id must be provided");

            using (var context = this.ContextResolver())
            {
                var  catchUpSubscription = await context.CatchUpSubscriptions.Where(c => c.Id == catchUpSubscriptionId)
                    .SingleOrDefaultAsync(cancellationToken);

                if (catchUpSubscription != null)
                {
                    context.CatchUpSubscriptions.Remove(catchUpSubscription);
                    await context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    throw new NotFoundException($"Catch Up Subscription Group with Id {catchUpSubscriptionId} not found");
                }
            }
        }
        #endregion

        #endregion
    }
}