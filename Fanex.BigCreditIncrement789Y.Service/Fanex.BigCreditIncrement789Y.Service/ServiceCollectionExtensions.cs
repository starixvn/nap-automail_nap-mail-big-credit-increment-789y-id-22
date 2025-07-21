using Fanex.BigCreditIncrement789Y.Business.Models;
using Fanex.BigCreditIncrement789Y.Business.Services;
using Fanex.BigCreditIncrement789Y.Service.Services;
using Fanex.Data.Repository;
using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.IO;
using LetstalkConfig = Fanex.LetstalkService.Client.AppConfigService;

namespace Fanex.BigCreditIncrement789Y.Service
{
    using AppConfigService = Business.Services.AppConfigService;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Services { get; set; }

        public static void AddConfig(this IServiceCollection services)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText("appsettings.json");
            AppConfigService.Config = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
            AppConfigService.RecyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            LetstalkConfig.Config.LetstalkServiceApi = AppConfigService.Config.LetstalkServiceApi;
        }

        public static void AddInternalServices(this IServiceCollection services)
        {
            Services = services;

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

            services.AddSingleton<MainBackground60MinService>();

            MailClientHandler.Start();
        }
    }
}