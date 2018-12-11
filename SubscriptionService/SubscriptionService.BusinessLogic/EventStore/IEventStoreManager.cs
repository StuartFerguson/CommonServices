using System.Threading.Tasks;
using Shared.EventStore;

namespace SubscriptionService.BusinessLogic.EventStore
{
    public interface IEventStoreManager
    {
        /// <summary>
        /// Gets the event store context.
        /// </summary>
        /// <param name="eventAppearedEventHandler">The event appeared event handler.</param>
        /// <param name="subscriptionDroppedEventHandler">The subscription dropped event handler.</param>
        /// <param name="liveProcessStartedEventHandler">The live process started event handler.</param>
        /// <returns></returns>
        Task<IEventStoreContext> GetEventStoreContext(EventAppearedEventHandler eventAppearedEventHandler, SubscriptionDroppedEventHandler subscriptionDroppedEventHandler, LiveProcessStartedEventHandler liveProcessStartedEventHandler);
    }
}