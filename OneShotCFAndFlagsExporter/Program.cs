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

            foreach (var row in rows)
            {
                Console.WriteLine(row.ClientName);
            }
        }
    }
}
