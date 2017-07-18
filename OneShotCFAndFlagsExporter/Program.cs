using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneShotCFAndFlagsExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var rows = CFAndFlagRepository.GetRows();

            var cf1Max = rows.Select(l => l.GetCF1List().Count).OrderByDescending(c => c).First();
            var cf2Max = rows.Select(l => l.GetCF2List().Count).OrderByDescending(c => c).First();
            var flagMax = rows.Select(l => l.GetFlags().Count).OrderByDescending(c => c).First();

            var maxLength = Math.Max(flagMax, Math.Max(cf1Max, cf2Max));

            foreach (var row in rows)
            {
                Console.WriteLine(row.ClientName + " " + row.GetFlags().Count());
            }

            Console.ReadKey();
        }
    }
}
