using Fanex.BigCreditIncrement789Y.Business.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.BigCreditIncrement789Y.Business.Services
{
    public interface ExcelService
    {
        string ExportExcel(List<PunterModel> data, int batchIndex, int totalBatch, bool isAllInfo, BigCreditIncrement789YSettings setting);

        string ExportExcelWithNoData(BigCreditIncrement789YSettings setting);
    }

    public class ExcelServiceImp : ExcelService
    {
        public string ExportExcel(List<PunterModel> data, int batchIndex, int totalBatch, bool isAllInfo, BigCreditIncrement789YSettings setting)
        {
            using var excelPackage = new ExcelPackage(new FileInfo("Templates/BigCreditIncrement789Y.xlsx"));
            using var template = excelPackage.Workbook.Worksheets["Data"];
            template.Cells["B5"].LoadFromCollection(data, false);

            RenderTitle(template);
            RenderData(data, template);

            return Save(excelPackage, batchIndex, isAllInfo, setting);
        }

        private static void RenderData(List<PunterModel> data, ExcelWorksheet template)
        {
            var startRowIndex = 5;
            var maxRowIndex = data.Count + startRowIndex - 1;

            for (int rowIndex = startRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                template.Row(rowIndex).Height = 19.5;
            }

            for (int columnIndex = 2; columnIndex < 9; columnIndex++)
            {
                template.Cells[5, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[5, columnIndex].StyleID;
            }
        }

        private static string Save(ExcelPackage excelPackage, int batchIndex, bool isAllInfo, BigCreditIncrement789YSettings setting)
        {
            var date = DateTime.Now;
            var fileName = isAllInfo
                ? $"{setting.ExcelTitle} {date:yyyyMMdd HH00}.xlsx"
                : $"{setting.ExcelTitle} Batch {batchIndex + 1} {date:yyyyMMdd HH00}.xlsx";
            var path = $"{AppConfigService.Config.Path}/{date:yyyyMMdd}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $"{path}/{fileName}";
            excelPackage.Workbook.Worksheets["Data"].Column(9).Hidden = true;
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);
            excelPackage.Dispose();
            return path;
        }

        public string ExportExcelWithNoData(BigCreditIncrement789YSettings setting)
        {
            using var excelPackage = new ExcelPackage(new FileInfo("Templates/BigCreditIncrement789Y-NoData.xlsx"));
            using var template = excelPackage.Workbook.Worksheets["Data"];
            RenderTitle(template);

            return Save(excelPackage, 0, true, setting);
        }

        private static void RenderTitle(ExcelWorksheet template)
        {
            var now = DateTime.Now.AddHours(-1);
            var title = new List<string>
            {
                "[Credit increment notification]",
                $"Date: {now:MM/dd/yyyy} GMT-4",
                $"Time: {now:HH:00} - {now:HH:59}"
            };

            template.Cells["B2"].Value = string.Join(Environment.NewLine, title);
        }
    }
}