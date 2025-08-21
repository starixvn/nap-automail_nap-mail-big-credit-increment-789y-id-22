using System.Collections.Generic;

namespace Fanex.BigCreditIncrement789Y.Business.Services
{
    using System.IO;
    using LetstalkService.Client.Models;

    public interface MessageService
    {
        LetstalkModel CreateLetsTalkModel(string fileName, string letsTalkGroup);
    }

    public class MessageServiceImp : MessageService
    {
        public LetstalkModel CreateLetsTalkModel(string fileName, string letsTalkGroup)
        {
            var attachments = new List<LetstalkAttachment>
            {
                new()
                {
                    FileName = fileName,
                    Bytes = File.ReadAllBytes(fileName)
                }
            };

            return new LetstalkModel
            {
                TargetName = AppConfigService.Config.TargetName,
                Group = letsTalkGroup,
                LetstalkAttachments = attachments,
                Subject = string.Empty,
                Content = string.Empty
            };
        }
    }
}