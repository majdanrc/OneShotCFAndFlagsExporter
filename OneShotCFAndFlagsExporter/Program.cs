using DAL.Repositories;
using System;
using System.Linq;
using System.IO;
using OfficeOpenXml;

namespace OneShotCFAndFlagsExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataRows = CFAndFlagRepository.GetRows();

            var cf1Max = dataRows.Select(l => l.GetCF1List().Count).OrderByDescending(c => c).First();
            var cf2Max = dataRows.Select(l => l.GetCF2List().Count).OrderByDescending(c => c).First();
            var flagMax = dataRows.Select(l => l.GetFlags().Count).OrderByDescending(c => c).First();

            var maxLength = Math.Max(flagMax, Math.Max(cf1Max, cf2Max));

            var fileName = "export_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var newFile = new FileInfo("./" + fileName);
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var currentRow = 1;

                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("test");

                foreach (var row in dataRows)
                {
                    Console.WriteLine("processing: " + row.ClientName);

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
                    currentColumn = 2;

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

            Console.WriteLine("done... exported to " + fileName);
            Console.WriteLine("press any key...");

            Console.ReadKey();
        }
    }
}
