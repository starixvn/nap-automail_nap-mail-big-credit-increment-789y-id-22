using Fanex.BigCreditIncrement789Y.Business.Services;
using ONEbook.UM.Services;
using Quartz;
using System.Threading.Tasks;

namespace Fanex.BigCreditIncrement789Y.Service.Services
{
    public class BigCreditIncrement789YJobImp : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var umService = RegisterService.Container.GetInstance<IMaintenanceService>();
            var bigCreditIncrement789Y = RegisterService.Container.GetInstance<MainService>();
            if (umService.IsUnderMaintenance())
            {
                return Task.CompletedTask;
            }

            bigCreditIncrement789Y.Execute();
            return Task.CompletedTask;
        }
    }
}