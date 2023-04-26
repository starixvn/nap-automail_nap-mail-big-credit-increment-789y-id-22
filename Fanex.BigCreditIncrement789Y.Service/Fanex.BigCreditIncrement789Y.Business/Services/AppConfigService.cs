using Fanex.BigCreditIncrement789Y.Business.Models;
using Microsoft.IO;

namespace Fanex.BigCreditIncrement789Y.Business.Services
{
    public static class AppConfigService
    {
        public static ConfigSettingModel Config { get; set; }

        public static RecyclableMemoryStreamManager RecyclableMemoryStreamManager { get; set; }
    }
}