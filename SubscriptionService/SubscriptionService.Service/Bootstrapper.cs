using System;
using System.Reflection;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.EventStore;
using StructureMap;
using StructureMap.Pipeline;
using SubscriptionService.BusinessLogic;
using SubscriptionService.BusinessLogic.EventStore;
using SubscriptionService.BusinessLogic.Repository;
using SubscriptionService.BusinessLogic.Subscription;
using SubscriptionService.BusinessLogic.SubscriptionCache;
using SubscriptionService.Database;
using SubscriptionService.DataTransferObjects;

namespace SubscriptionService.Service
{
    public class Bootstrapper
    {
        public static IContainer Container;

        #region public static IContainer ConfigureServices(IServiceCollection services)        
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IContainer ConfigureServices(IServiceCollection services)
        {
            ConfigureCommonServices(services);

            IContainer container = new Container();
            
            container.Configure(ConfigureCommonContainer);

            container.Populate(services);

            InitialiseDatabase(services.BuildServiceProvider()).Wait();

            Bootstrapper.Container = container;

            return container;
        }
        #endregion

        #region private static void ConfigureCommonServices(IServiceCollection services)        
        /// <summary>
        /// Configures the common services.
        /// </summary>
        /// <param name="services">The services.</param>
        private static void ConfigureCommonServices(IServiceCollection services)
        {
            // Register services
            services.AddSingleton<ISubscriptionService, BusinessLogic.SubscriptionService>();
            services.AddSingleton<ISubscriptionCache<SubscriptionGroup>, SubscriptionCache>();
            services.AddTransient<ISubscription, Subscription>();
            services.AddSingleton<IConfigurationRepository, ConfigurationRepository>();
            services.AddSingleton<IEventStoreManager, EventStoreManager>();
            services.Configure<ServiceSettings>(Program.Configuration.GetSection("ServiceSettings"));
            services.Configure<EventStoreSettings>(Program.Configuration.GetSection("EventStoreSettings"));

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment == "IntegrationTest")
            {
                services.AddDbContext<SubscriptionServiceConfigurationContext>(builder =>
                        builder.UseInMemoryDatabase("SubscriptionServiceConfigurationContext"))
                    .AddTransient<SubscriptionServiceConfigurationContext>();
            }
            else
            {
                var connectionString =
                    Program.Configuration.GetConnectionString(nameof(SubscriptionServiceConfigurationContext));

                String migrationsAssembly = typeof(SubscriptionServiceConfigurationContext).GetTypeInfo().Assembly
                    .GetName().Name;

                services.AddDbContext<SubscriptionServiceConfigurationContext>(builder =>
                        builder.UseMySql(connectionString,
                            sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))
                    .AddTransient<SubscriptionServiceConfigurationContext>();
            }
        }
        #endregion
 
        #region private static void ConfigureCommonContainer(ConfigurationExpression configurationExpression)        
        /// <summary>
        /// Configures the common container.
        /// </summary>
        /// <param name="configurationExpression">The configuration expression.</param>
        private static void ConfigureCommonContainer(ConfigurationExpression configurationExpression)
        {
            var connString = Program.Configuration.GetValue<String>("EventStoreSettings:ConnectionString");
            var connectionName = Program.Configuration.GetValue<String>("EventStoreSettings:ConnectionName");
            var httpPort = Program.Configuration.GetValue<Int32>("EventStoreSettings:HttpPort");

            EventStoreConnectionSettings settings = EventStoreConnectionSettings.Create(connString, connectionName, httpPort);

            configurationExpression.For<IEventStoreContext>().Use<EventStoreContext>().Singleton().Ctor<EventStoreConnectionSettings>().Is(settings);

            Func<String, IEventStoreContext> eventStoreContextFunc = (connectionString) =>
                {  
                    EventStoreConnectionSettings connectionSettings = EventStoreConnectionSettings.Create(connectionString,connectionName, httpPort);

                    ExplicitArguments args = new ExplicitArguments().Set(connectionSettings);

                    return Bootstrapper.Container.GetInstance<IEventStoreContext>(args);
                };
            configurationExpression.For<Func<String, IEventStoreContext>>().Use(eventStoreContextFunc);

            Func<EventStoreConnectionSettings, IEventStoreConnection> eventStoreConnectionFunc = (connectionSettings) =>
            {  
                return EventStoreConnection.Create(connectionSettings.ConnectionString);                
            };

            configurationExpression.For<Func<EventStoreConnectionSettings, IEventStoreConnection>>().Use(eventStoreConnectionFunc);
        }
        #endregion
 
        #region private async Task InitialiseDatabase(IServiceProvider serviceProvider)
        /// <summary>
        /// Initialises the database.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
        private static async Task InitialiseDatabase(IServiceProvider serviceProvider)
        {
            using (IServiceScope scope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                SubscriptionServiceConfigurationContext SubscriptionServiceConfigurationContext = scope.ServiceProvider.GetRequiredService<SubscriptionServiceConfigurationContext>();

                DatabaseSeeding.InitialiseDatabase(SubscriptionServiceConfigurationContext);
            }
        }
        #endregion
    }
}
