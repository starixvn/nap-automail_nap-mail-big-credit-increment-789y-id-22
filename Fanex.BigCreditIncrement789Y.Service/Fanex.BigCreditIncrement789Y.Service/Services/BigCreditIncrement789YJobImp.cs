using Fanex.BigCreditIncrement789Y.Business.Services;
using Microsoft.Extensions.DependencyInjection;
using ONEbook.UM.Services;
using Quartz;
using System.Threading.Tasks;

namespace Fanex.BigCreditIncrement789Y.Service.Services
{
    public class BigCreditIncrement789YJobImp : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var services = ServiceCollectionExtensions.Services;
            var provider = services.BuildServiceProvider();
            var umService = provider.GetService<IMaintenanceService>();
            var bigCreditIncrement789Y = provider.GetService<MainService>();
            if (umService.IsUnderMaintenance())
            {
                return Task.CompletedTask;
            }

            bigCreditIncrement789Y.Execute();
            return Task.CompletedTask;
        }
    }
}