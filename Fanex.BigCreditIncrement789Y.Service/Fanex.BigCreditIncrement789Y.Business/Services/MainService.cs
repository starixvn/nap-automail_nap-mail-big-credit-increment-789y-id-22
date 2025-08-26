using Fanex.BigCreditIncrement789Y.Business.Models;
using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.BigCreditIncrement789Y.Business.Services
{
    public interface MainService
    {
        JobMessage Execute();
    }

    public class MainServiceImp(
        Log log,
        DataService dataService,
        LetstalkClientService letsTalkService,
        ExcelService excelService,
        MessageService messageService,
        ImageService imageService,
        MailService mailService)
        : MainService
    {
        private readonly Mail mail = new() { Subject = string.Empty, Body = string.Empty };

        public JobMessage Execute()
        {
            var result = new ExecuteResultModel();
            JobMessage jobMessage = new JobMessage();
            try
            {
                result = SendNotification();
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
                log.Log(e);
            }
            finally
            {
                jobMessage.Mail = result.Mail;
                jobMessage.JobStatus = result.Success ? JobStatus.SuccessWithoutSendmail : JobStatus.DataIsNotReady;
                jobMessage.TaskLog = new TaskLog
                {
                    LogTime = DateTime.Now,
                    Message = result.Message
                };
            }

            return jobMessage;
        }

        private ExecuteResultModel SendNotification()
        {
            log.Log("Fanex.BigCreditIncrement789Y.Service");
            var fails = new List<string>();
            var rawData = dataService.QueryBigCreditIncrement();
            var errFiles = rawData.Count > 0
                ? SendLetsTalkImageData(rawData)
                : SendLetsTalkImageNoData();

            errFiles.ForEach(file =>
            {
                if (file.Length > 1)
                {
                    fails.Add(file);
                }
            });

            return new ExecuteResultModel
            {
                Success = fails.Count == 0,
                Message = string.Join(Environment.NewLine, fails),
                Mail = mail
            };
        }

        private List<string> SendLetsTalkImageData(List<PunterModel> data)
        {
            var rowPerBatch = data.Count;
            var totalBatch = data.Count / rowPerBatch + ((data.Count % rowPerBatch) > 0 ? 1 : 0);
            var imageSentFail = new List<string>();
            var setting = AppConfigService.Config.BigCreditIncrement789YSettings;

            for (var batchIndex = 0; batchIndex < totalBatch; batchIndex++)
            {
                var rowsOfBatch = data.Skip(batchIndex * rowPerBatch).Take(rowPerBatch).ToList();
                var excelPath = excelService.ExportExcel(rowsOfBatch, batchIndex, totalBatch, false, setting);
                var rowCount = rowsOfBatch.Count;
                var imagePath = ExportImage(setting, excelPath, batchIndex, rowCount);
                var status = SendImage(imagePath, setting.LetsTalkGroup);
                var countRetry = 0;
                while (!status && countRetry < 10)
                {
                    status = SendImage(imagePath, setting.LetsTalkGroup);
                    countRetry++;
                }

                if (!status)
                {
                    imageSentFail.Add(imagePath);
                }
            }

            var excelAllPath = excelService.ExportExcel(data, 0, 0, true, setting);
            SendImage(excelAllPath, setting.LetsTalkGroup);
            SendMail(setting, excelAllPath);

            return imageSentFail;
        }

        private void SendMail(BigCreditIncrement789YSettings setting, string excelAllPath)
        {
            mailService.SendMail(excelAllPath, setting.ExcelTitle, setting.Subject, DateTime.Now);
        }

        private List<string> SendLetsTalkImageNoData()
        {
            var setting = AppConfigService.Config.BigCreditIncrement789YSettings;
            var excelPath = excelService.ExportExcelWithNoData(setting);
            ExportImage(setting, excelPath, 0, 0);
            SendMail(setting, excelPath);
            return [string.Empty];
        }

        private string ExportImage(BigCreditIncrement789YSettings setting, string excelPath, int batchIndex,
            int rowCount)
        {
            var lastRow = rowCount + (rowCount > 0 ? 4 : 5);
            var image = AppConfigService.Config.ImageSettings["BigCreditIncrement789YSettings"];
            image.ImageName = setting.ExcelTitle;
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;
            var path = AppConfigService.Config.Path;
            var imagePath = imageService.ExportImage(excelPath, batchIndex, image.SheetName, image, path);

            return imagePath;
        }

        private bool SendImage(string imagePath, string group)
        {
            var letsTalkGroup = group;
            var letsTalkModel = messageService.CreateLetsTalkModel(imagePath, letsTalkGroup);
            var status = letsTalkService.SendMessage(letsTalkModel);

            return status;
        }
    }
}