using Fanex.BigCreditIncrement789Y.Business.Services;
using Microsoft.Extensions.Hosting;
using ONEbook.UM.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fanex.BigCreditIncrement789Y.Service.Services
{
    public class MainBackground60MinService(MainService mainService, IMaintenanceService um, Log log)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var delayToNextHour = CalculateDelayToNextHour();
                    await Task.Delay(delayToNextHour, stoppingToken);

                    if (!await um.IsUnderMaintenanceAsync())
                    {
                        mainService.Execute();
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    log.Log(ex);
                }
            }
        }

        private static TimeSpan CalculateDelayToNextHour()
        {
            var now = DateTime.UtcNow;
            var nextHour = now.Date.AddHours(now.Hour + 1);
            return nextHour - now;
        }
    }
}