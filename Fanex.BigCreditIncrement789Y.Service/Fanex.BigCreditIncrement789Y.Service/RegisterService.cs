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
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LetstalkConfig = Fanex.LetstalkService.Core.Services.AppConfigService;

namespace Fanex.BigCreditIncrement789Y.Service
{
    public static class RegisterService
    {
        public static void RegisterServices(IConfiguration configuration, Container container)
        {
            LoadConfig();
            SentryRegister();
            ConfigureFanexData(configuration);
            DependencyInjection(container);
        }

        public static Container Container { get; set; }

        public static IServiceCollection Services { get; set; }

        private static void LoadConfig()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText("appsettings.json");
            AppConfigService.Config = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
            AppConfigService.RecyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            LetstalkConfig.Config.LetstalkServiceApi = AppConfigService.Config.LetstalkServiceApi;
        }

        private static void SentryRegister()
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

        private static void ConfigureFanexData(IConfiguration Configuration)
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

        private static void DependencyInjection(Container container)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            container.RegisterSingleton<Log, LogImp>();
            container.RegisterSingleton<IDynamicRepository, DynamicRepository>();
            container.RegisterSingleton<DataService, DataServiceImp>();
            container.RegisterSingleton<MailService, MailServiceImp>();
            container.RegisterSingleton<MessageService, MessageServiceImp>();
            container.RegisterSingleton<LetstalkClientService, LetstalkClientServiceImp>();
            container.RegisterSingleton<ImageService, ImageServiceImp>();
            container.RegisterSingleton<DateTimeServiceProvider, DateTimeServiceProviderImp>();
            container.RegisterSingleton<ExcelService, ExcelServiceImp>();
            container.RegisterSingleton<MainService, MainServiceImp>();

            Container = container;
            OnebookUMInitializer.InitializeUMClientManager();
            OnebookUMInitializer.RegisterSimpleInjector(container);
        }
    }
}