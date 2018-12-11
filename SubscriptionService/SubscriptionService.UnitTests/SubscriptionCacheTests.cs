using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using SubscriptionService.BusinessLogic;
using SubscriptionService.BusinessLogic.Repository;
using SubscriptionService.BusinessLogic.SubscriptionCache;
using SubscriptionService.DataTransferObjects;
using Xunit;

namespace SubscriptionService.UnitTests
{
    public class SubscriptionCacheTests
    {
        [Fact]
        public void SubscriptionCache_CanBeCreated_IsCreated()
        {
            Mock<IConfigurationRepository> configurationRepository = new Mock<IConfigurationRepository>();

            var config = Options.Create(new ServiceSettings
            {
                CacheTimeout = 30
            });

            ISubscriptionCache<SubscriptionGroup> subscriptionCache = new SubscriptionCache(configurationRepository.Object, config);

            subscriptionCache.ShouldNotBeNull();
        }
    }
}
