using System.IO;
using Marten;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PoCCommon.Database;
using PoCCommon.Services;
using PoCEventHandler.Services;
using Serilog;
using Serilog.Events;

namespace PoCEventHandler
{
    public class Startup
    {
        public static ServiceProvider ConfigureServices(IServiceCollection services, string[] args)
        {
            var configuration = ConfigureOptions(services, args);
            ConfigureLogging();
            services.AddMarten(configuration.GetConnectionString("Marten"));
            
            services.AddDbContext<PocDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SqlDB")));
            
            services.AddTransient<EventHandler>();
            services.AddTransient<WatermarkService>();
            
            return services.BuildServiceProvider();
        }


        private static IConfigurationRoot ConfigureOptions(IServiceCollection services, string[] args)
        {
            //This means we first load from appsettings.json 
            //Then override with env vars if they exist
            //Then override with command line args if they exist
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            var config = configBuilder.Build();
            return config;
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()

                .WriteTo.Console(LogEventLevel.Information)
                .CreateLogger();
        }
    }
}