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

namespace SubscriptionService.UnitTests
{
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

            var subscriptionGroups = await repository.GetSubscriptions(CancellationToken.None);

            subscriptionGroups.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task ConfigurationRepository_GetSubscriptions_NoSubscriptions_EmptyListReturned()
        {
            String databaseName = Guid.NewGuid().ToString("N");
            var context = GetContext(databaseName);
            
            Func<SubscriptionServiceConfigurationContext> contextResolver = () => { return context; };
            
            ConfigurationRepository repository = new ConfigurationRepository(contextResolver);

            var subscriptionGroups = await repository.GetSubscriptions(CancellationToken.None);

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

            Guid subscriptionId = Guid.Parse("317E17B5-8E98-48A4-BFD3-9C0EE2919EAC");
            await repository.ResetSubscriptionStreamPosition(subscriptionId, CancellationToken.None);

            var verifyContext = GetContext(databaseName);
            verifyContext.SubscriptionGroups.First().StreamPosition.ShouldBeNull();
        }

        [Fact]
        public void ConfigurationRepository_ResetSubscriptionStreamPosition_InvalidSubscriptionId_ErrorThrown()
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
                Guid subscriptionId = Guid.Parse("317E17B5-8E98-48A4-BFD3-9C0EE2919EAC");
                await repository.ResetSubscriptionStreamPosition(subscriptionId, CancellationToken.None);
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

            Guid subscriptionId = Guid.Parse("317E17B5-8E98-48A4-BFD3-9C0EE2919EAC");
            var endPoint = await repository.GetEndPointForSubscriptionGroup(subscriptionId, CancellationToken.None);

            endPoint.ShouldNotBeNull();
        }

        [Fact]
        public async Task ConfigurationRepository_GetEndPointForSubscriptionGroup_InvalidSubscriptionId_ErrorThrown()
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

            Guid subscriptionId = Guid.Parse("317E17B5-8E98-48A4-BFD3-9C0EE2919EAC");
            Should.Throw<NotFoundException>(async () =>
            {
                await repository.GetEndPointForSubscriptionGroup(subscriptionId, CancellationToken.None);
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

            Guid subscriptionId = Guid.Parse("D1AA40C2-040B-4AF6-9864-8EC0A2E20BCF");
            var endPoint = await repository.GetEndPointForCatchUpSubscription(subscriptionId, CancellationToken.None);

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
                Guid subscriptionId = Guid.Parse("D1AA40C2-040B-4AF6-9864-8EC0A2E20BCF");
                await repository.GetEndPointForCatchUpSubscription(subscriptionId, CancellationToken.None);
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

            Guid subscriptionId = Guid.Parse("D1AA40C2-040B-4AF6-9864-8EC0A2E20BCF");
            await repository.DeleteCatchUpSubscription(subscriptionId, CancellationToken.None);

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
                Guid subscriptionId = Guid.Parse("D1AA40C2-040B-4AF6-9864-8EC0A2E20BCF");
                await repository.DeleteCatchUpSubscription(subscriptionId, CancellationToken.None);
            });            
        }

        #endregion
    }

    public static class ContextExtensions
    {
        public static void AddTestData(this SubscriptionServiceConfigurationContext context, TestScenario testScenario)
        {
            if (testScenario == TestScenario.GetSubscriptionsData)
            {
                SubscriptionGroup subscriptionGroup = new SubscriptionGroup
                {
                    BufferSize = null,
                    Id  = Guid.Parse("317E17B5-8E98-48A4-BFD3-9C0EE2919EAC"),
                    EndPointId = Guid.Parse("9F791A4A-56C1-47D0-ABE8-F3DCF3253066"),
                    Name = "TestGroup1",
                    StreamPosition = null,
                    SubscriptionStream = new SubscriptionStream
                    {
                        Id = Guid.Parse("DC390376-71BA-426B-943C-109727906C5B"),
                        StreamName = "TestStream",
                        SubscriptionType = 0
                    }                    
                };

                context.SubscriptionGroups.Add(subscriptionGroup);
            }
            else if (testScenario == TestScenario.ResetSubscriptionStreamPositionData)
            {
                SubscriptionGroup subscriptionGroup = new SubscriptionGroup
                {
                    BufferSize = null,
                    Id  = Guid.Parse("317E17B5-8E98-48A4-BFD3-9C0EE2919EAC"),
                    EndPointId = Guid.Parse("9F791A4A-56C1-47D0-ABE8-F3DCF3253066"),
                    Name = "TestGroup1",
                    StreamPosition = 100,
                    SubscriptionStream = new SubscriptionStream
                    {
                        Id = Guid.Parse("DC390376-71BA-426B-943C-109727906C5B"),
                        StreamName = "TestStream",
                        SubscriptionType = 0
                    }                    
                };

                context.SubscriptionGroups.Add(subscriptionGroup);
            }
            else if (testScenario == TestScenario.GetEndPointForSubscriptionGroupData)
            {
                SubscriptionGroup subscriptionGroup = new SubscriptionGroup
                {
                    BufferSize = null,
                    Id  = Guid.Parse("317E17B5-8E98-48A4-BFD3-9C0EE2919EAC"),
                    EndPointId = Guid.Parse("9F791A4A-56C1-47D0-ABE8-F3DCF3253066"),
                    Name = "TestGroup1",
                    StreamPosition = 100,
                    SubscriptionStream = new SubscriptionStream
                    {
                        Id = Guid.Parse("DC390376-71BA-426B-943C-109727906C5B"),
                        StreamName = "TestStream",
                        SubscriptionType = 0
                    } ,
                    EndPoint = new EndPoint
                    {
                        EndPointId = Guid.Parse("5A13083E-61BD-492A-93EC-4B59DAF45B06"),
                        Name = "TestEndPoint1",
                        Url = "http://localhost:5000"
                    }
                };
                context.SubscriptionGroups.Add(subscriptionGroup);
            }
            else if (testScenario == TestScenario.GetEndPointForCatchUpSubscriptionData)
            {
                CatchUpSubscription catchUpSubscription = new CatchUpSubscription
                {
                    CreateDateTime = DateTime.Now,
                    EndPoint = new EndPoint
                    {
                        EndPointId = Guid.Parse("42FB7F28-82B7-43E1-8D7D-32E073B9DE1F"),
                        Name = "TestCatchUpEndPoint",
                        Url = "http://localhost:5001"
                    },
                    Name = "TestCatchup",
                    Id = Guid.Parse("D1AA40C2-040B-4AF6-9864-8EC0A2E20BCF"),
                    Position = 0,
                    StreamName = "TestStream"
                };

                context.CatchUpSubscriptions.Add(catchUpSubscription);
            }
            else if (testScenario == TestScenario.GetNextCatchupSubscriptionData)
            {
                CatchUpSubscription catchUpSubscription = new CatchUpSubscription
                {
                    CreateDateTime = DateTime.Now,
                    EndPoint = new EndPoint
                    {
                        EndPointId = Guid.Parse("42FB7F28-82B7-43E1-8D7D-32E073B9DE1F"),
                        Name = "TestCatchUpEndPoint",
                        Url = "http://localhost:5001"
                    },
                    Name = "TestCatchup",
                    Id = Guid.Parse("D1AA40C2-040B-4AF6-9864-8EC0A2E20BCF"),
                    Position = 0,
                    StreamName = "TestStream"
                };

                context.CatchUpSubscriptions.Add(catchUpSubscription);
            }
            else if (testScenario == TestScenario.DeleteCatchUpSubscriptionData)
            {
                CatchUpSubscription catchUpSubscription = new CatchUpSubscription
                {
                    CreateDateTime = DateTime.Now,
                    EndPoint = new EndPoint
                    {
                        EndPointId = Guid.Parse("42FB7F28-82B7-43E1-8D7D-32E073B9DE1F"),
                        Name = "TestCatchUpEndPoint",
                        Url = "http://localhost:5001"
                    },
                    Name = "TestCatchup",
                    Id = Guid.Parse("D1AA40C2-040B-4AF6-9864-8EC0A2E20BCF"),
                    Position = 0,
                    StreamName = "TestStream"
                };

                context.CatchUpSubscriptions.Add(catchUpSubscription);
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
        DeleteCatchUpSubscriptionData
    }
}
