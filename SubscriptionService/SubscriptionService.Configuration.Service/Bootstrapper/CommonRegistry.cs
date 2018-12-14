using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StructureMap;
using SubscriptionService.BusinessLogic.Repository;

namespace SubscriptionService.Configuration.Service.Bootstrapper
{
    public class CommonRegistry : Registry
    {
        public CommonRegistry()
        {
            For<IConfigurationRepository>().Use<ConfigurationRepository>().AlwaysUnique().Singleton();
        }
    }
}
