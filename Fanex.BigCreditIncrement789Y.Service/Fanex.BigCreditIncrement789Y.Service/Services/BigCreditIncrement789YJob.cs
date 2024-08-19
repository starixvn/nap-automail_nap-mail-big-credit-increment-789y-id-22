using Fanex.BigCreditIncrement789Y.Business.Services;
using ONEbook.UM.Services;
using Quartz;
using System.Threading.Tasks;

namespace Fanex.BigCreditIncrement789Y.Service.Services
{
    public class BigCreditIncrement789YJob : IJob
    {
        private readonly IMaintenanceService um;
        private readonly MainService service;

        public BigCreditIncrement789YJob(
            IMaintenanceService um,
            MainService service)
        {
            this.um = um;
            this.service = service;
        }

        public Task Execute(IJobExecutionContext context)
        {
            if (um.IsUnderMaintenance())
            {
                return Task.CompletedTask;
            }

            service.Execute();

            return Task.CompletedTask;
        }
    }
}