using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PaymentCentric.Common;
using PaymentCentric.Domain;
using Shouldly;
using SubscriptionService.BusinessLogic.EventStoreContext;
using Xunit;

namespace SubscriptionService.UnitTests
{
    public class EventStoreContextTests
    {
        [Fact]
        public void EventStoreContext_CanBeCreated_IsCreated()
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };
            
            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);

            context.ShouldNotBeNull();
        }

        [Fact]
        public void EventStoreContext_CanBeCreated_NullConnectionSettings_ErrorThrown()
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = null;
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            Should.Throw<ArgumentNullException>(() =>
            {
                EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);
            });           
        }

        #region Connect To Subscription Tests

        [Fact]
        public async Task EventStoreContext_ConnectToSubscription_NewSubscription_ConnectionMade()
        {
            EventStorePersistentSubscription result = null;

            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            connection.SetupSequence(c => c.ConnectToPersistentSubscriptionAsync(It.IsAny<String>(), It.IsAny<String>(),
                    It.IsAny<Func<EventStorePersistentSubscriptionBase, ResolvedEvent, Int32?, Task>>(),
                    It.IsAny<Action<EventStorePersistentSubscriptionBase, SubscriptionDropReason, Exception>>(),
                    It.IsAny<UserCredentials>(), It.IsAny<Int32>(), It.IsAny<Boolean>()))
                .Throws(new Exception("Error", new Exception("Subscription not found")))
                .ReturnsAsync(result);
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);
            
            await context.ConnectToSubscription(EventStoreContextTestData.StreamName, EventStoreContextTestData.GroupName,
                EventStoreContextTestData.PersistentSubscriptionId,
                EventStoreContextTestData.BufferSize);

            connection.Verify(x => x.ConnectToPersistentSubscriptionAsync(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<Func<EventStorePersistentSubscriptionBase, ResolvedEvent, Int32?, Task>>(), 
                It.IsAny<Action<EventStorePersistentSubscriptionBase, SubscriptionDropReason, Exception>>(), It.IsAny<UserCredentials>(), It.IsAny<Int32>(), It.IsAny<Boolean>()), Times.Exactly(2));
            connection.Verify(x => x.CreatePersistentSubscriptionAsync(EventStoreContextTestData.StreamName, EventStoreContextTestData.GroupName, It.IsAny<PersistentSubscriptionSettings>(), It.IsAny<UserCredentials>()));
        }

        [Fact]
        public async Task EventStoreContext_ConnectToSubscription_ExisitngSubscription_ConnectionMade()
        {
            EventStorePersistentSubscription result = null;

            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            connection.Setup(c => c.ConnectToPersistentSubscriptionAsync(It.IsAny<String>(), It.IsAny<String>(),
                    It.IsAny<Func<EventStorePersistentSubscriptionBase, ResolvedEvent, Int32?, Task>>(),
                    It.IsAny<Action<EventStorePersistentSubscriptionBase, SubscriptionDropReason, Exception>>(),
                    It.IsAny<UserCredentials>(), It.IsAny<Int32>(), It.IsAny<Boolean>()))
                .ReturnsAsync(result);
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);
            
            await context.ConnectToSubscription(EventStoreContextTestData.StreamName, EventStoreContextTestData.GroupName,
                EventStoreContextTestData.PersistentSubscriptionId,
                EventStoreContextTestData.BufferSize);

            connection.Verify(x => x.ConnectToPersistentSubscriptionAsync(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<Func<EventStorePersistentSubscriptionBase, ResolvedEvent, Int32?, Task>>(), 
                It.IsAny<Action<EventStorePersistentSubscriptionBase, SubscriptionDropReason, Exception>>(), It.IsAny<UserCredentials>(), It.IsAny<Int32>(), It.IsAny<Boolean>()), Times.Exactly(1));
        }

        [Theory]
        [InlineData(null, "groupName", true, typeof(ArgumentNullException))]
        [InlineData("", "groupName", true, typeof(ArgumentNullException))]
        [InlineData("streamName", null, true, typeof(ArgumentNullException))]
        [InlineData("streamName", "", true, typeof(ArgumentNullException))]
        [InlineData("streamName", "groupName", false, typeof(ArgumentNullException))]
        public void EventStoreContext_ConnectToSubscription_ConnectException_InvalidData_ErrorThrown(String streamName, String groupName, Boolean validSubscriptionGroupId, Type exceptionType)
        {
            EventStorePersistentSubscription result = null;

            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);

            Should.Throw<Exception>(async () =>
            {
                await context.ConnectToSubscription(streamName, groupName, validSubscriptionGroupId ? EventStoreContextTestData.PersistentSubscriptionId : Guid.Empty,
                    EventStoreContextTestData.BufferSize);
            });
        }

        [Fact]
        public void EventStoreContext_ConnectToSubscription_ConnectException_ErrorThrown()
        {
            EventStorePersistentSubscription result = null;

            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            connection.Setup(c => c.ConnectToPersistentSubscriptionAsync(It.IsAny<String>(), It.IsAny<String>(),
                    It.IsAny<Func<EventStorePersistentSubscriptionBase, ResolvedEvent, Int32?, Task>>(),
                    It.IsAny<Action<EventStorePersistentSubscriptionBase, SubscriptionDropReason, Exception>>(),
                    It.IsAny<UserCredentials>(), It.IsAny<Int32>(), It.IsAny<Boolean>()))
                .Throws(new Exception("Error", new Exception("Some nasty error")));

            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);

            Should.Throw<Exception>(async () =>
            {
                await context.ConnectToSubscription(EventStoreContextTestData.StreamName,
                    EventStoreContextTestData.GroupName,
                    EventStoreContextTestData.PersistentSubscriptionId,
                    EventStoreContextTestData.BufferSize);
            });
        }

        #endregion

        #region Create Persisitent Subscription Tests

        [Fact]
        public async Task EventStoreContext_CreateNewPersistentSubscription_SubscriptionCreated()
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);

            await context.CreateNewPersistentSubscription(EventStoreContextTestData.StreamName,
                EventStoreContextTestData.GroupName);

            connection.Verify(x => x.CreatePersistentSubscriptionAsync(EventStoreContextTestData.StreamName, EventStoreContextTestData.GroupName, It.IsAny<PersistentSubscriptionSettings>(), It.IsAny<UserCredentials>()));
        }

        [Fact]
        public async Task EventStoreContext_CreatePersistentSubscriptionFromBeginning_SubscriptionCreated()
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);

            await context.CreatePersistentSubscriptionFromBeginning(EventStoreContextTestData.StreamName,
                EventStoreContextTestData.GroupName);

            connection.Verify(x => x.CreatePersistentSubscriptionAsync(EventStoreContextTestData.StreamName, EventStoreContextTestData.GroupName, It.IsAny<PersistentSubscriptionSettings>(), It.IsAny<UserCredentials>()));
        }

        [Theory]
        [InlineData("", "groupName", typeof(ArgumentNullException))]
        [InlineData(null, "groupName", typeof(ArgumentNullException))]
        [InlineData("streamName", "", typeof(ArgumentNullException))]
        [InlineData("streamName", null, typeof(ArgumentNullException))]
        public async Task EventStoreContext_CreatePersistentSubscriptionFromBeginning_InvalidData_ErrorThrown(String streamName, String groupName, Type exceptionType)
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);

            Should.Throw(async () => { await context.CreatePersistentSubscriptionFromBeginning(streamName, groupName); }, exceptionType);
        }

        [Fact]
        public async Task EventStoreContext_CreatePersistentSubscriptionFromCurrent_SubscriptionCreated()
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);

            await context.CreatePersistentSubscriptionFromCurrent(EventStoreContextTestData.StreamName,
                EventStoreContextTestData.GroupName);

            connection.Verify(x => x.CreatePersistentSubscriptionAsync(EventStoreContextTestData.StreamName, EventStoreContextTestData.GroupName, It.IsAny<PersistentSubscriptionSettings>(), It.IsAny<UserCredentials>()));
        }

        [Theory]
        [InlineData("", "groupName", typeof(ArgumentNullException))]
        [InlineData(null, "groupName", typeof(ArgumentNullException))]
        [InlineData("streamName", "", typeof(ArgumentNullException))]
        [InlineData("streamName", null, typeof(ArgumentNullException))]
        public async Task EventStoreContext_CreatePersistentSubscriptionFromCurrent_InvalidData_ErrorThrown(String streamName, String groupName, Type exceptionType)
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);

            Should.Throw(async () => { await context.CreatePersistentSubscriptionFromCurrent(streamName, groupName); }, exceptionType);
        }

        [Fact]
        public async Task EventStoreContext_CreatePersistentSubscriptionFromPosition_SubscriptionCreated()
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);

            await context.CreatePersistentSubscriptionFromPosition(EventStoreContextTestData.StreamName,
                EventStoreContextTestData.GroupName, EventStoreContextTestData.StartPosition);

            connection.Verify(x => x.CreatePersistentSubscriptionAsync(EventStoreContextTestData.StreamName, EventStoreContextTestData.GroupName, It.IsAny<PersistentSubscriptionSettings>(), It.IsAny<UserCredentials>()));
        }

        [Theory]
        [InlineData("", "groupName", typeof(ArgumentNullException))]
        [InlineData(null, "groupName", typeof(ArgumentNullException))]
        [InlineData("streamName", "", typeof(ArgumentNullException))]
        [InlineData("streamName", null, typeof(ArgumentNullException))]
        public async Task EventStoreContext_CreatePersistentSubscriptionFromPosition_InvalidData_ErrorThrown(String streamName, String groupName, Type exceptionType)
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);

            Should.Throw(async () => { await context.CreatePersistentSubscriptionFromPosition(streamName, groupName, EventStoreContextTestData.StartPosition); }, exceptionType);
        }

        #endregion

        #region Delete Persistent Subscription Tests

        [Fact]
        public async Task EventStoreContext_DeletePersistentSubscription_SubscriptionDeleted()
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);

            await context.DeletePersistentSubscription(EventStoreContextTestData.StreamName,
                EventStoreContextTestData.GroupName);

            connection.Verify(x => x.DeletePersistentSubscriptionAsync(EventStoreContextTestData.StreamName, EventStoreContextTestData.GroupName, It.IsAny<UserCredentials>()));
        }

        #endregion

        #region Insert Events
        
        [Fact]
        public async Task EventStoreContext_InsertEvents_EventsInserted()
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreContextTestData.ConnectionString);
            Mock<IEventStoreConnection> connection = new Mock<IEventStoreConnection>();
            Mock<ISerialiser> serialiser = new Mock<ISerialiser>();
            Func<EventStoreConnectionSettings, IEventStoreConnection> connectionResolver = settings =>
            {
                return connection.Object;
            };

            PaymentCentric.Logging.Logger.Initialise(NullLogger.Instance);

            EventStoreContext context = new EventStoreContext(eventStoreConnectionSettings, connectionResolver, serialiser.Object);

            await context.InsertEvents(EventStoreContextTestData.StreamName, EventStoreContextTestData.ExpectedVersion,
                EventStoreContextTestData.DomainEvents);

            connection.Verify(x => x.AppendToStreamAsync(EventStoreContextTestData.StreamName, EventStoreContextTestData.ExpectedVersion,
                It.IsAny<List<EventData>>(), It.IsAny<UserCredentials>()));
        }

        #endregion
    }

    public class TestEvent : DomainEvent
    {
        private TestEvent(Guid aggregateId, Guid eventId) : base(aggregateId, eventId)
        {

        }

        public static TestEvent Create()
        {
            return new TestEvent(Guid.NewGuid(), Guid.NewGuid());
        }
    }
}
