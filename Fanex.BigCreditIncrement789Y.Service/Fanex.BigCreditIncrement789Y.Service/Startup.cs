using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace Fanex.BigCreditIncrement789Y.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            services.AddControllers();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fanex.BigCreditIncrement789Y.Service", Version = "v1" });
            });

            ServiceCollectionExtensions.LoadConfig();
            ServiceCollectionExtensions.SentryRegister(services);
            ServiceCollectionExtensions.FanexDataRegister(services, Configuration);
            ServiceCollectionExtensions.InternalServiceRegister(services);
            ServiceCollectionExtensions.StartScheduler(services);

            //services.Configure<HostOptions>(options =>
            //{
            //    options.ShutdownTimeout = TimeSpan.FromSeconds(45);
            //});

            //services.ConfigureAll<IHostApplicationLifetime>(lifetime =>
            //{
            //    lifetime.ApplicationStopped.Register(() =>
            //    {
            //        Console.WriteLine($"Application is stopped at {DateTimeOffset.Now}");
            //    });

            //    lifetime.ApplicationStopping.Register(() =>
            //    {
            //        Console.WriteLine($"Application is being stopped at {DateTimeOffset.Now}");
            //    });
            //});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fanex.BigCreditIncrement789Y.Service v1"));
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}