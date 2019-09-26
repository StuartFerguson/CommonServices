using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shared.EventStore;
using Shared.General;
using SubscriptionService.BusinessLogic.Repository;

namespace SubscriptionService.BusinessLogic.EventStore
{
    public class EventStoreManager : IEventStoreManager
    {
        #region Fields

        /// <summary>
        /// The event store context function
        /// </summary>
        private readonly Func<String, IEventStoreContext> EventStoreContextFunc;

        /// <summary>
        /// The configuration repository resolver
        /// </summary>
        private readonly Func<INewConfigurationRepository> ConfigurationRepositoryResolver;

        /// <summary>
        /// The event store settings
        /// </summary>
        private readonly EventStoreSettings EventStoreSettings;

        /// <summary>
        /// The context
        /// </summary>
        private IEventStoreContext Context;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreManager" /> class.
        /// </summary>
        /// <param name="eventStoreContextFunc">The event store context function.</param>
        /// <param name="configurationRepositoryResolver">The configuration repository resolver.</param>
        /// <param name="eventStoreSettings">The event store settings.</param>
        public EventStoreManager(Func<String, IEventStoreContext> eventStoreContextFunc, Func<INewConfigurationRepository> configurationRepositoryResolver,
            IOptions<EventStoreSettings> eventStoreSettings)
        {
            this.EventStoreContextFunc = eventStoreContextFunc;
            this.ConfigurationRepositoryResolver = configurationRepositoryResolver;
            this.EventStoreSettings = eventStoreSettings.Value;
        }

        #endregion

        #region Public Methods

        #region public async Task<IEventStoreContext> GetEventStoreContext()
        /// <summary>
        /// Gets the event store context.
        /// </summary>
        /// <param name="eventAppearedEventHandler">The event appeared event handler.</param>
        /// <param name="subscriptionDroppedEventHandler">The subscription dropped event handler.</param>
        /// <param name="liveProcessStartedEventHandler">The live process started event handler.</param>
        /// <returns></returns>
        public async Task<IEventStoreContext> GetEventStoreContext(EventAppearedEventHandler eventAppearedEventHandler,
            SubscriptionDroppedEventHandler subscriptionDroppedEventHandler,
            LiveProcessStartedEventHandler liveProcessStartedEventHandler)
        {
            if (this.Context == null)
            {
                IEventStoreContext resolvedContext = this.EventStoreContextFunc(this.EventStoreSettings.ConnectionString);

                // Wire up the context event handlers
                resolvedContext.EventAppeared += eventAppearedEventHandler;
                resolvedContext.SubscriptionDropped += subscriptionDroppedEventHandler;
                resolvedContext.LiveProcessStarted += liveProcessStartedEventHandler;

                resolvedContext.ConnectionDestroyed += this.ConnectionDestroyedEventHandler;

                this.Context = resolvedContext;
            }

            // return the context
            return this.Context;

        }
        #endregion

        /// <summary>
        /// Handles the ConnectionDestroyed event of the EventStoreContext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ConnectionDestroyedEventHandler(Object sender, EventArgs e)
        {
            Exception exception = new Exception("EventStore connection destoyed. Service now stopping");
            Logger.LogError(exception);

            throw exception;
        }

        #endregion
    }
}