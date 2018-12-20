using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using Shared.General;
using StructureMap;
using SubscriptionService.Configuration.Service.Bootstrapper;
using SubscriptionService.Database;
using Swashbuckle.AspNetCore.Swagger;

namespace SubscriptionService.Configuration.Service
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        #region Properties
 
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public static IConfigurationRoot Configuration { get; set; }
 
        /// <summary>
        /// Gets or sets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        public static IHostingEnvironment HostingEnvironment { get; set; }

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        public static IContainer Container { get; private set; }

        #endregion

        #region Constructors
 
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The env.</param>
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder =
                new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional:true, reloadOnChange:true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional:true)
                    .AddEnvironmentVariables();
 
            Startup.Configuration = builder.Build();
            Startup.HostingEnvironment = env;
        }

        #endregion

        #region Public Methods

        #region public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)        
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            String nlogConfigFilename = $"nlog.config";
            if (String.Compare(HostingEnvironment.EnvironmentName, "Development", true) == 0)
            {
                nlogConfigFilename = $"nlog.{HostingEnvironment.EnvironmentName}.config";
            }

            loggerFactory.AddConsole();
            loggerFactory.ConfigureNLog(Path.Combine(HostingEnvironment.ContentRootPath, nlogConfigFilename));
            loggerFactory.AddNLog();

            ILogger logger = loggerFactory.CreateLogger("SubscriptionService");

            Logger.Initialise(logger);
            Logger.LogInformation("Hello from Logger.");

            ConfigurationReader.Initialise(Startup.Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Subscription Service Configuration API v1");
            });
        }
        #endregion

        #region public static IContainer GetConfiguredContainer(IServiceCollection services, IHostingEnvironment hostingEnvironment)        
        /// <summary>
        /// Gets the configured container.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        /// <returns></returns>
        public static IContainer GetConfiguredContainer(IServiceCollection services, IHostingEnvironment hostingEnvironment)
        {
            var container = new Container();
            
            container.Configure(config =>
            {
                config.AddRegistry<CommonRegistry>();
                config.Populate(services);
            });

            Startup.Container = container;

            return container;
        }
        #endregion

        #region public IServiceProvider ConfigureServices(IServiceCollection services)        
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ConfigureMiddlewareServices(services);

            IContainer container = GetConfiguredContainer(services, HostingEnvironment);
            
            return container.GetInstance<IServiceProvider>();
        }
        #endregion

        #endregion

        #region Private Methods

        #region private static void ConfigureMiddlewareServices(IServiceCollection services)        
        /// <summary>
        /// Configures the middleware services.
        /// </summary>
        /// <param name="services">The services.</param>
        private static void ConfigureMiddlewareServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            if (HostingEnvironment.IsEnvironment("IntegrationTest"))
            {
                services.AddDbContext<SubscriptionServiceConfigurationContext>(builder =>
                        builder.UseInMemoryDatabase("SubscriptionServiceConfigurationContext"))
                    .AddTransient<SubscriptionServiceConfigurationContext>();
            }
            else
            {
                var connectionString =
                    Startup.Configuration.GetConnectionString(nameof(SubscriptionServiceConfigurationContext));

                services.AddDbContext<SubscriptionServiceConfigurationContext>(builder =>
                        builder.UseMySql(connectionString))
                    .AddTransient<SubscriptionServiceConfigurationContext>();;
            }

            services.AddMvcCore();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Subscription Service Configuration API", Version = "v1" });
            });
        }
        #endregion

        #endregion
    }
}
