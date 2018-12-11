using System;
using Microsoft.Extensions.Options;
using Moq;
using Shared.EventStore;
using Shouldly;
using SubscriptionService.BusinessLogic.EventStore;
using SubscriptionService.BusinessLogic.Repository;
using Xunit;

namespace SubscriptionService.UnitTests
{
    public class EventStoreManagerTests
    {
        [Fact]
        public void EventStoreManager_CanBeCreated_IsCreated()
        {
            Mock<Func<String, IEventStoreContext>> contextResolver = new Mock<Func<String, IEventStoreContext>>();
            Mock<Func<IConfigurationRepository>> configurationRepositoryResolver = new Mock<Func<IConfigurationRepository>>();
            Mock<IOptions<EventStoreSettings>> eventStoreSettings = new Mock<IOptions<EventStoreSettings>>();

            EventStoreManager manager = new EventStoreManager(contextResolver.Object, configurationRepositoryResolver.Object, eventStoreSettings.Object);

            manager.ShouldNotBeNull();
        }

        [Fact]
        public void EventStoreManager_GetEventStoreContext_ContextReturned()
        {
            Mock<IEventStoreContext> eventStoreContext = new Mock<IEventStoreContext>();

            Func<String, IEventStoreContext> contextResolver = (connectionString) => eventStoreContext.Object;
            
            Mock<Func<IConfigurationRepository>> configurationRepositoryResolver = new Mock<Func<IConfigurationRepository>>();
            Mock<IOptions<EventStoreSettings>> eventStoreSettings = new Mock<IOptions<EventStoreSettings>>();

            // Mocked event handlers
            Mock<EventAppearedEventHandler> eventAppearedEventHandler = new Mock<EventAppearedEventHandler>();
            Mock<SubscriptionDroppedEventHandler> subscriptionDroppedEventHandler = new Mock<SubscriptionDroppedEventHandler>();
            Mock<LiveProcessStartedEventHandler> liveProcessStartedEventHandler = new Mock<LiveProcessStartedEventHandler>();

            EventStoreManager manager = new EventStoreManager(contextResolver, configurationRepositoryResolver.Object, eventStoreSettings.Object);

            var context = manager.GetEventStoreContext(eventAppearedEventHandler.Object, subscriptionDroppedEventHandler.Object,
                liveProcessStartedEventHandler.Object);

            context.ShouldNotBeNull();
        }
    }
}
