using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using DAL.Blocks;
using DAL.Services;
using OfficeOpenXml;

namespace OneShotCFAndFlagsExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataRows = CFAndFlagRepository.GetRows();

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            var fileName = "export_" + timestamp + ".xlsx";

            var newFile = new FileInfo("./" + fileName);
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var currentRow = 1;

                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("main");

                foreach (var row in dataRows)
                {
                    Console.WriteLine("processing main: " + row.ClientName);

                    worksheet.Cells[currentRow, 1].Value = row.ClientName;

                    worksheet.Cells[currentRow + 1, 1].Value = "Client Filter 1";
                    var currentColumn = 2;
                    foreach (var cf1 in row.GetCF1List())
                    {
                        worksheet.Cells[currentRow + 1, currentColumn].Value = cf1;
                        currentColumn++;
                    }

                    worksheet.Cells[currentRow + 2, 1].Value = "Client Filter 2";
                    currentColumn = 2;
                    foreach (var cf2 in row.GetCF2List())
                    {
                        worksheet.Cells[currentRow + 2, currentColumn].Value = cf2;
                        currentColumn++;
                    }

                    worksheet.Cells[currentRow + 3, 1].Value = "Flags";
                    currentColumn = 2;
                    foreach (var flag in row.GetFlags())
                    {
                        worksheet.Cells[currentRow + 3, currentColumn].Value = flag;
                        currentColumn++;
                    }

                    currentRow += 5;
                }


                xlPackage.Save();
            }

            var cpsCounts = CFAndFlagRepository.GetCFCountPerSupplier();
            var flagCounts = CFAndFlagRepository.GetFlagCountPerSupplier();
            
            var cpsCalculated = cpsCounts.GroupBy(c => c.ClientId).Select(gr => new
            {
                ClientId = gr.Key,
                ClientName = gr.Select(cn => cn.ClientName).FirstOrDefault(),
                MaxCF1 = gr.Max(g => g.CF1Count),
                MaxCF2 = gr.Max(g => g.CF2Count),
                AvgCF1 = gr.Average(g => g.CF1Count),
                AvgCF2 = gr.Average(g => g.CF2Count)
            }).ToDictionary(k => k.ClientId, v => v);

            var flagsCalculated = flagCounts.GroupBy(c => c.ClientId).Select(gr => new
            {
                ClientId = gr.Key,
                ClientName = gr.Select(cn => cn.ClientName).FirstOrDefault(),
                MaxFlags = gr.Max(g => g.FlagCount),
                AvgFlags = gr.Average(g => g.FlagCount)
            }).ToDictionary(k => k.ClientId, v => v);

            var clientIds = cpsCalculated.Keys.ToList().Union(flagsCalculated.Keys).ToList();

            var fileNameCounts = "COUNT_export_" + timestamp + ".xlsx";

            var newFileCounts = new FileInfo("./" + fileNameCounts);
            using (ExcelPackage xlPackage = new ExcelPackage(newFileCounts))
            {
                var currentRow = 1;

                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("counts");

                foreach (var row in clientIds)
                {
                    Console.WriteLine("processing counts: " + row);

                    var cpsRow = cpsCalculated.ContainsKey(row) ? cpsCalculated[row] : null;
                    var flagRow = flagsCalculated.ContainsKey(row) ? flagsCalculated[row] : null;

                    var clientName = cpsRow != null
                        ? cpsRow.ClientName
                        : (flagRow != null ? flagRow.ClientName : string.Empty);

                    worksheet.Cells[currentRow, 1].Value = clientName;

                    worksheet.Cells[currentRow + 1, 1].Value = "Maximum Number of CF1 per supplier";
                    worksheet.Cells[currentRow + 1, 2].Value = cpsRow?.MaxCF1 ?? 0;

                    worksheet.Cells[currentRow + 2, 1].Value = "Average Number of CF1 per supplier";
                    worksheet.Cells[currentRow + 2, 2].Value = cpsRow?.AvgCF1 ?? 0;

                    worksheet.Cells[currentRow + 3, 1].Value = "Maximum Number of CF2 per supplier";
                    worksheet.Cells[currentRow + 3, 2].Value = cpsRow?.MaxCF2 ?? 0;

                    worksheet.Cells[currentRow + 4, 1].Value = "Average Number of CF2 per supplier";
                    worksheet.Cells[currentRow + 4, 2].Value = cpsRow?.AvgCF2 ?? 0;

                    worksheet.Cells[currentRow + 5, 1].Value = "Maximum Number of Flags per supplier";
                    worksheet.Cells[currentRow + 5, 2].Value = flagRow?.MaxFlags ?? 0;

                    worksheet.Cells[currentRow + 6, 1].Value = "Average Number of Flags per supplier";
                    worksheet.Cells[currentRow + 6, 2].Value = flagRow?.AvgFlags ?? 0;

                    currentRow += 8;
                }


                xlPackage.Save();
            }

            Console.WriteLine("done... main exported to " + fileName);
            Console.WriteLine("done... counts exported to " + fileNameCounts);

            Console.WriteLine("press any key...");

            Console.ReadKey();
        }
    }
}
