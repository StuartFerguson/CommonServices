using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Builders;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using TechTalk.SpecFlow;

namespace MessagingService.IntegrationTests.Specflow.Common
{
    [Binding]    
    public class GenericSteps
    {
        protected ScenarioContext ScenarioContext;
        protected IContainerService MessagingServiceContainer;
        protected INetworkService TestNetwork;
        protected Int32 MessagingServicePort;

        protected GenericSteps(ScenarioContext scenarioContext) 
        {
            this.ScenarioContext = scenarioContext;
        }
        
        protected void RunSystem(String testFolder)
        {
            String messagingServiceContainerName = $"messagingService{Guid.NewGuid():N}";
            
            // Build a network
            this.TestNetwork = new Builder().UseNetwork($"testnetwork{Guid.NewGuid()}").Build();

            // Messaging Service Container
            this.MessagingServiceContainer = new Builder()
                .UseContainer()
                .WithName(messagingServiceContainerName)
                .WithEnvironment("SeedingType=IntegrationTest", "ASPNETCORE_ENVIRONMENT=IntegrationTest")
                .UseImage("messagingserviceservice")
                .ExposePort(5002)
                .UseNetwork(this.TestNetwork)
                .Mount($"D:\\temp\\docker\\{testFolder}", "/home", MountType.ReadWrite)                
                .Build()
                .Start()
                .WaitForPort("5002/tcp", 30000);
            
            // Cache the ports
            this.MessagingServicePort= this.MessagingServiceContainer.ToHostExposedEndpoint("5002/tcp").Port;            
       }

        protected void StopSystem()
        {
            if (this.MessagingServiceContainer != null)
            {
                this.MessagingServiceContainer.Stop();
                this.MessagingServiceContainer.Remove(true);
            }

            if (this.TestNetwork != null)
            {
                this.TestNetwork.Stop();
                this.TestNetwork.Remove(true);
            }
        }
    }
}
