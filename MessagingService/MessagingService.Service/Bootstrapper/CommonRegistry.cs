using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MessagingService.Service.CommandHandlers;
using MessagingService.Service.Services;
using MessagingService.Service.Services.Email;
using MessagingService.Service.Services.Email.Smtp2Go;
using MessagingService.Service.Services.SMS;
using MessagingService.Service.Services.SMS.TheSmsWorks;
using Shared.CommandHandling;
using StructureMap;

namespace MessagingService.Service.Bootstrapper
{
    [ExcludeFromCodeCoverage]
    public class CommonRegistry : Registry
    {
        public CommonRegistry()
        {
            For<ICommandRouter>().Use<CommandRouter>().Singleton();
            For<IEmailServiceProxy>().Use<Smtp2GoProxy>().Singleton();
            For<ISMSServiceProxy>().Use<TheSmsWorksProxy>().Singleton();
        }
    }
}
