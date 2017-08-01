using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Blocks;
using DAL.Repositories;

namespace DAL.Services
{
    public class CFAndFlagService
    {
        public static List<CFAndFlagCountSupplier> GetCountsPerSupplier()
        {
            var cpsCounts = CFAndFlagRepository.GetCFCountPerSupplier();
            var flagCounts = CFAndFlagRepository.GetFlagCountPerSupplier();

            var result = new List<CFAndFlagCountSupplier>();

            result.AddRange(cpsCounts);

            foreach (var cfAndFlagCountSupplier in flagCounts)
            {
                var existing = result.Find(c => c.ClientId == cfAndFlagCountSupplier.ClientId && c.SupplierId == cfAndFlagCountSupplier.SupplierId);

                if (existing != null)
                {
                    existing.FlagCount = cfAndFlagCountSupplier.FlagCount;
                }
                else
                {
                    result.Add(cfAndFlagCountSupplier);
                }
            }

            return result;
        }

        public static List<CFAndFlagCountSupplier> GetCountsPerSupplier_OPT()
        {
            var cpsCounts = CFAndFlagRepository.GetCFCountPerSupplier();
            var flagCounts = CFAndFlagRepository.GetFlagCountPerSupplier();

            var result = new Dictionary<string, CFAndFlagCountSupplier>();

            foreach (var item in cpsCounts)
            {
                result.Add(item.ClientId + "_" + item.SupplierId, item);
            }

            foreach (var cfAndFlagCountSupplier in flagCounts)
            {
                var key = cfAndFlagCountSupplier.ClientId + "_" + cfAndFlagCountSupplier.SupplierId;

                var existing = result.ContainsKey(key) ? result[key] : null;

                if (existing != null)
                {
                    existing.FlagCount = cfAndFlagCountSupplier.FlagCount;
                }
                else
                {
                    result.Add(cfAndFlagCountSupplier.ClientId + "_" + cfAndFlagCountSupplier.SupplierId, cfAndFlagCountSupplier);
                }
            }

            return result.Select(v => v.Value).ToList();
        }
    }
}
