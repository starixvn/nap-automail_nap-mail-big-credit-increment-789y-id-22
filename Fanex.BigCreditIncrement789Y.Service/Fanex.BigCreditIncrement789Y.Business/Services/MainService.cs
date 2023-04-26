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

    public class MainServiceImp : MainService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly LetstalkClientService letstalkService;
        private readonly ExcelService excelService;
        private readonly MessageService messageService;
        private readonly ImageService imageService;
        private readonly MailService mailService;

        private readonly Mail mail = new() { Subject = string.Empty, Body = string.Empty };

        public MainServiceImp(
            Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            ExcelService excelService,
            MessageService messageService,
            ImageService imageService,
            MailService mailService)
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelService = excelService;
            this.messageService = messageService;
            this.imageService = imageService;
            this.mailService = mailService;
        }

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
            var letstalkFails = new List<string>();
            var rawData = dataService.QueryBigCreditIncrement();
            var errFiles = rawData.Count > 0
                ? SendLetstalkImageData(rawData)
                : SendLetstalkImageNoData();

            errFiles.ForEach(file =>
            {
                if (file.Length > 1)
                {
                    letstalkFails.Add(file);
                }
            });

            return new ExecuteResultModel
            {
                Success = letstalkFails.Count == 0,
                Message = string.Join(Environment.NewLine, letstalkFails),
                Mail = mail
            };
        }

        private List<string> SendLetstalkImageData(List<PunterModel> data)
        {
            var rowPerBatch = data.Count;
            var totalBatch = data.Count / rowPerBatch + ((data.Count % rowPerBatch) > 0 ? 1 : 0);
            var imageSentFail = new List<string>();
            var letstalkFails = new List<string>();
            var setting = AppConfigService.Config.BigCreditIncrement789YSettings;

            for (var batchIndex = 0; batchIndex < totalBatch; batchIndex++)
            {
                var rowsOfBatch = data.Skip(batchIndex * rowPerBatch).Take(rowPerBatch).ToList();
                var excelPath = excelService.ExportExcel(rowsOfBatch, batchIndex, totalBatch, false, setting);
                var rowCount = rowsOfBatch.Count;
                var imagePath = ExportImage(setting, excelPath, batchIndex, rowCount);
                var status = SendImage(imagePath, setting.LetstalkGroup);
                var countRetry = 0;
                while (!status && countRetry < 10)
                {
                    status = SendImage(imagePath, setting.LetstalkGroup);
                    countRetry++;
                }

                if (!status)
                {
                    imageSentFail.Add(imagePath);
                }
            }

            var excelAllPath = excelService.ExportExcel(data, 0, 0, true, setting);
            SendImage(excelAllPath, setting.LetstalkGroup);
            SendMail(setting, excelAllPath);

            if (imageSentFail.Count > 0)
            {
                letstalkFails.Add($"{string.Join(Environment.NewLine, imageSentFail)} - Letstalk sent images Unsuccessfully.");
            }

            return imageSentFail;
        }

        private void SendMail(BigCreditIncrement789YSettings setting, string excelAllPath)
        {
            mailService.SendMail(excelAllPath, setting.Exceltitle, setting.Subject, DateTime.Now);
        }

        private List<string> SendLetstalkImageNoData()
        {
            var setting = AppConfigService.Config.BigCreditIncrement789YSettings;
            var excelPath = excelService.ExportExcelWithNoData(setting);
            ExportImage(setting, excelPath, 0, 0);
            SendMail(setting, excelPath);
            return new List<string> { string.Empty };
        }

        private string ExportImage(BigCreditIncrement789YSettings setting, string excelPath, int batchIndex, int rowCount)
        {
            var lastRow = rowCount + (rowCount > 0 ? 4 : 5);
            var image = AppConfigService.Config.ImageSettings["BigCreditIncrement789YSettings"];
            image.ImageName = setting.Exceltitle;
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;
            var path = AppConfigService.Config.Path;
            var imagePath = imageService.ExportImage(excelPath, batchIndex, image.SheetName, image, path);

            return imagePath;
        }

        private bool SendImage(string imagePath, string group)
        {
            var letstalkGroup = group;
            var letstalkModel = messageService.CreateLetstalkModel(imagePath, letstalkGroup);
            var status = letstalkService.SendMessage(letstalkModel);

            return status;
        }
    }
}