using System;
using Moq;
using Shouldly;
using SubscriptionService.BusinessLogic.Repository;
using SubscriptionService.BusinessLogic.Subscription;
using SubscriptionService.BusinessLogic.SubscriptionCache;
using SubscriptionService.DataTransferObjects;
using Xunit;

namespace SubscriptionService.UnitTests
{
    public class SubscriptionServiceTests
    {
        [Fact]
        public void SubscriptionService_CanBeCreated_IsCreated()
        {
            Mock<ISubscriptionCache<SubscriptionGroup>> subscriptionCache = new Mock<ISubscriptionCache<SubscriptionGroup>>();
            Mock<Func<ISubscription>> subscriptionResolver = new Mock<Func<ISubscription>>();
            Mock<Func<IConfigurationRepository>> configurationRepository = new Mock<Func<IConfigurationRepository>>();

            BusinessLogic.SubscriptionService SubscriptionService = new BusinessLogic.SubscriptionService(
                subscriptionCache.Object,
                subscriptionResolver.Object, configurationRepository.Object);

            SubscriptionService.ShouldNotBeNull();
        }
    }
}
