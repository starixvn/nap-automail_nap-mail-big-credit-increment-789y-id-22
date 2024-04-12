using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;

namespace Fanex.BigCreditIncrement789Y.Service.Services
{
    public class BigCreditIncrement789YJobFactory : IJobFactory
    {
        private readonly IServiceProvider serviceProvider;

        public BigCreditIncrement789YJobFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return serviceProvider.GetService<BigCreditIncrement789YJob>();
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}