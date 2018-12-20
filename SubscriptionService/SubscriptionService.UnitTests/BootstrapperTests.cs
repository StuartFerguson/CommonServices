using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using SubscriptionService.Service;
using Xunit;

namespace SubscriptionService.UnitTests
{
    public class BootstrapperTests
    {
        [Fact(Skip = "Come back to this test not working when local db stopped")]
        public void VerifyBootstrapperIsValid()
        {
            ServiceCollection servicesCollection = new ServiceCollection();

            Program.Configuration = SetupMemoryConfiguration();

            IContainer container = Bootstrapper.ConfigureServices(servicesCollection);

            AddTestRegistrations(container);

            container.AssertConfigurationIsValid();
        }
        
        private IConfigurationRoot SetupMemoryConfiguration()
        {
            Dictionary<String, String> configuration = new Dictionary<String, String>();

            configuration.Add("ConnectionStrings:SubscriptionServiceConfigurationContext", "server=localhost;database=SubscriptionServiceConfiguration;user id=root;password=Pa55word");
            
            configuration.Add("EventStoreSettings:ConnectionString", "ConnectTo=tcp://admin:changeit@127.0.0.1:1113;VerboseLogging=true;");
            configuration.Add("EventStoreSettings:ConnectionName", "Subscription Service");
            configuration.Add("EventStoreSettings:HttpPort", "2113");

            configuration.Add("ServiceSettings:CacheTimeout", "30");

            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(configuration);

            return builder.Build();
        }

        private void AddTestRegistrations(IContainer container)
        {

        }
    }
}