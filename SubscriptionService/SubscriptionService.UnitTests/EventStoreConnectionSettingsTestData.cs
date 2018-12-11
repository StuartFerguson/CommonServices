using System;

namespace SubscriptionService.UnitTests
{
    public class EventStoreConnectionSettingsTestData
    {
        public static String ConnectionString = "ConnectTo=tcp://admin:changeit@127.0.0.2:1113;VerboseLogging=true;";
        public static Int32 DefaultHttpPort = 2113;
        public static Int32 HttpPort = 2114;
        public static String IPAddress = "127.0.0.2";
        public static String Password = "changeit";
        public static Int32 TcpPort = 1113;
        public static String UserName = "admin";
        public static String ConnectionName = "Test Connection";
    }
}