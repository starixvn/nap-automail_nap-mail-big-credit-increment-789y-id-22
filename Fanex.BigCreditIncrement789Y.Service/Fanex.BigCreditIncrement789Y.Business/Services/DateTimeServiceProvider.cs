using System.Collections.Generic;

namespace Fanex.BigCreditIncrement789Y.Business.Services
{
    public interface DateTimeServiceProvider
    {
        DateTimeService DateTime { get; }
    }

    public class DateTimeServiceProviderImp : DateTimeServiceProvider
    {
        private Dictionary<bool, DateTimeService> Date =>
            new()
            {
                { true, new DateTimeServiceManualImp()},
                { false, new DateTimeServiceImp()},
            };

        public DateTimeService DateTime => Date[AppConfigService.Config.IsManual];
    }
}