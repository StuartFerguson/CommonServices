using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.EventStore;
using Shared.General;
using SubscriptionService.Database;
using SubscriptionService.Database.Models;
using CatchUpSubscriptionDTO = SubscriptionService.DataTransferObjects.CatchUpSubscription;
using EndPointDTO = SubscriptionService.DataTransferObjects.EndPoint;
using SubscriptionGroupDTO = SubscriptionService.DataTransferObjects.SubscriptionGroup;
using SubscriptionStreamDTO = SubscriptionService.DataTransferObjects.SubscriptionStream;

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

        #region public async Task<List<SubscriptionGroupDTO>> GetSubscriptions(CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the subscriptions.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<SubscriptionGroupDTO>> GetSubscriptions(CancellationToken cancellationToken)
        {
            List<SubscriptionGroupDTO> result = new List<SubscriptionGroupDTO>();

            using (var context = this.ContextResolver())
            {
                var subscriptionGroups = await context.SubscriptionGroups.Include(g => g.SubscriptionStream).ToListAsync(cancellationToken);

                foreach (var subscriptionGroup in subscriptionGroups)
                {
                    result.Add(new SubscriptionGroupDTO
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

        #region public async Task ResetSubscriptionStreamPosition(Guid subscriptionGroupId, CancellationToken cancellationToken)        
        /// <summary>
        /// Resets the subscription stream position.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">Subscription Group with Id {subscriptionGroupId}</exception>
        public async Task ResetSubscriptionStreamPosition(Guid subscriptionGroupId, CancellationToken cancellationToken)
        {
            Guard.ThrowIfInvalidGuid(subscriptionGroupId, typeof(ArgumentNullException), "A subscription group Id must be provided");

            using (var context = this.ContextResolver())
            {
                var subscriptionGroup = await context.SubscriptionGroups.Where(g => g.Id == subscriptionGroupId)
                    .SingleOrDefaultAsync(cancellationToken);

                if (subscriptionGroup == null)
                {
                    throw new NotFoundException($"Subscription Group with Id {subscriptionGroupId} not found");
                }

                // Reset the Stream Position
                subscriptionGroup.StreamPosition = null;

                await context.SaveChangesAsync(cancellationToken);
            }
        }
        #endregion

        #region public async Task<EndPointDTO> GetEndPointForSubscriptionGroup(Guid subscriptionGroupId, CancellationToken cancellationToken)
        /// <summary>
        /// Gets the end point for subscription group.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<EndPointDTO> GetEndPointForSubscriptionGroup(Guid subscriptionGroupId, CancellationToken cancellationToken)
        {
            Guard.ThrowIfInvalidGuid(subscriptionGroupId, typeof(ArgumentNullException), "A subscription group Id must be provided");

            EndPointDTO result = null;

            using (var context = this.ContextResolver())
            {
                var endpoint = await context.SubscriptionGroups.Include(s => s.EndPoint)
                    .Where(s => s.Id == subscriptionGroupId).SingleOrDefaultAsync(cancellationToken);

                if (endpoint == null)
                {
                    throw new NotFoundException($"End Point for Subscription Group with Id {subscriptionGroupId} not found");
                }

                result = new EndPointDTO
                {
                    EndPointId = endpoint.EndPointId,
                    Name = endpoint.Name,
                    Url = endpoint.EndPoint.Url
                };
            }

            return result;
        }
        #endregion

        #region public async Task<EndPointDTO> GetEndPointForCatchUpSubscription(Guid catchupSubscriptionId, CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the end point for catch up subscription.
        /// </summary>
        /// <param name="catchupSubscriptionId">The catchup subscription identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<EndPointDTO> GetEndPointForCatchUpSubscription(Guid catchupSubscriptionId, CancellationToken cancellationToken)
        {
            Guard.ThrowIfInvalidGuid(catchupSubscriptionId, typeof(ArgumentNullException), "A catchup subscription Id must be provided");

            EndPointDTO result = null;

            using (var context = this.ContextResolver())
            {
                var endpoint = await context.CatchUpSubscriptions.Include(c => c.EndPoint)
                    .Where(c => c.Id == catchupSubscriptionId).SingleOrDefaultAsync(cancellationToken);

                if (endpoint == null)
                {
                    throw new NotFoundException($"End Point for Catch Up Subscription Group with Id {catchupSubscriptionId} not found");
                }

                result = new EndPointDTO
                {
                    EndPointId = endpoint.EndPointId,
                    Name = endpoint.Name,
                    Url = endpoint.EndPoint.Url
                };
            }

            return result;
        }
        #endregion

        #region public async Task<CatchUpSubscriptionDTO> GetNextCatchUpSubscription(CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the next catch up subscription.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<CatchUpSubscriptionDTO> GetNextCatchUpSubscription(CancellationToken cancellationToken)
        {
            CatchUpSubscriptionDTO result = null;

            using (var context = this.ContextResolver())
            {
                // Find the next subscription based on Create date (only 1 is returned to simplify processing)
                var nextCatchUpSubscription = await context.CatchUpSubscriptions.OrderBy(c => c.CreateDateTime).FirstOrDefaultAsync(cancellationToken);

                // Check if we have found a subscription to process
                if (nextCatchUpSubscription != null)
                {
                    result = new CatchUpSubscriptionDTO
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

                if (catchUpSubscription == null)
                {
                    throw new NotFoundException($"Catch Up Subscription Group with Id {catchUpSubscriptionId} not found");
                }

                context.CatchUpSubscriptions.Remove(catchUpSubscription);

                await context.SaveChangesAsync(cancellationToken);
            }
        }
        #endregion

        #region public async Task<Guid> CreateSubscriptionStream(Guid subscriptionStreamId, String streamName, SubscriptionType subscriptionType, CancellationToken cancellationToken)        
        /// <summary>
        /// Creates the subscription stream.
        /// </summary>
        /// <param name="subscriptionStreamId">The subscription stream identifier.</param>
        /// <param name="streamName">Name of the stream.</param>
        /// <param name="subscriptionType">Type of the subscription.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<Guid> CreateSubscriptionStream(Guid subscriptionStreamId, String streamName, SubscriptionType subscriptionType, CancellationToken cancellationToken)
        {
            Guard.ThrowIfNullOrEmpty(streamName, typeof(ArgumentNullException), "Stream name must not be null or empty to Create a new Subscription Stream");

            if (subscriptionStreamId == Guid.Empty)
            {
                subscriptionStreamId = Guid.NewGuid();
            }

            using (var context = this.ContextResolver())
            {
                var isDuplicate = await context.SubscriptionStream.Where(s => s.Id == subscriptionStreamId).AnyAsync(cancellationToken);

                if (isDuplicate)
                {
                    throw new InvalidOperationException($"Subscription Stream with Id {subscriptionStreamId} already exists.");
                }

                SubscriptionStream subscriptionStream = new SubscriptionStream
                {
                    Id = subscriptionStreamId,
                    StreamName = streamName,
                    SubscriptionType = (Int32) subscriptionType,                    
                };

                context.SubscriptionStream.Add(subscriptionStream);

                await context.SaveChangesAsync(cancellationToken);
            }

            return subscriptionStreamId;
        }
        #endregion

        #region public async Task<List<SubscriptionStreamDTO>> GetSubscriptionStreams(SubscriptionType subscriptionType, CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the subscription streams.
        /// </summary>
        /// <param name="subscriptionType">Type of the subscription.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<SubscriptionStreamDTO>> GetSubscriptionStreams(SubscriptionType subscriptionType, CancellationToken cancellationToken)
        {
            List<SubscriptionStreamDTO> result = new List<SubscriptionStreamDTO>();

            using (var context = this.ContextResolver())
            {
                var subscriptionStreams = await context.SubscriptionStream.Where(s => s.SubscriptionType == (Int32)subscriptionType).ToListAsync(cancellationToken);

                if (subscriptionStreams.Any())
                {
                    result=new List<SubscriptionStreamDTO>();

                    foreach (var subscriptionStream in subscriptionStreams)
                    {
                        result.Add(new SubscriptionStreamDTO
                        {
                            StreamName = subscriptionStream.StreamName,
                            SubscriptionType = subscriptionStream.SubscriptionType,
                            Id = subscriptionStream.Id
                        });
                    }
                }
            }

            return result;
        }
        #endregion

        #region public async Task<Guid> CreateEndPoint(String endpointName, String url, CancellationToken cancellationToken)        
        /// <summary>
        /// Creates the end point.
        /// </summary>
        /// <param name="endpointName">Name of the endpoint.</param>
        /// <param name="url">The URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<Guid> CreateEndPoint(String endpointName, String url, CancellationToken cancellationToken)
        {
            Guard.ThrowIfNullOrEmpty(endpointName, typeof(ArgumentNullException), "An endpoint name must be specified to add an endpoint");
            Guard.ThrowIfNullOrEmpty(url, typeof(ArgumentNullException), "An endpoint url must be specified to add an endpoint");

            Guid endpointId = Guid.Empty;

            using (var context = this.ContextResolver())
            {
                endpointId = Guid.NewGuid();
                EndPoint endPoint = new EndPoint
                {
                    Name = endpointName,
                    EndPointId = endpointId,
                    Url = url
                };

                context.EndPoints.Add(endPoint);

                await context.SaveChangesAsync(cancellationToken);
            }

            return endpointId;
        }
        #endregion

        #region public async Task<Guid> CreateSubscriptionGroup(Guid subscriptionGroupId, Guid subscriptionStreamId, Guid endpointId, String subscriptionGroupName, CancellationToken cancellationToken)        
        /// <summary>
        /// Creates the subscription group.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="subscriptionStreamId">The subscription stream identifier.</param>
        /// <param name="endpointId">The endpoint identifier.</param>
        /// <param name="subscriptionGroupName">Name of the subscription group.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Subscription Group with Id {subscriptionGroupId}</exception>
        public async Task<Guid> CreateSubscriptionGroup(Guid subscriptionGroupId, Guid subscriptionStreamId, Guid endpointId, String subscriptionGroupName, CancellationToken cancellationToken)
        {
            Guard.ThrowIfInvalidGuid(subscriptionStreamId, typeof(ArgumentNullException), "An subscription stream Id must be specified to add a subscription group");
            Guard.ThrowIfInvalidGuid(endpointId, typeof(ArgumentNullException), "An endpoint id must be specified to add a subscription group");
            Guard.ThrowIfNullOrEmpty(subscriptionGroupName, typeof(ArgumentNullException), "An subscription group name must be specified to add a subscription group");

            using (var context = this.ContextResolver())
            {
                if (subscriptionGroupId == Guid.Empty)
                {
                    subscriptionGroupId = Guid.NewGuid();
                }

                var isDuplicate = await context.SubscriptionGroups.Where(s => s.Id == subscriptionGroupId).AnyAsync(cancellationToken);

                if (isDuplicate)
                {
                    throw new InvalidOperationException($"Subscription Group with Id {subscriptionGroupId} already exists.");
                }

                SubscriptionGroup subscriptionGroup = new SubscriptionGroup
                {
                    EndPointId = endpointId,
                    Id = subscriptionGroupId,
                    Name = subscriptionGroupName,
                    SubscriptionStreamId = subscriptionStreamId
                };

                context.SubscriptionGroups.Add(subscriptionGroup);

                await context.SaveChangesAsync(cancellationToken);
            }

            return subscriptionGroupId;
        }
        #endregion

        #region public async Task<SubscriptionGroup> GetSubscriptionGroup(Guid subscriptionGroupId,CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the subscription group.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">Subscription group with Id {subscriptionGroupId}</exception>
        public async Task<SubscriptionGroupDTO> GetSubscriptionGroup(Guid subscriptionGroupId,CancellationToken cancellationToken)
        {
            Guard.ThrowIfInvalidGuid(subscriptionGroupId, typeof(ArgumentNullException), "A subscription group Id must be provided");

            SubscriptionGroupDTO result = null;

            using (var context = this.ContextResolver())
            {
                var subscriptionGroup = await context.SubscriptionGroups
                    .Include(s => s.SubscriptionStream)
                    .Where(s => s.Id == subscriptionGroupId)
                    .SingleOrDefaultAsync(cancellationToken);

                if (subscriptionGroup == null)
                {
                    throw new NotFoundException($"Subscription group with Id {subscriptionGroupId} not found");
                }

                result = new SubscriptionGroupDTO
                {
                    StreamName = subscriptionGroup.SubscriptionStream.StreamName,
                    GroupName = subscriptionGroup.Name,
                    SubscriptionGroupId = subscriptionGroup.Id
                };
            }

            return result;
        }
        #endregion

        #region public async Task<SubscriptionGroup> GetSubscriptionGroups(CancellationToken cancellationToken)                
        /// <summary>
        /// Gets the subscription groups.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>

        public async Task<List<SubscriptionGroupDTO>> GetSubscriptionGroups(CancellationToken cancellationToken)
        {
            List<SubscriptionGroupDTO> result =new List<SubscriptionGroupDTO>();

            using (var context = this.ContextResolver())
            {
                var subscriptionGroups = await context.SubscriptionGroups.Include(s => s.SubscriptionStream).ToListAsync(cancellationToken);

                foreach (var subscriptionGroup in subscriptionGroups)
                {
                    result.Add(new SubscriptionGroupDTO
                    {
                        StreamName = subscriptionGroup.SubscriptionStream.StreamName,
                        GroupName = subscriptionGroup.Name,
                        SubscriptionGroupId = subscriptionGroup.Id
                    });
                }
            }

            return result;
        }
        #endregion

        #region public async Task RemoveSubscriptionGroup(Guid subscriptionGroupId, CancellationToken cancellationToken)        
        /// <summary>
        /// Removes the subscription group.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task RemoveSubscriptionGroup(Guid subscriptionGroupId, CancellationToken cancellationToken)
        {
            Guard.ThrowIfInvalidGuid(subscriptionGroupId, typeof(ArgumentNullException), "A subscription group Id must be provided");

            using (var context = this.ContextResolver())
            {
                var  subscriptionGroup = await context.SubscriptionGroups.Where(s => s.Id == subscriptionGroupId)
                    .SingleOrDefaultAsync(cancellationToken);

                if (subscriptionGroup == null)
                {
                    throw new NotFoundException($"Subscription Group with Id {subscriptionGroupId} not found");
                }

                context.SubscriptionGroups.Remove(subscriptionGroup);

                await context.SaveChangesAsync(cancellationToken);
            }
        }
        #endregion

        #endregion
    }
}