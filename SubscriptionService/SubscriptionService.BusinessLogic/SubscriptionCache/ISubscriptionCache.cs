using System;

namespace SubscriptionService.BusinessLogic.SubscriptionCache
{
    public interface ISubscriptionCache<T> where T : class
    {
        #region Events

        /// <summary>
        /// Occurs when [item added].
        /// </summary>
        event EventHandler<T> ItemAdded;

        /// <summary>
        /// Occurs when [item removed].
        /// </summary>
        event EventHandler<T> ItemRemoved;

        /// <summary>
        /// Occurs when [item updated].
        /// </summary>
        event EventHandler<T> ItemUpdated;
        
        #endregion

        #region Methods

        /// <summary>
        /// Initialises the cache.
        /// </summary>
        void InitialiseCache();

        #endregion
    }
}