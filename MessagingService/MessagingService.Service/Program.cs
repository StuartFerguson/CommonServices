using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MessagingService.Service
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(String[] args)
        {
            Console.Title = "Messaging Service API";
 
            BuildWebHost(args).Run();
        }
 
        public static IWebHost BuildWebHost(String[] args)
        {
            String environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
 
            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: false)
                .AddJsonFile($"hosting.{environmentName}.json", optional: true)
                .Build();
 
            IWebHost host = new WebHostBuilder().UseKestrel()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>().Build();
 
            return host;
        }
    }
}
