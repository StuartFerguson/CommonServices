using System;
using System.Collections.Generic;
using Shared.EventSourcing;

namespace SubscriptionService.UnitTests
{
    public class EventStoreContextTestData
    {
        public static String ConnectionString = "ConnectTo=tcp://admin:changeit@127.0.0.2:1113;VerboseLogging=true;";

        public static String StreamName = "TestStream1";
        public static String GroupName = "TestGroup1";
        public static Guid PersistentSubscriptionId = Guid.Parse("4c0e3203-412e-4097-8cb5-78952034357a");
        public static Int32 BufferSize = 10;
        public static Int32 StartPosition = 1;
        public static Int32 ExpectedVersion = 0;
        public static Int64 FromVersion = 0;

        public static List<DomainEvent> DomainEvents = new List<DomainEvent>
        {
            TestEvent.Create(),
            TestEvent.Create()
        };
    }
}