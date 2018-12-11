using System;

namespace SubscriptionService.UnitTests
{
    public class SubscriptionTestData
    {
        /// <summary>
        /// The subscription identifier
        /// </summary>
        public static Guid SubscriptionId = Guid.Parse("F0DAA0F3-DE93-4114-B0A3-CAC4F5A437E4");

        /// <summary>
        /// The stream name
        /// </summary>
        public static String StreamName = "Test Stream";

        /// <summary>
        /// The group name
        /// </summary>
        public static String GroupName = "Test Group";

        /// <summary>
        /// The start position null
        /// </summary>
        public static Int32? StartPositionNull = null;

        /// <summary>
        /// The start position negative
        /// </summary>
        public static Int32? StartPositionNegative = -1;

        /// <summary>
        /// The start position zero
        /// </summary>
        public static Int32? StartPositionZero = 0;

        /// <summary>
        /// The start position positive
        /// </summary>
        public static Int32? StartPositionPositive = 1;
    }
}