using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.EventStore;
using Shouldly;
using SubscriptionService.BusinessLogic.Repository;
using SubscriptionService.Database;
using SubscriptionService.Database.Models;
using Xunit;
using SubscriptionServiceModel = SubscriptionService.Database.Models.SubscriptionService;

namespace SubscriptionService.UnitTests
{
    using Shared.Exceptions;

    public class ConfigurationRepositoryTests
    {
        private SubscriptionServiceConfigurationContext GetContext(String databaseName)
        {
            DbContextOptionsBuilder<SubscriptionServiceConfigurationContext> builder = new DbContextOptionsBuilder<SubscriptionServiceConfigurationContext>()
                .UseInMemoryDatabase(databaseName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            SubscriptionServiceConfigurationContext context = new SubscriptionServiceConfigurationContext(builder.Options);
            
            return context;
        }

        #region Create Tests

        [Fact]
        public void ConfigurationRepository_CanBeCreated_IsCreated()
        {
            var context = GetContext(Guid.NewGuid().ToString("N"));

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };

            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            repository.ShouldNotBeNull();
        }

        #endregion

        #region Get Subscriptions Tests

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptions_ListReturned()
        {            
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.GetSubscriptionsData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var subscriptionGroups = await repository.GetSubscriptions(SubscriptionServiceConfigurationTestData.SubscriptionServiceId, CancellationToken.None);

            subscriptionGroups.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptions_NoSubscriptions_EmptyListReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            
            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var subscriptionGroups = await repository.GetSubscriptions(SubscriptionServiceConfigurationTestData.SubscriptionServiceId, CancellationToken.None);

            subscriptionGroups.ShouldBeEmpty();
        }

        #endregion

        #region Reset Subscription Stream Position Tests

        [Fact]
        public async Task ConfigurationRepository_ResetSubscriptionStreamPosition_PositionReset()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.ResetSubscriptionStreamPositionData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionGroupId = SubscriptionServiceConfigurationTestData.SubscriptionGroupId;
            await repository.ResetSubscriptionStreamPosition(subscriptionGroupId, CancellationToken.None);

            var verifyContext = GetContext(databaseName);
            verifyContext.SubscriptionGroups.First().StreamPosition.ShouldBeNull();
        }

        [Fact]
        public void ConfigurationRepository_ResetSubscriptionStreamPosition_InvalidSubscriptionGroupId_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.ResetSubscriptionStreamPositionData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<ArgumentNullException>(async () =>
            {
                await repository.ResetSubscriptionStreamPosition(Guid.Empty, CancellationToken.None);
            });            
        }

        [Fact]
        public void ConfigurationRepository_ResetSubscriptionStreamPosition_SubscriptionIdNotFound_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<NotFoundException>(async () =>
            {
                Guid subscriptionGroupId = SubscriptionServiceConfigurationTestData.SubscriptionGroupId;
                await repository.ResetSubscriptionStreamPosition(subscriptionGroupId, CancellationToken.None);
            });            
        }

        #endregion

        #region Get EndPoint For Subscription Group Tests

        [Fact]
        public async Task ConfigurationRepository_GetEndPointForSubscriptionGroup_EndPointReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.GetEndPointForSubscriptionGroupData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionGroupId = SubscriptionServiceConfigurationTestData.SubscriptionGroupId;
            var endPoint = await repository.GetEndPointForSubscriptionGroup(subscriptionGroupId, CancellationToken.None);

            endPoint.ShouldNotBeNull();
        }

        [Fact]
        public async Task ConfigurationRepository_GetEndPointForSubscriptionGroup_InvalidSubscriptionGroupId_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.GetEndPointForSubscriptionGroupData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<ArgumentNullException>(async () =>
            {
                await repository.GetEndPointForSubscriptionGroup(Guid.Empty, CancellationToken.None);
            });
        }

        [Fact]
        public async Task ConfigurationRepository_GetEndPointForSubscriptionGroup_EndPointNotFound_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionGroupId = SubscriptionServiceConfigurationTestData.SubscriptionGroupId;
            Should.Throw<NotFoundException>(async () =>
            {
                await repository.GetEndPointForSubscriptionGroup(subscriptionGroupId, CancellationToken.None);
            });
        }

        #endregion

        #region Get EndPoint For CatchUp Subscription Tests

        [Fact]
        public async Task ConfigurationRepository_GetEndPointForCatchUpSubscription_EndPointReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.GetEndPointForCatchUpSubscriptionData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid catchUpSubscriptionId = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionId;
            var endPoint = await repository.GetEndPointForCatchUpSubscription(catchUpSubscriptionId, CancellationToken.None);

            endPoint.ShouldNotBeNull();
        }

        [Fact]
        public async Task ConfigurationRepository_GetEndPointForCatchUpSubscription_InvalidSubscriptionId_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.GetEndPointForSubscriptionGroupData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<ArgumentNullException>(async () =>
            {
                await repository.GetEndPointForCatchUpSubscription(Guid.Empty, CancellationToken.None);
            });

        }

        [Fact]
        public async Task ConfigurationRepository_GetEndPointForCatchUpSubscription_EndPointNotFound_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };

            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<NotFoundException>(async () =>
            {
                Guid catchUpSubscriptionId = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionId;
                await repository.GetEndPointForCatchUpSubscription(catchUpSubscriptionId, CancellationToken.None);
            });
        }

        #endregion

        #region Get Next CatchUp Subscription Tests

        [Fact]
        public async Task ConfigurationRepository_GetNextCatchupSubscription_SubscriptionReturned()
        {            
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.GetNextCatchupSubscriptionData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var catchUpSubscription = await repository.GetNextCatchUpSubscription(CancellationToken.None);

            catchUpSubscription.ShouldNotBeNull();
        }

        [Fact]
        public async Task ConfigurationRepository_GetNextCatchupSubscription_NoCatchupSubscriptions_NullReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            
            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var catchUpSubscription = await repository.GetNextCatchUpSubscription(CancellationToken.None);

            catchUpSubscription.ShouldBeNull();
        }

        #endregion

        #region Delete CatchUp Subscription Tests

        [Fact]
        public async Task ConfigurationRepository_DeleteCatchUpSubscription_SubscriptionDeleted()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.DeleteCatchUpSubscriptionData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid catchUpSubscriptionId = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionId;
            await repository.DeleteCatchUpSubscription(catchUpSubscriptionId, CancellationToken.None);

            var verifyContext = GetContext(databaseName);
            verifyContext.SubscriptionGroups.Count().ShouldBe(0);
        }

        [Fact]
        public void ConfigurationRepository_DeleteCatchUpSubscription_InvalidSubscriptionId_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.DeleteCatchUpSubscriptionData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<ArgumentNullException>(async () =>
            {
                await repository.DeleteCatchUpSubscription(Guid.Empty, CancellationToken.None);
            });            
        }

        [Fact]
        public void ConfigurationRepository_DeleteCatchUpSubscription_SubscriptionIdNotFound_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<NotFoundException>(async () =>
            {
                Guid catchUpSubscriptionId = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionId;
                await repository.DeleteCatchUpSubscription(catchUpSubscriptionId, CancellationToken.None);
            });            
        }

        #endregion

        #region Create Subscription Stream Tests

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ConfigurationRepository_CreateSubscriptionStream_SubscriptionStreamCreated(Boolean emptyGuid)
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionStreamId =
                emptyGuid ? Guid.Empty : SubscriptionServiceConfigurationTestData.SubscriptionStreamId;
            String streamName = SubscriptionServiceConfigurationTestData.SubscriptionStreamName;

            var result = await repository.CreateSubscriptionStream(subscriptionStreamId, streamName,
                SubscriptionType.Persistent, CancellationToken.None);

            var verifyContext = GetContext(databaseName);
            verifyContext.SubscriptionStream.Count().ShouldBe(1);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ConfigurationRepository_CreateSubscriptionStream_InvalidStreamName_ErrorThrown(String streamName)
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionStreamId = Guid.Parse("876BBEBF-3354-4E4D-8BA0-EE322211155B");

            Should.Throw<ArgumentNullException>(async () =>
            {
                var result = await repository.CreateSubscriptionStream(subscriptionStreamId, streamName,
                    SubscriptionType.Persistent, CancellationToken.None);
            });
        }

        [Fact]
        public void ConfigurationRepository_CreateSubscriptionStream_DuplicateSubscriptionStreamId_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.CreateSubscriptionStreamData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionStreamId = Guid.Parse("876BBEBF-3354-4E4D-8BA0-EE322211155B");
            String streamName = "TestStream";

            Should.Throw<InvalidOperationException>(async () =>
            {
                var result = await repository.CreateSubscriptionStream(subscriptionStreamId, streamName,
                    SubscriptionType.Persistent, CancellationToken.None);
            });
        }

        #endregion

        #region Get Subscription Streams Tests

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptionStreams_StreamsReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.GetSubscriptionStreamsData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var subscriptionStreams = await repository.GetSubscriptionStreams(SubscriptionType.Persistent, CancellationToken.None);

            subscriptionStreams.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptionStreams_NoSubscriptionStreams_EmptyListReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var subscriptionStreams = await repository.GetSubscriptionStreams(SubscriptionType.Persistent, CancellationToken.None);

            subscriptionStreams.ShouldBeEmpty();
        }
        #endregion

        #region Create EndPoint Tests
        [Fact]
        public async Task ConfigurationRepository_CreateEndPoint_EndPointCreated()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            String endPointName = SubscriptionServiceConfigurationTestData.EndPointName;
            String endpointUrl = SubscriptionServiceConfigurationTestData.EndPointUrl;

            var result = await repository.CreateEndPoint(endPointName, endpointUrl, CancellationToken.None);

            var verifyContext = GetContext(databaseName);
            verifyContext.EndPoints.Count().ShouldBe(1);
        }

        [Theory]
        [InlineData("", "url")]
        [InlineData(null, "url")]
        [InlineData("endPointName", "")]
        [InlineData("endPointName", null)]
        public void ConfigurationRepository_CreateEndPoint_InvalidData_ErrorThrown(String endPointName, String endpointUrl)
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<ArgumentNullException>(async () =>
            {
                await repository.CreateEndPoint(endPointName, endpointUrl, CancellationToken.None);
            });
        }

        #endregion

        #region Create Subscription Group Tests

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ConfigurationRepository_CreateSubscriptionGroup_SubscriptionGroupCreated(Boolean emptyGuid)
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionGroupId =
                emptyGuid ? Guid.Empty : SubscriptionServiceConfigurationTestData.SubscriptionGroupId;
            Guid subscriptionStreamId = SubscriptionServiceConfigurationTestData.SubscriptionStreamId;
            Guid endpointId = SubscriptionServiceConfigurationTestData.EndPointId;
            String subscriptionGroupName = SubscriptionServiceConfigurationTestData.SubscriptionGroupName;

            var result = await repository.CreateSubscriptionGroup(subscriptionGroupId, subscriptionStreamId, endpointId,
                subscriptionGroupName, CancellationToken.None);

            var verifyContext = GetContext(databaseName);
            verifyContext.SubscriptionGroups.Count().ShouldBe(1);
        }

        [Theory]
        [InlineData(false, true, "groupName")]
        [InlineData(true, false, "groupName")]
        [InlineData(true, true, "")]
        [InlineData(true, true, null)]
        public void ConfigurationRepository_CreateSubscriptionGroup_InvalidData_ErrorThrown(Boolean validSubscriptionStreamId, Boolean validEndPointId, String subscriptionGroupName)
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionGroupId = Guid.Parse("17191786-9D66-4752-81E2-CBD3A03C2E13");
            Guid subscriptionStreamId = validSubscriptionStreamId ? Guid.Parse("876BBEBF-3354-4E4D-8BA0-EE322211155B") : Guid.Empty;
            Guid endpointId = validEndPointId ? Guid.Parse("CBCB485D-F510-4ADE-B9B8-27208468B9B9") : Guid.Empty;

            Should.Throw<ArgumentNullException>(async () =>
            {
                await repository.CreateSubscriptionGroup(subscriptionGroupId, subscriptionStreamId, endpointId,
                    subscriptionGroupName, CancellationToken.None);
            });
        }

        [Fact]
        public void ConfigurationRepository_CreateSubscriptionGroup_DuplicateSubscriptionGroupId_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.CreateSubscriptionGroupData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionGroupId = SubscriptionServiceConfigurationTestData.SubscriptionGroupId;
            Guid subscriptionStreamId = SubscriptionServiceConfigurationTestData.SubscriptionStreamId;
            Guid endpointId = SubscriptionServiceConfigurationTestData.EndPointId;
            String subscriptionGroupName = SubscriptionServiceConfigurationTestData.SubscriptionGroupName;

            Should.Throw<InvalidOperationException>(async () =>
            {
                var result = await repository.CreateSubscriptionGroup(subscriptionGroupId, subscriptionStreamId, endpointId, subscriptionGroupName, CancellationToken.None);
            });
        }

        #endregion

        #region Get Subscription Group Tests

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptionGroup_SubscriptionGroupReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.GetSubscriptionGroupData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionGroupId = SubscriptionServiceConfigurationTestData.SubscriptionGroupId;
            var subscriptionGroup = await repository.GetSubscriptionGroup(subscriptionGroupId, CancellationToken.None);

            subscriptionGroup.ShouldNotBeNull();
        }

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptionGroup_InvalidSubscriptionGroupId_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<ArgumentNullException>(async () =>
            {
                await repository.GetSubscriptionGroup(Guid.Empty, CancellationToken.None);
            });
        }

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptionGroup_SubscriptionGroupNotFound_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionGroupId = SubscriptionServiceConfigurationTestData.SubscriptionGroupId;
            Should.Throw<NotFoundException>(async () =>
            {
                await repository.GetSubscriptionGroup(subscriptionGroupId, CancellationToken.None);
            });
        }
        #endregion

        #region Get Subscription Groups Tests

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptionGroups_SubscriptionGroupsReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.GetSubscriptionGroupData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var subscriptionGroups = await repository.GetSubscriptionGroups(CancellationToken.None);

            subscriptionGroups.ShouldNotBeEmpty();
        }
        
        [Fact]
        public async Task ConfigurationRepository_GetSubscriptionGroups_SubscriptionGroupsNotFound_EmptyListReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var subscriptionGroups = await repository.GetSubscriptionGroups(CancellationToken.None);

            subscriptionGroups.ShouldBeEmpty();
        }

        #endregion

        #region Remove Subscription Group Tests

        [Fact]
        public async Task ConfigurationRepository_RemoveSubscriptionGroup_SubscriptionGroupDeleted()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.RemoveSubscriptionGroupData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionGroupId = SubscriptionServiceConfigurationTestData.SubscriptionGroupId;
            await repository.RemoveSubscriptionGroup(subscriptionGroupId, CancellationToken.None);

            var verifyContext = GetContext(databaseName);
            verifyContext.SubscriptionGroups.Count().ShouldBe(0);
        }

        [Fact]
        public void ConfigurationRepository_RemoveSubscriptionGroup_InvalidSubscriptionGroupId_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<ArgumentNullException>(async () =>
            {
                await repository.RemoveSubscriptionGroup(Guid.Empty, CancellationToken.None);
            });            
        }

        [Fact]
        public void ConfigurationRepository_RemoveSubscriptionGroup_SubscriptionGroupIdNotFound_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            //context.AddTestData(TestScenario.RemoveSubscriptionGroupData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<NotFoundException>(async () =>
            {
                Guid subscriptionGroupId = SubscriptionServiceConfigurationTestData.SubscriptionGroupId;
                await repository.RemoveSubscriptionGroup(subscriptionGroupId, CancellationToken.None);
            });            
        }

        #endregion

        #region Create Subscription Service Tests

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ConfigurationRepository_CreateSubscriptionService_SubscriptionServiceCreated(Boolean emptyGuid)
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionServiceId =
                emptyGuid ? Guid.Empty : SubscriptionServiceConfigurationTestData.SubscriptionServiceId;
            var result = await repository.CreateSubscriptionService(subscriptionServiceId, SubscriptionServiceConfigurationTestData.SubscriptionServiceDescription, CancellationToken.None);

            result.ShouldNotBe(Guid.Empty);
            var verifyContext = GetContext(databaseName);
            verifyContext.SubscriptionServices.Count().ShouldBe(1);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ConfigurationRepository_CreateSubscriptionService_InvalidData_ErrorThrown(String subscriptionServiceDescription)
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<ArgumentNullException>(async () =>
            {
                await repository.CreateSubscriptionService(SubscriptionServiceConfigurationTestData.SubscriptionServiceId, 
                    subscriptionServiceDescription, CancellationToken.None);
            });
        }

        #endregion

        #region Add Subscription Group To Subscription Service Tests

        [Fact]
        public async Task ConfigurationRepository_AddSubscriptionGroupToSubscriptionService_SubscriptionGroutAddedToSubscriptionService()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var result = await repository.AddSubscriptionGroupToSubscriptionService(SubscriptionServiceConfigurationTestData.SubscriptionServiceId, 
                SubscriptionServiceConfigurationTestData.SubscriptionGroupId, CancellationToken.None);

            result.ShouldNotBe(Guid.Empty);
            var verifyContext = GetContext(databaseName);
            verifyContext.SubscriptionServiceGroups.Count().ShouldBe(1);
        }

        [Theory]
        [InlineData(true,false)]
        [InlineData(false,true)]
        public void ConfigurationRepository_AddSubscriptionGroupToSubscriptionService_InvalidData_ErrorThrown(Boolean validSubscriptionServiceId, Boolean validSubscriptionGroupId)
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionServiceId = validSubscriptionServiceId
                ? SubscriptionServiceConfigurationTestData.SubscriptionServiceId
                : Guid.Empty;
            Guid subscriptionGroupId = validSubscriptionGroupId
                ? SubscriptionServiceConfigurationTestData.SubscriptionGroupId
                : Guid.Empty;

            Should.Throw<ArgumentNullException>(async () =>
            {
                var result = await repository.AddSubscriptionGroupToSubscriptionService(subscriptionServiceId, subscriptionGroupId, CancellationToken.None);
            });
        }

        #endregion

        #region Add Subscription Group To Subscription Service Tests

        [Fact]
        public async Task ConfigurationRepository_RemoveSubscriptionGroupFromSubscriptionService_SubscriptionGroupRemovedFromSubscriptionService()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.RemoveSubscritionGroupFromSubscriptionServiceData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            await repository.RemoveSubscriptionGroupFromSubscriptionService(SubscriptionServiceConfigurationTestData.SubscriptionServiceId, 
                SubscriptionServiceConfigurationTestData.SubscriptionGroupId, CancellationToken.None);
            
            var verifyContext = GetContext(databaseName);
            verifyContext.SubscriptionServiceGroups.Count().ShouldBe(0);
        }

        [Theory]
        [InlineData(true,false)]
        [InlineData(false,true)]
        public void ConfigurationRepository_RemoveSubscriptionGroupFromSubscriptionService_InvalidData_ErrorThrown(Boolean validSubscriptionServiceId, Boolean validSubscriptionGroupId)
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionServiceId = validSubscriptionServiceId
                ? SubscriptionServiceConfigurationTestData.SubscriptionServiceId
                : Guid.Empty;
            Guid subscriptionGroupId = validSubscriptionGroupId
                ? SubscriptionServiceConfigurationTestData.SubscriptionGroupId
                : Guid.Empty;

            Should.Throw<ArgumentNullException>(async () =>
            {
                var result = await repository.AddSubscriptionGroupToSubscriptionService(subscriptionServiceId, subscriptionGroupId, CancellationToken.None);
            });
        }

        [Fact]
        public async Task ConfigurationRepository_RemoveSubscriptionGroupFromSubscriptionService_SubscriptionGroupNotFound_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<NotFoundException>(async () =>
            {
                await repository.RemoveSubscriptionGroupFromSubscriptionService(
                    SubscriptionServiceConfigurationTestData.SubscriptionServiceId,
                    SubscriptionServiceConfigurationTestData.SubscriptionGroupId, CancellationToken.None);
            });
        }

        #endregion

        #region Get Subscription Service Tests

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptionService_SubscriptionServiceReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.GetSubscriptionServiceData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var subscriptionService = await repository.GetSubscriptionService(SubscriptionServiceConfigurationTestData.SubscriptionServiceId, CancellationToken.None);

            subscriptionService.ShouldNotBeNull();
        }

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptionService_InvalidSubscriptionServiceId_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Should.Throw<ArgumentNullException>(async () =>
            {
                await repository.GetSubscriptionService(Guid.Empty, CancellationToken.None);
            });
        }

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptionService_SubscriptionServiceNotFound_ErrorThrown()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            Guid subscriptionGroupId = SubscriptionServiceConfigurationTestData.SubscriptionGroupId;
            Should.Throw<NotFoundException>(async () =>
            {
                await repository.GetSubscriptionService(subscriptionGroupId, CancellationToken.None);
            });
        }
        #endregion

        #region Get Subscription Groups Tests

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptionServices_SubscriptionServicesReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            context.AddTestData(TestScenario.GetSubscriptionServiceData);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var subscriptionServices = await repository.GetSubscriptionServices(CancellationToken.None);

            subscriptionServices.ShouldNotBeEmpty();
        }
        
        [Fact]
        public async Task ConfigurationRepository_GetSubscriptionServices_SubscriptionServicesNotFound_EmptyListReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);

            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var subscriptionServices = await repository.GetSubscriptionServices(CancellationToken.None);

            subscriptionServices.ShouldBeEmpty();
        }

        #endregion
    }

    public static class ContextExtensions
    {       
        public static void AddTestData(this SubscriptionServiceConfigurationContext context, TestScenario testScenario)
        {
            if (testScenario == TestScenario.GetSubscriptionsData)
            {
                Database.Models.SubscriptionService service = new Database.Models.SubscriptionService
                {
                    SubscriptionServiceId = SubscriptionServiceConfigurationTestData.SubscriptionServiceId,
                    Description = SubscriptionServiceConfigurationTestData.SubscriptionServiceDescription
                };

                SubscriptionServiceGroup subscriptionServiceGroup = new SubscriptionServiceGroup
                {
                    SubscriptionServiceGroupId = SubscriptionServiceConfigurationTestData.SubscriptionServiceGroupId,
                    SubscriptionService = new Database.Models.SubscriptionService
                    {
                        SubscriptionServiceId = SubscriptionServiceConfigurationTestData.SubscriptionServiceId,
                        Description = SubscriptionServiceConfigurationTestData.SubscriptionServiceDescription
                    },
                    SubscriptionGroup = new SubscriptionGroup
                    {
                        Id = SubscriptionServiceConfigurationTestData.SubscriptionGroupId,
                        Name = SubscriptionServiceConfigurationTestData.SubscriptionGroupName,
                        SubscriptionStream = new SubscriptionStream
                        {
                            Id = SubscriptionServiceConfigurationTestData.SubscriptionStreamId,
                            StreamName = SubscriptionServiceConfigurationTestData.SubscriptionStreamName,
                            SubscriptionType = (Int32)SubscriptionType.Persistent
                        },
                        EndPoint = new EndPoint
                        {
                            Name = SubscriptionServiceConfigurationTestData.EndPointName,
                            EndPointId = SubscriptionServiceConfigurationTestData.EndPointId,
                            Url = SubscriptionServiceConfigurationTestData.EndPointUrl
                        }
                    }
                };

                context.SubscriptionServices.Add(service);
                context.SubscriptionServiceGroups.Add(subscriptionServiceGroup);

            }
            else if (testScenario == TestScenario.ResetSubscriptionStreamPositionData)
            {
                SubscriptionGroup subscriptionGroup = new SubscriptionGroup
                {
                    Id = SubscriptionServiceConfigurationTestData.SubscriptionGroupId,
                    Name = SubscriptionServiceConfigurationTestData.SubscriptionGroupName,
                    SubscriptionStream = new SubscriptionStream
                    {
                        Id = SubscriptionServiceConfigurationTestData.SubscriptionStreamId,
                        StreamName = SubscriptionServiceConfigurationTestData.SubscriptionStreamName,
                        SubscriptionType = (Int32)SubscriptionType.Persistent
                    },
                    EndPoint = new EndPoint
                    {
                        Name = SubscriptionServiceConfigurationTestData.EndPointName,
                        EndPointId = SubscriptionServiceConfigurationTestData.EndPointId,
                        Url = SubscriptionServiceConfigurationTestData.EndPointUrl
                    }
                };

                context.SubscriptionGroups.Add(subscriptionGroup);
            }
            else if (testScenario == TestScenario.GetEndPointForSubscriptionGroupData)
            {
                SubscriptionGroup subscriptionGroup = new SubscriptionGroup
                {
                    Id = SubscriptionServiceConfigurationTestData.SubscriptionGroupId,
                    Name = SubscriptionServiceConfigurationTestData.SubscriptionGroupName,
                    SubscriptionStream = new SubscriptionStream
                    {
                        Id = SubscriptionServiceConfigurationTestData.SubscriptionStreamId,
                        StreamName = SubscriptionServiceConfigurationTestData.SubscriptionStreamName,
                        SubscriptionType = (Int32)SubscriptionType.Persistent
                    },
                    EndPoint = new EndPoint
                    {
                        Name = SubscriptionServiceConfigurationTestData.EndPointName,
                        EndPointId = SubscriptionServiceConfigurationTestData.EndPointId,
                        Url = SubscriptionServiceConfigurationTestData.EndPointUrl
                    }
                };

                context.SubscriptionGroups.Add(subscriptionGroup);
            }
            else if (testScenario == TestScenario.GetEndPointForCatchUpSubscriptionData)
            {
                CatchUpSubscription catchUpSubscription = new CatchUpSubscription
                {
                    CreateDateTime = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionCreatDateTime,
                    EndPoint = new EndPoint
                    {
                        EndPointId = SubscriptionServiceConfigurationTestData.EndPointId,
                        Name = SubscriptionServiceConfigurationTestData.EndPointName,
                        Url = SubscriptionServiceConfigurationTestData.EndPointUrl
                    },
                    Name = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionName,
                    Id = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionId,
                    Position = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionPosition,
                    StreamName = SubscriptionServiceConfigurationTestData.SubscriptionStreamName
                };

                context.CatchUpSubscriptions.Add(catchUpSubscription);
            }
            else if (testScenario == TestScenario.GetNextCatchupSubscriptionData)
            {
                CatchUpSubscription catchUpSubscription = new CatchUpSubscription
                {
                    CreateDateTime = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionCreatDateTime,
                    EndPoint = new EndPoint
                    {
                        EndPointId = SubscriptionServiceConfigurationTestData.EndPointId,
                        Name = SubscriptionServiceConfigurationTestData.EndPointName,
                        Url = SubscriptionServiceConfigurationTestData.EndPointUrl
                    },
                    Name = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionName,
                    Id = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionId,
                    Position = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionPosition,
                    StreamName = SubscriptionServiceConfigurationTestData.SubscriptionStreamName
                };

                context.CatchUpSubscriptions.Add(catchUpSubscription);
            }
            else if (testScenario == TestScenario.DeleteCatchUpSubscriptionData)
            {
                CatchUpSubscription catchUpSubscription = new CatchUpSubscription
                {
                    CreateDateTime = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionCreatDateTime,
                    EndPoint = new EndPoint
                    {
                        EndPointId = SubscriptionServiceConfigurationTestData.EndPointId,
                        Name = SubscriptionServiceConfigurationTestData.EndPointName,
                        Url = SubscriptionServiceConfigurationTestData.EndPointUrl
                    },
                    Name = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionName,
                    Id = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionId,
                    Position = SubscriptionServiceConfigurationTestData.CatchUpSubscriptionPosition,
                    StreamName = SubscriptionServiceConfigurationTestData.SubscriptionStreamName
                };

                context.CatchUpSubscriptions.Add(catchUpSubscription);
            }
            else if (testScenario == TestScenario.CreateSubscriptionStreamData)
            {
                SubscriptionStream subscriptionStream = new SubscriptionStream
                {
                    StreamName = SubscriptionServiceConfigurationTestData.SubscriptionStreamName,
                    Id = SubscriptionServiceConfigurationTestData.SubscriptionStreamId,
                    SubscriptionType = (Int32)SubscriptionType.Persistent
                };

                context.SubscriptionStream.Add(subscriptionStream);
            }
            else if (testScenario == TestScenario.GetSubscriptionStreamsData)
            {
                SubscriptionStream subscriptionStream1 = new SubscriptionStream
                {
                    StreamName = SubscriptionServiceConfigurationTestData.SubscriptionStream1Name,
                    Id = SubscriptionServiceConfigurationTestData.SubscriptionStream1Id,
                    SubscriptionType = (Int32)SubscriptionType.Persistent
                };

                SubscriptionStream subscriptionStream2 = new SubscriptionStream
                {
                    StreamName = SubscriptionServiceConfigurationTestData.SubscriptionStream2Name,
                    Id = SubscriptionServiceConfigurationTestData.SubscriptionStream2Id,
                    SubscriptionType = (Int32)SubscriptionType.Persistent
                };

                context.SubscriptionStream.Add(subscriptionStream1);
                context.SubscriptionStream.Add(subscriptionStream2);
            }
            else if (testScenario == TestScenario.CreateSubscriptionGroupData)
            {
                SubscriptionGroup subscriptionGroup = new SubscriptionGroup
                {
                    Id = SubscriptionServiceConfigurationTestData.SubscriptionGroupId,
                    Name = SubscriptionServiceConfigurationTestData.SubscriptionGroupName,
                    SubscriptionStreamId = SubscriptionServiceConfigurationTestData.SubscriptionStreamId,
                    EndPointId = SubscriptionServiceConfigurationTestData.EndPointId
                };

                context.SubscriptionGroups.Add(subscriptionGroup);
            }
            else if (testScenario == TestScenario.GetSubscriptionGroupData)
            {
                SubscriptionGroup subscriptionGroup = new SubscriptionGroup
                {
                    Id = SubscriptionServiceConfigurationTestData.SubscriptionGroupId,
                    Name = SubscriptionServiceConfigurationTestData.SubscriptionGroupName,
                    SubscriptionStream = new SubscriptionStream
                    {
                        Id = SubscriptionServiceConfigurationTestData.SubscriptionStreamId,
                        StreamName = SubscriptionServiceConfigurationTestData.SubscriptionStreamName,
                        SubscriptionType = (Int32)SubscriptionType.Persistent
                    },
                    EndPoint = new EndPoint
                    {
                        Name = SubscriptionServiceConfigurationTestData.EndPointName,
                        EndPointId = SubscriptionServiceConfigurationTestData.EndPointId,
                        Url = SubscriptionServiceConfigurationTestData.EndPointUrl
                    }
                };

                context.SubscriptionGroups.Add(subscriptionGroup);
            }
            else if (testScenario == TestScenario.RemoveSubscriptionGroupData)
            {
                SubscriptionGroup subscriptionGroup = new SubscriptionGroup
                {
                    Id = SubscriptionServiceConfigurationTestData.SubscriptionGroupId,
                    Name = SubscriptionServiceConfigurationTestData.SubscriptionGroupName,
                    SubscriptionStream = new SubscriptionStream
                    {
                        Id = SubscriptionServiceConfigurationTestData.SubscriptionStreamId,
                        StreamName = SubscriptionServiceConfigurationTestData.SubscriptionStreamName,
                        SubscriptionType = (Int32)SubscriptionType.Persistent
                    },
                    EndPoint = new EndPoint
                    {
                        Name = SubscriptionServiceConfigurationTestData.EndPointName,
                        EndPointId = SubscriptionServiceConfigurationTestData.EndPointId,
                        Url = SubscriptionServiceConfigurationTestData.EndPointUrl
                    }
                };

                context.SubscriptionGroups.Add(subscriptionGroup);
            }
            else if (testScenario == TestScenario.GetSubscriptionServiceData)
            {
                Database.Models.SubscriptionService service = new Database.Models.SubscriptionService
                {
                    SubscriptionServiceId = SubscriptionServiceConfigurationTestData.SubscriptionServiceId,
                    Description = SubscriptionServiceConfigurationTestData.SubscriptionServiceDescription
                };

                context.SubscriptionServices.Add(service);
            }
            else if (testScenario == TestScenario.RemoveSubscritionGroupFromSubscriptionServiceData)
            {
                Database.Models.SubscriptionService service = new Database.Models.SubscriptionService
                {
                    SubscriptionServiceId = SubscriptionServiceConfigurationTestData.SubscriptionServiceId,
                    Description = SubscriptionServiceConfigurationTestData.SubscriptionServiceDescription
                };

                SubscriptionServiceGroup subscriptionServiceGroup = new SubscriptionServiceGroup
                {
                    SubscriptionServiceGroupId = SubscriptionServiceConfigurationTestData.SubscriptionServiceGroupId,
                    SubscriptionService = new Database.Models.SubscriptionService
                    {
                        SubscriptionServiceId = SubscriptionServiceConfigurationTestData.SubscriptionServiceId,
                        Description = SubscriptionServiceConfigurationTestData.SubscriptionServiceDescription
                    },
                    SubscriptionGroup = new SubscriptionGroup
                    {
                        Id = SubscriptionServiceConfigurationTestData.SubscriptionGroupId,
                        Name = SubscriptionServiceConfigurationTestData.SubscriptionGroupName,
                        SubscriptionStream = new SubscriptionStream
                        {
                            Id = SubscriptionServiceConfigurationTestData.SubscriptionStreamId,
                            StreamName = SubscriptionServiceConfigurationTestData.SubscriptionStreamName,
                            SubscriptionType = (Int32)SubscriptionType.Persistent
                        },
                        EndPoint = new EndPoint
                        {
                            Name = SubscriptionServiceConfigurationTestData.EndPointName,
                            EndPointId = SubscriptionServiceConfigurationTestData.EndPointId,
                            Url = SubscriptionServiceConfigurationTestData.EndPointUrl
                        }
                    }
                };

                context.SubscriptionServices.Add(service);
                context.SubscriptionServiceGroups.Add(subscriptionServiceGroup);
            }
            
            context.SaveChanges();
        }        
    }

    public enum TestScenario
    {
        NoData = 0,
        GetSubscriptionsData,
        ResetSubscriptionStreamPositionData,
        GetEndPointForSubscriptionGroupData,
        GetEndPointForCatchUpSubscriptionData,
        GetNextCatchupSubscriptionData,
        DeleteCatchUpSubscriptionData,
        CreateSubscriptionStreamData,
        GetSubscriptionStreamsData,
        CreateSubscriptionGroupData,
        GetSubscriptionGroupData,
        RemoveSubscriptionGroupData,
        GetSubscriptionServiceData,
        RemoveSubscritionGroupFromSubscriptionServiceData
    }
}
