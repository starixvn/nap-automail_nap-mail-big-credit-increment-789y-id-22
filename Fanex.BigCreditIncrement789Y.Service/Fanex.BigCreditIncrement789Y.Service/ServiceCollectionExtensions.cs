using Fanex.BigCreditIncrement789Y.Business.Models;
using Fanex.BigCreditIncrement789Y.Business.Services;
using Fanex.BigCreditIncrement789Y.Service.Services;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.LetstalkService.Client;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Newtonsoft.Json;
using OfficeOpenXml;
using Quartz;
using Quartz.Impl;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LetstalkConfig = Fanex.LetstalkService.Core.Services.AppConfigService;

namespace Fanex.BigCreditIncrement789Y.Service
{
    public static class ServiceCollectionExtensions
    {
        public static void LoadConfig()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText("appsettings.json");
            AppConfigService.Config = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
            AppConfigService.RecyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            LetstalkConfig.Config.LetstalkServiceApi = AppConfigService.Config.LetstalkServiceApi;
        }

        public static void SentryRegister(this IServiceCollection services)
        {
            var sentry = new SentryLogging(new SentryEngineOptions
            {
                Dsn = new Dsn(AppConfigService.Config.SentryClientKey)
            });

            LogManager
               .SetDefaultLogCategory(AppConfigService.Config.CategorySentryLog)
               .Use(new List<ILogEngine>
               {
                    sentry
               });
        }

        public static void FanexDataRegister(this IServiceCollection services, IConfiguration Configuration)
        {
            var connections = Configuration.GetSection("ConnectionStrings")
                .GetChildren()
                .AsEnumerable()
                .ToDictionary
                (
                     x => x.Key,
                     y => y.Value
                );

            DbSettingProviderManager
             .StartNewSession()
             .UseConnectionStrings(connections)
             .UseDefaultDbSettingProvider(Configuration.GetSection("StoredProcedurePath").Value)
             .Run();
        }

        public static void InternalServiceRegister(this IServiceCollection services)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            services.AddSingleton<Log, LogImp>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<DataService, DataServiceImp>();
            services.AddSingleton<MailService, MailServiceImp>();
            services.AddSingleton<MessageService, MessageServiceImp>();
            services.AddSingleton<LetstalkClientService, LetstalkClientServiceImp>();
            services.AddSingleton<ImageService, ImageServiceImp>();
            services.AddSingleton<DateTimeServiceProvider, DateTimeServiceProviderImp>();
            services.AddSingleton<ExcelService, ExcelServiceImp>();
            services.AddSingleton<MainService, MainServiceImp>();

            OnebookUMInitializer.InitializeUMClientManager();
            OnebookUMInitializer.RegisterSimpleInjector(services);
        }

        public static void StartScheduler(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.JobFactory = new BigCreditIncrement789YJobFactory(serviceProvider);

            var trigger = TriggerBuilder.Create()
                .WithIdentity("Fanex.BigCreditIncrement789Y.Service", "Fanex.BigCreditIncrement789Y.Service")
                .StartNow()
                .WithCronSchedule(AppConfigService.Config.CronSchedule)
                .Build();

            var job = JobBuilder.Create<BigCreditIncrement789YJob>().Build();
            scheduler.ScheduleJob(job, trigger).Wait();
            scheduler.Start().Wait();
        }
    }
}