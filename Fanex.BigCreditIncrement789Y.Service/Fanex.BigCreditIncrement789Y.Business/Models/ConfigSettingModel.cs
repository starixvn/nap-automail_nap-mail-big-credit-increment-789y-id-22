using System;
using System.Collections.Generic;

namespace Fanex.BigCreditIncrement789Y.Business.Models
{
    public class ConfigSettingModel
    {
        public string LetsTalkServiceApi { get; set; }

        public string Path { get; set; }

        public string TargetName { get; set; }

        public string Password { get; set; }

        public int MailId { get; set; }

        public bool IsTest { get; set; }

        public bool IsManual { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public Dictionary<string, ImageProperty> ImageSettings { get; set; } = new();

        public BigCreditIncrement789YSettings BigCreditIncrement789YSettings { get; set; }
    }
}