using System;
using Shouldly;
using SubscriptionService.BusinessLogic.EventStoreContext;
using Xunit;

namespace SubscriptionService.UnitTests
{
    public class EventStoreConnectionSettingsTests
    {
        [Fact]
        public void EventStoreConnectionSettings_CanBeCreated_ValidConnectionString_IsCreated()
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreConnectionSettingsTestData.ConnectionString);

            eventStoreConnectionSettings.ShouldNotBeNull();
            eventStoreConnectionSettings.ConnectionString.ShouldBe(EventStoreConnectionSettingsTestData.ConnectionString);
            eventStoreConnectionSettings.ConnectionName.ShouldBeNull();
            eventStoreConnectionSettings.HttpPort.ShouldBe(EventStoreConnectionSettingsTestData.DefaultHttpPort);
            eventStoreConnectionSettings.IPAddress.ShouldBe(EventStoreConnectionSettingsTestData.IPAddress);
            eventStoreConnectionSettings.Password.ShouldBe(EventStoreConnectionSettingsTestData.Password);
            eventStoreConnectionSettings.TcpPort.ShouldBe(EventStoreConnectionSettingsTestData.TcpPort);
            eventStoreConnectionSettings.UserName.ShouldBe(EventStoreConnectionSettingsTestData.UserName);
        }

        [Fact]
        public void EventStoreConnectionSettings_CanBeCreated_ValidConnectionString_SetConnectionName_IsCreated()
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreConnectionSettingsTestData.ConnectionString, EventStoreConnectionSettingsTestData.ConnectionName);

            eventStoreConnectionSettings.ShouldNotBeNull();
            eventStoreConnectionSettings.ConnectionString.ShouldBe(EventStoreConnectionSettingsTestData.ConnectionString);
            eventStoreConnectionSettings.ConnectionName.ShouldBe(EventStoreConnectionSettingsTestData.ConnectionName);
            eventStoreConnectionSettings.HttpPort.ShouldBe(EventStoreConnectionSettingsTestData.DefaultHttpPort);
            eventStoreConnectionSettings.IPAddress.ShouldBe(EventStoreConnectionSettingsTestData.IPAddress);
            eventStoreConnectionSettings.Password.ShouldBe(EventStoreConnectionSettingsTestData.Password);
            eventStoreConnectionSettings.TcpPort.ShouldBe(EventStoreConnectionSettingsTestData.TcpPort);
            eventStoreConnectionSettings.UserName.ShouldBe(EventStoreConnectionSettingsTestData.UserName);
        }

        [Fact]
        public void EventStoreConnectionSettings_CanBeCreated_ValidConnectionString_SetHttpPort_IsCreated()
        {
            EventStoreConnectionSettings eventStoreConnectionSettings = EventStoreConnectionSettings.Create(EventStoreConnectionSettingsTestData.ConnectionString, httpPort:EventStoreConnectionSettingsTestData.HttpPort);

            eventStoreConnectionSettings.ShouldNotBeNull();
            eventStoreConnectionSettings.ConnectionString.ShouldBe(EventStoreConnectionSettingsTestData.ConnectionString);
            eventStoreConnectionSettings.ConnectionName.ShouldBeNull();
            eventStoreConnectionSettings.HttpPort.ShouldBe(EventStoreConnectionSettingsTestData.HttpPort);
            eventStoreConnectionSettings.IPAddress.ShouldBe(EventStoreConnectionSettingsTestData.IPAddress);
            eventStoreConnectionSettings.Password.ShouldBe(EventStoreConnectionSettingsTestData.Password);
            eventStoreConnectionSettings.TcpPort.ShouldBe(EventStoreConnectionSettingsTestData.TcpPort);
            eventStoreConnectionSettings.UserName.ShouldBe(EventStoreConnectionSettingsTestData.UserName);
        }

        [Theory]
        [InlineData("ConnectTo=tcp://admin:changeit127.0.0.2:1113;VerboseLogging=true;")]
        [InlineData("ConnectTo=tcp://admin:changeit@127.0.0.21113;VerboseLogging=true;")]
        [InlineData("ConnectTo=tcp://admin:changeit@127.0.0.2:1113VerboseLogging=true;")]
        [InlineData("ConnectTo=tcp:admin:changeit@127.0.0.2:1113;VerboseLogging=true;")]
        [InlineData("ConnectTo=tcp://adminchangeit@127.0.0.2:1113;VerboseLogging=true;")]
        public void EventStoreConnectionSettings_CanBeCreated_InvalidConnectionString_ErrorThrown(String connecttionString)
        {
            Should.Throw<ArgumentException>(() =>
            {
                EventStoreConnectionSettings eventStoreConnectionSettings =
                    EventStoreConnectionSettings.Create(connecttionString);
            });
        }
    }
}