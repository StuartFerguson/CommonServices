using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shared.EventStore;
using Shared.General;
using Shouldly;
using SubscriptionService.BusinessLogic.EventStore;
using SubscriptionService.BusinessLogic.Repository;
using SubscriptionService.BusinessLogic.Subscription;
using Xunit;

namespace SubscriptionService.UnitTests
{
    public class SubscriptionTests
    {
        public SubscriptionTests()
        {
            Logger.Initialise(NullLogger.Instance);
        }

        [Fact]
        public void Subscription_CanBeCreated_IsCreated()
        {
            Mock<IEventStoreManager> eventStoreManager = new Mock<IEventStoreManager>();
            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            Func<IConfigurationRepository> configurationRepositoryResolver = () =>
            {
                return configurationRepository.Object;
            };

            Subscription subscription = new Subscription(eventStoreManager.Object, configurationRepositoryResolver);

            subscription.ShouldNotBeNull();
            subscription.GroupName.ShouldBeNullOrEmpty();
            subscription.StartPosition.ShouldBeNull();
            subscription.StreamName.ShouldBeNullOrEmpty();
            subscription.SubscriptionId.ShouldBe(Guid.Empty);
            subscription.Status.ShouldBe(SubscriptionStatus.NotSet);
        }

        [Fact]
        public async Task Subscription_StartSubscription_NoStreamPosition_IsStarted()
        {
            Mock<IEventStoreManager> eventStoreManager = new Mock<IEventStoreManager>();
            Mock<IEventStoreContext> eventStoreContext = new Mock<IEventStoreContext>();
            
            eventStoreManager
                .Setup(m => m.GetEventStoreContext(It.IsAny<EventAppearedEventHandler>(),
                    It.IsAny<SubscriptionDroppedEventHandler>(), It.IsAny<LiveProcessStartedEventHandler>()))
                .ReturnsAsync(eventStoreContext.Object);
            
            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            Func<IConfigurationRepository> configurationRepositoryResolver = () =>
            {
                return configurationRepository.Object;
            };

            Subscription subscription = new Subscription(eventStoreManager.Object, configurationRepositoryResolver);
            
            await subscription.StartSubscription(SubscriptionTestData.SubscriptionId, SubscriptionTestData.StreamName,
                SubscriptionTestData.GroupName);

            subscription.GroupName.ShouldBe(SubscriptionTestData.GroupName);
            subscription.StartPosition.ShouldBeNull();
            subscription.StreamName.ShouldBe(SubscriptionTestData.StreamName);
            subscription.SubscriptionId.ShouldBe(SubscriptionTestData.SubscriptionId);
            subscription.Status.ShouldBe(SubscriptionStatus.Started);
            eventStoreContext.Verify(c => c.ConnectToSubscription(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Int32>()));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Subscription_StartSubscription_WithStreamPosition_IsStarted(Int32? streamPosition)
        {
            Mock<IEventStoreManager> eventStoreManager = new Mock<IEventStoreManager>();
            Mock<IEventStoreContext> eventStoreContext = new Mock<IEventStoreContext>();
            
            eventStoreManager
                .Setup(m => m.GetEventStoreContext(It.IsAny<EventAppearedEventHandler>(),
                    It.IsAny<SubscriptionDroppedEventHandler>(), It.IsAny<LiveProcessStartedEventHandler>()))
                .ReturnsAsync(eventStoreContext.Object);
            
            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            Func<IConfigurationRepository> configurationRepositoryResolver = () =>
            {
                return configurationRepository.Object;
            };

            Subscription subscription = new Subscription(eventStoreManager.Object, configurationRepositoryResolver);

            
            await subscription.StartSubscription(SubscriptionTestData.SubscriptionId, SubscriptionTestData.StreamName,
                SubscriptionTestData.GroupName, streamPosition);

            subscription.GroupName.ShouldBe(SubscriptionTestData.GroupName);
            subscription.StartPosition.ShouldBe(streamPosition);
            subscription.StreamName.ShouldBe(SubscriptionTestData.StreamName);
            subscription.SubscriptionId.ShouldBe(SubscriptionTestData.SubscriptionId);
            subscription.Status.ShouldBe(SubscriptionStatus.Started);
            eventStoreContext.Verify(c => c.ConnectToSubscription(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Int32>()));
        }

        [Fact]
        public void Subscription_StartSubscription_ConnectToSubscriptionThrowsException_ErrorThrown()
        {
            Mock<IEventStoreManager> eventStoreManager = new Mock<IEventStoreManager>();
            Mock<IEventStoreContext> eventStoreContext = new Mock<IEventStoreContext>();
            
            eventStoreManager
                .Setup(m => m.GetEventStoreContext(It.IsAny<EventAppearedEventHandler>(),
                    It.IsAny<SubscriptionDroppedEventHandler>(), It.IsAny<LiveProcessStartedEventHandler>()))
                .ReturnsAsync(eventStoreContext.Object);

            eventStoreContext.Setup(c =>
                c.ConnectToSubscription(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Int32>())).Throws<Exception>();

            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            Func<IConfigurationRepository> configurationRepositoryResolver = () =>
            {
                return configurationRepository.Object;
            };

            Subscription subscription = new Subscription(eventStoreManager.Object, configurationRepositoryResolver);

            Should.Throw<Exception>(async () =>
            {
                await subscription.StartSubscription(SubscriptionTestData.SubscriptionId, SubscriptionTestData.StreamName, SubscriptionTestData.GroupName);
            });
        }

        [Theory]
        [InlineData(false, "StreamName", "GroupName")]
        [InlineData(true, null, "GroupName")]
        [InlineData(true, "", "GroupName")]
        [InlineData(true, "StreamName", null)]
        [InlineData(true, "StreamName", "")]
        public void Subscription_StartSubscription_InvalidData_ErrorThrown(Boolean validSubscriptionId, String streamName, String groupName)
        {
            Mock<IEventStoreManager> eventStoreManager = new Mock<IEventStoreManager>();
            Mock<IEventStoreContext> eventStoreContext = new Mock<IEventStoreContext>();
            
            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            Func<IConfigurationRepository> configurationRepositoryResolver = () =>
            {
                return configurationRepository.Object;
            };

            Subscription subscription = new Subscription(eventStoreManager.Object, configurationRepositoryResolver);

            Guid subscriptionId = validSubscriptionId ? SubscriptionTestData.SubscriptionId : Guid.Empty;

            Should.Throw<ArgumentNullException>(async () =>
            {
                await subscription.StartSubscription(subscriptionId,streamName, groupName);
            });
        }

        [Fact]
        public async Task Subscription_StopSubscription_IsStopped()
        {
            Mock<IEventStoreManager> eventStoreManager = new Mock<IEventStoreManager>();
            Mock<IEventStoreContext> eventStoreContext = new Mock<IEventStoreContext>();
            
            eventStoreManager
                .Setup(m => m.GetEventStoreContext(It.IsAny<EventAppearedEventHandler>(),
                    It.IsAny<SubscriptionDroppedEventHandler>(), It.IsAny<LiveProcessStartedEventHandler>()))
                .ReturnsAsync(eventStoreContext.Object);
            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();
            Func<IConfigurationRepository> configurationRepositoryResolver = () =>
            {
                return configurationRepository.Object;
            };

            Subscription subscription = new Subscription(eventStoreManager.Object, configurationRepositoryResolver);

            await subscription.StopSubscription();

            subscription.Status.ShouldBe(SubscriptionStatus.Stopped);
            eventStoreContext.Verify(c => c.DeletePersistentSubscription(It.IsAny<String>(), It.IsAny<String>()));
        }        
    }
}
