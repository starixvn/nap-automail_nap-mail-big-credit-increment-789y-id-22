using Fanex.BigCreditIncrement789Y.Business.Services;
using Quartz;
using Quartz.Impl;

namespace Fanex.BigCreditIncrement789Y.Service.Services
{
    public class BigCreditIncrement789YService
    {
        public static async void Execute()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
            IJobDetail job = JobBuilder.Create<BigCreditIncrement789YJobImp>().Build();
            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("BigCreditIncrement789Y", "BigCreditIncrement789Y")
            .StartNow()
            .WithCronSchedule(AppConfigService.Config.CronSchedule)
            .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}