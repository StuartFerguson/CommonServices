namespace SubscriptionService.BusinessLogic
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// Starts the service.
        /// </summary>
        void StartService();

        /// <summary>
        /// Stops the service.
        /// </summary>
        void StopService();
    }

    public enum CacheEventType
    {
        Added,
        Updated,
        Removed
    }
}
