using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using StructureMap;
using SubscriptionService.BusinessLogic;
using Logger = Shared.General.Logger;
using ILogger =  Microsoft.Extensions.Logging.ILogger;

namespace SubscriptionService.Service
{
    public class Program
    {
        /// <summary>
        /// The configuration
        /// </summary>
        public static IConfigurationRoot Configuration;

        static async Task Main(String[] args)
        {
            Console.Title = "Subscription Service";

            IServiceCollection services = new ServiceCollection();

            Program.InitialiseConfiguration(services);

            Program.ConfigureLogging(services);

            IContainer container = Bootstrapper.ConfigureServices(services);
            
            Logger.LogInformation("About to Start Service");

            ISubscriptionService SubscriptionService = container.GetInstance<ISubscriptionService>();

            SubscriptionService.StartService();
            Logger.LogInformation("Service Started");

            Console.Read();
        }

        #region Private Methods

        #region private static void InitialiseConfiguration(IServiceCollection services)
        /// <summary>
        /// Initialises the configuration.
        /// </summary>
        /// <param name="services">The services.</param>
        private static void InitialiseConfiguration(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

            Program.Configuration = configurationBuilder.Build();
        }
        #endregion

        #region private static void ConfigureLogging(IServiceCollection services)        
        /// <summary>
        /// Configures the logging.
        /// </summary>
        /// <param name="services">The services.</param>
        private static void ConfigureLogging(IServiceCollection services)
        {
            services.AddLogging();

            // Build the Service Provider
            var serviceProvider = services.BuildServiceProvider();

            // Resolve the Logger factory
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            // Configure NLog
            String nlogConfigFilename = $"nlog.config";
            loggerFactory.AddNLog();

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            
            LogManager.LoadConfiguration(nlogConfigFilename);
            
            // Create a logger instance
            ILogger logger = loggerFactory.CreateLogger("Subscription Service");
            
            // Setup the static logger
            Logger.Initialise(logger);
        }
        #endregion

        #endregion
    }
}
