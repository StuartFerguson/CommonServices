using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using StructureMap;
using System;
using System.Collections.Generic;
using Xunit;

namespace MessagingService.Service.Tests
{
    public class BootstrapperTests
    {
        [Fact]
        public void VerifyBootstrapperIsValid_Development()
        {
            ServiceCollection servicesCollection = new ServiceCollection();
            Mock<IHostingEnvironment> hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(he => he.EnvironmentName).Returns("Development");

            Startup.Configuration = SetupMemoryConfiguration();
            Startup.HostingEnvironment = hostingEnvironment.Object;

            IContainer container = Startup.GetConfiguredContainer(servicesCollection, hostingEnvironment.Object);

            AddTestRegistrations(container);

            container.AssertConfigurationIsValid();
        }

        [Fact]
        public void VerifyBootstrapperIsValid_Production()
        {
            ServiceCollection servicesCollection = new ServiceCollection();
            Mock<IHostingEnvironment> hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(he => he.EnvironmentName).Returns("Production");

            Startup.Configuration = SetupMemoryConfiguration();
            Startup.HostingEnvironment = hostingEnvironment.Object;

            IContainer container = Startup.GetConfiguredContainer(servicesCollection, hostingEnvironment.Object);

            AddTestRegistrations(container);

            container.AssertConfigurationIsValid();
        }

        private IConfigurationRoot SetupMemoryConfiguration()
        {
            Dictionary<String, String> configuration = new Dictionary<String, String>();

            IConfigurationBuilder builder = new ConfigurationBuilder();

            //configuration.Add("EventStoreSettings:ConnectionString", "ConnectTo=tcp://admin:changeit@127.0.0.1:1112;VerboseLogging=true;");
            //configuration.Add("EventStoreSettings:ConnectionName", "UnitTestConnection");
            //configuration.Add("EventStoreSettings:HttpPort", "2113");

            builder.AddInMemoryCollection(configuration);

            return builder.Build();
        }

        private void AddTestRegistrations(IContainer container)
        {
            //container.Configure(c => c.For<IOptions<EventStoreConnectionSettings>>().Use<OptionsManager<EventStoreConnectionSettings>>()); 
            //container.Configure(c => c.For<IOptionsFactory<EventStoreConnectionSettings>>().Use<OptionsFactory<EventStoreConnectionSettings>>());
        }

    }
}
