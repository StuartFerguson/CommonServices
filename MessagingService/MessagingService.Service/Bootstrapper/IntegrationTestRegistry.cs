using System.Diagnostics.CodeAnalysis;
using MessagingService.Service.Services.Email;
using MessagingService.Service.Services.Email.IntegrationTest;
using MessagingService.Service.Services.SMS;
using MessagingService.Service.Services.SMS.IntegrationTest;
using StructureMap;

namespace MessagingService.Service.Bootstrapper
{
    [ExcludeFromCodeCoverage]
    public class IntegrationTestRegistry : Registry
    {
        public IntegrationTestRegistry()
        {
            For<IEmailServiceProxy>().Use<IntegrationTestEmailServiceProxy>().Singleton();
            For<ISMSServiceProxy>().Use<IntegrationTestSMSServiceProxy>().Singleton();
        }
    }
}