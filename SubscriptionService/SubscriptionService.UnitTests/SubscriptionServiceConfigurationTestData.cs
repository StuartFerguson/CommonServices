using System;
using System.Collections.Generic;
using System.Text;

namespace SubscriptionService.UnitTests
{
    public class SubscriptionServiceConfigurationTestData
    {
        public const String SubscriptionStreamName = "TestStream";
        public static Guid SubscriptionStreamId = Guid.Parse("876BBEBF-3354-4E4D-8BA0-EE322211155B");

        public const String SubscriptionStream1Name = "TestStream1";
        public static Guid SubscriptionStream1Id = Guid.Parse("876BBEBF-3354-4E4D-8BA0-EE322211155B");
        
        public static Guid SubscriptionStream2Id = Guid.Parse("9CBAA03F-FF8B-4D7F-B6E0-865F412E9609");
        public const String SubscriptionStream2Name = "TestStream2";

        public static Guid SubscriptionGroupId = Guid.Parse("16605B25-ACE5-4780-87ED-C36BC426CE18");
        public static String SubscriptionGroupName = "GroupName";

        public static Guid EndPointId = Guid.Parse("CBCB485D-F510-4ADE-B9B8-27208468B9B9");
        public static String EndPointName = "TestEndpoint";
        public static String EndPointUrl = "http://testendpoint";

        public static Guid CatchUpSubscriptionId = Guid.Parse("D1AA40C2-040B-4AF6-9864-8EC0A2E20BCF");
        public static String CatchUpSubscriptionName = "TestCatchup";
        public static Int32 CatchUpSubscriptionPosition = 0;
        public static DateTime CatchUpSubscriptionCreatDateTime= new DateTime(2018,12,14);
    }
}
