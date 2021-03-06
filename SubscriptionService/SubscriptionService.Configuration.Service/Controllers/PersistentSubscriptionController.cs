﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shared.EventStore;
using SubscriptionService.BusinessLogic.Repository;

namespace SubscriptionService.Configuration.Service.Controllers
{
    /*
    [Route("api/[controller]")]
    public class PersistentSubscriptionController : Controller
    {
        #region Fields

        /// <summary>
        /// The configuration repository
        /// </summary>
        private readonly IConfigurationRepository ConfigurationRepository;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentSubscriptionController"/> class.
        /// </summary>
        /// <param name="configurationRepository">The configuration repository.</param>
        public PersistentSubscriptionController(IConfigurationRepository configurationRepository)
        {
            this.ConfigurationRepository = configurationRepository;
        }

        #endregion

        #region Public Methods

        #region public async Task<IActionResult> CreateSubscriptionStream(Guid subscriptionStreamId, String streamName, CancellationToken cancellationToken)        
        /// <summary>
        /// Creates the subscription stream.
        /// </summary>
        /// <param name="subscriptionStreamId">The subscription stream identifier.</param>
        /// <param name="streamName">Name of the stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("subscriptionstream")]
        public async Task<IActionResult> CreateSubscriptionStream(Guid subscriptionStreamId, String streamName, CancellationToken cancellationToken)
        {
            var result = await this.ConfigurationRepository.CreateSubscriptionStream(subscriptionStreamId, streamName,
                SubscriptionType.Persistent, cancellationToken);

            return this.Ok(result);
        }
        #endregion

        #region public async Task<IActionResult> GetStreams(CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the streams.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("subscriptionstream")]
        public async Task<IActionResult> GetStreams(CancellationToken cancellationToken)
        {
            var result =
                await this.ConfigurationRepository.GetSubscriptionStreams(SubscriptionType.Persistent,
                    cancellationToken);


            return this.Ok(result);
        }
        #endregion

        #region public async Task<IActionResult> CreateSubscriptionGroup(Guid subscriptionGroupId, Guid subscriptionStreamId, String name, String url,CancellationToken cancellationToken)        
        /// <summary>
        /// Creates the subscription group.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="subscriptionStreamId">The subscription stream identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="url">The URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("subscriptiongroup")]
        public async Task<IActionResult> CreateSubscriptionGroup(Guid subscriptionGroupId, Guid subscriptionStreamId, String name, String url,CancellationToken cancellationToken)
        {
            // Create the endpoint
            Guid endpointId = await this.ConfigurationRepository.CreateEndPoint(name, url, cancellationToken);

            // Now create the subscription group
            subscriptionGroupId =
                await this.ConfigurationRepository.CreateSubscriptionGroup(subscriptionGroupId, subscriptionStreamId, endpointId, name, cancellationToken);

            return this.Ok(subscriptionGroupId);
        }
        #endregion

        #region public async Task<IActionResult> GetSubscriptionGroup(Guid subscriptionGroupId,CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the subscription group.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("subscriptiongroup/{subscriptionGroupId}")]
        public async Task<IActionResult> GetSubscriptionGroup(Guid subscriptionGroupId,CancellationToken cancellationToken)
        {
            var result =
                await this.ConfigurationRepository.GetSubscriptionGroup(subscriptionGroupId,
                    cancellationToken);

            return this.Ok(result);
        }
        #endregion

        #region public async Task<IActionResult> GetSubscriptionGroups(CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the subscription groups.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("subscriptiongroup")]
        public async Task<IActionResult> GetSubscriptionGroups(CancellationToken cancellationToken)
        {
            var result = await this.ConfigurationRepository.GetSubscriptionGroups(cancellationToken);

            return this.Ok(result);
        }
        #endregion

        #region public async Task<IActionResult> RemoveSubscriptionGroup(Guid subscriptionGroupId, CancellationToken cancellationToken)        
        /// <summary>
        /// Removes the subscription group.
        /// </summary>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("subscriptiongroup")]
        public async Task<IActionResult> RemoveSubscriptionGroup(Guid subscriptionGroupId, CancellationToken cancellationToken)
        {
            await this.ConfigurationRepository.RemoveSubscriptionGroup(subscriptionGroupId, cancellationToken);

            return this.Ok();
        }
        #endregion
        
        #region public async Task<IActionResult> CreateSubscriptionService(Guid subscriptionServiceId, String description, CancellationToken cancellationToken)        
        /// <summary>
        /// Creates the subscription service.
        /// </summary>
        /// <param name="subscriptionServiceId">The subscription service identifier.</param>
        /// <param name="description">The description.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]        
        [Route("subscriptionservice")]
        public async Task<IActionResult> CreateSubscriptionService(Guid subscriptionServiceId, String description, CancellationToken cancellationToken)
        {
            subscriptionServiceId =
                await this.ConfigurationRepository.CreateSubscriptionService(subscriptionServiceId, description,
                    cancellationToken);

            return this.Ok(subscriptionServiceId);
        }
        #endregion

        #region public async Task<IActionResult> AddSubscriptionGroupToSubscriptionService(Guid subscriptionServiceId, Guid[] subscriptionGroupId, CancellationToken cancellationToken)        
        /// <summary>
        /// Adds the subscription group to subscription service.
        /// </summary>
        /// <param name="subscriptionServiceId">The subscription service identifier.</param>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("subscriptionservice/{subscriptionServiceId}")]
        public async Task<IActionResult> AddSubscriptionGroupToSubscriptionService(Guid subscriptionServiceId, Guid[] subscriptionGroupId, CancellationToken cancellationToken)
        {
            foreach (Guid id in subscriptionGroupId)
            {
                // TODO: Eventually we cna create a call that takes in Guid[]
                await this.ConfigurationRepository.AddSubscriptionGroupToSubscriptionService(subscriptionServiceId, id, cancellationToken);
            }

            return this.Ok();
        }
        #endregion

        #region public async Task<IActionResult> GetSubscriptionService(Guid subscriptionServiceId, CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the subscription service.
        /// </summary>
        /// <param name="subscriptionServiceId">The subscription service identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("subscriptionservice/{subscriptionServiceId}")]
        public async Task<IActionResult> GetSubscriptionService(Guid subscriptionServiceId, CancellationToken cancellationToken)
        {
            var result = await this.ConfigurationRepository.GetSubscriptionGroups(cancellationToken);

            return this.Ok(result);
        }
        #endregion

        #region public async Task<IActionResult> GetSubscriptionServices(CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the subscription services.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("subscriptionservice")]
        public async Task<IActionResult> GetSubscriptionServices(CancellationToken cancellationToken)
        {
            var result = await this.ConfigurationRepository.GetSubscriptionGroups(cancellationToken);

            return this.Ok(result);
        }
        #endregion

        #region public async Task<IActionResult> RemoveSubscriptionGroupFromSubscriptionService(Guid subscriptionServiceId,Guid subscriptionGroupId, CancellationToken cancellationToken)        
        /// <summary>
        /// Removes the subscription group from subscription service.
        /// </summary>
        /// <param name="subscriptionServiceId">The subscription service identifier.</param>
        /// <param name="subscriptionGroupId">The subscription group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("subscriptionservice/{subscriptionServiceId}/{subscriptionGroupId}")]
        public async Task<IActionResult> RemoveSubscriptionGroupFromSubscriptionService(Guid subscriptionServiceId, Guid subscriptionGroupId, CancellationToken cancellationToken)
        {
            await this.ConfigurationRepository.RemoveSubscriptionGroupFromSubscriptionService(subscriptionServiceId,subscriptionGroupId, cancellationToken);

            return this.Ok();
        }

        #endregion

        #region public async Task<IActionResult> GetSubscriptionGroupsForSubscriptionService(Guid subscriptionServiceId, CancellationToken cancellationToken)        
        /// <summary>
        /// Gets the subscription groups for subscription service.
        /// </summary>
        /// <param name="subscriptionServiceId">The subscription service identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("subscriptionservice/{subscriptionServiceId}/subscriptionGroups")]
        public async Task<IActionResult> GetSubscriptionGroupsForSubscriptionService(Guid subscriptionServiceId, CancellationToken cancellationToken)
        {
            var subscriptionGroups = await this.ConfigurationRepository.GetSubscriptions(subscriptionServiceId, cancellationToken);
            
            return this.Ok(subscriptionGroups);
        }
        #endregion

        #endregion
    }
*/
    
}
