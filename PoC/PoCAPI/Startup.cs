using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PoCAPI.Services;
using PoCCommon.Database;
using PoCCommon.Services;

namespace PoCAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<EventRaiserOptions>(Configuration);
            
            services.AddTransient<EventRaiser>();
            services.AddTransient<WatermarkService>();
            services.AddTransient<HeartbeatService>();
            
            services.AddDbContext<PocDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlDB"),
                    sqlOptions => sqlOptions.MigrationsAssembly("PoCAPI"));
            });
            
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage());
            
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager recurringJobManagerJobs, HeartbeatService heartbeatService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            //THIS MAKES IT CALL HEARTBEAT ONCE A MINUTE
            app.UseHangfireServer();
            recurringJobManagerJobs.AddOrUpdate("heartbeat", () => heartbeatService.CheckHeartbeat(), Cron.Minutely);
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}