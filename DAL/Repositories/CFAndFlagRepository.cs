using DAL.Blocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DAL.Repositories
{
    public class CFAndFlagRepository
    {
        private const string MainQuerySql = @"SELECT 
	                                        CL.idClient,
	                                        CL.nameClient,

	                                        COALESCE(STUFF((SELECT '#;#' + CF1.nameCPS
                                                 FROM CPS CF1
                                                 WHERE CL.idClient = CF1.idClient AND CF1.listCPS <> 2
                                                    FOR XML PATH(''), TYPE
                                                    ).value('.', 'NVARCHAR(MAX)')
                                                ,1,1,''), '') CF1s,

	                                        COALESCE(STUFF((SELECT '#;#' + CF2.nameCPS
                                                 FROM CPS CF2
                                                 WHERE CL.idClient = CF2.idClient AND CF2.listCPS <> 1
                                                    FOR XML PATH(''), TYPE
                                                    ).value('.', 'NVARCHAR(MAX)')
                                                ,1,1,''), '') CF2s,

	                                        COALESCE(STUFF((SELECT '#;#' + FT.ShortName
                                                 FROM ClientFlag FLAG
		                                         INNER JOIN FlagT FT on FLAG.idflag = FT.idflag
                                                 WHERE CL.idClient = FLAG.idClient AND FT.idlanguage = 1
                                                    FOR XML PATH(''), TYPE
                                                    ).value('.', 'NVARCHAR(MAX)')
                                                ,1,1,''), '') FLAGs

                                        FROM Client CL
                                        ORDER BY nameClient
                                        ";

        private const string CPSCountPerSupplierQuery = @"SELECT 
	                                        CL.idClient,
	                                        CL.nameClient,

											CPSS.idSupplier as SupplierId,

											count(case when CPSm.listCPS <> 2 then 1 else null end) as CF1Count,
											count(case when CPSm.listCPS <> 1 then 1 else null end) as CF2Count

                                        FROM Client CL
										INNER JOIN CPS CPSm on CPSm.idClient=CL.idClient
										INNER JOIN CPSSupplier CPSS on CPSS.idCPS=CPSm.idCPS
										GROUP BY CL.idClient, CL.nameClient, CPSS.idSupplier
                                        ORDER BY nameClient, SupplierId
                            ";

        private const string FlagCountPerSupplierQuery = @"SELECT 
	                                        CL.idClient,
	                                        CL.nameClient,
											SPF.idSupplier as SupplierId,
											count(SPF.idflag) as FlagCount
									
                                        FROM Client CL
										INNER JOIN SupplierFlag SPF on SPF.idClient=CL.idClient
										GROUP BY CL.idClient, CL.nameClient, SPF.idsupplier
                                        ORDER BY nameClient, SupplierId
                            ";

        public static List<CFAndFlag> GetRows()
        {
            var result = new List<CFAndFlag>();

            using (var reader = DbAccess.GetReaderSimple(MainQuerySql))
            {
                try
                {
                    while (reader.Read())
                    {
                        var row = new CFAndFlag
                        {
                            ClientId = !reader.IsDBNull(reader.GetOrdinal("idClient"))
                                ? reader.GetInt32(reader.GetOrdinal("idClient"))
                                : -1,
                            ClientName = !reader.IsDBNull(reader.GetOrdinal("nameClient"))
                                ? reader.GetString(reader.GetOrdinal("nameClient"))
                                : string.Empty,
                            Flags = !reader.IsDBNull(reader.GetOrdinal("FLAGs"))
                                ? reader.GetString(reader.GetOrdinal("FLAGs"))
                                : string.Empty
                        };

                        var cf1s = !reader.IsDBNull(reader.GetOrdinal("CF1s"))
                                ? reader.GetString(reader.GetOrdinal("CF1s"))
                                : string.Empty;
                        var cf2s = !reader.IsDBNull(reader.GetOrdinal("CF2s"))
                                ? reader.GetString(reader.GetOrdinal("CF2s"))
                                : string.Empty;

                        row.CF1s = Regex.Replace(cf1s, @"^;#\s*", string.Empty, RegexOptions.Multiline);
                        row.CF2s = Regex.Replace(cf2s, @"^;#\s*", string.Empty, RegexOptions.Multiline);

                        result.Add(row);
                    }
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }

            return result;
        }

        public static List<CFAndFlagCountSupplier> GetCFCountPerSupplier()
        {
            var result = new List<CFAndFlagCountSupplier>();

            using (var reader = DbAccess.GetReaderSimple(CPSCountPerSupplierQuery))
            {
                try
                {
                    while (reader.Read())
                    {
                        var row = new CFAndFlagCountSupplier
                        {
                            ClientId = !reader.IsDBNull(reader.GetOrdinal("idClient"))
                                ? reader.GetInt32(reader.GetOrdinal("idClient"))
                                : -1,
                            ClientName = !reader.IsDBNull(reader.GetOrdinal("nameClient"))
                                ? reader.GetString(reader.GetOrdinal("nameClient"))
                                : string.Empty,
                            SupplierId = !reader.IsDBNull(reader.GetOrdinal("SupplierId"))
                                ? reader.GetInt32(reader.GetOrdinal("SupplierId"))
                                : -1,
                            CF1Count = !reader.IsDBNull(reader.GetOrdinal("CF1Count"))
                                ? reader.GetInt32(reader.GetOrdinal("CF1Count"))
                                : -1,
                            CF2Count = !reader.IsDBNull(reader.GetOrdinal("CF2Count"))
                                ? reader.GetInt32(reader.GetOrdinal("CF2Count"))
                                : -1,
                        };

                        result.Add(row);
                    }
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }

            return result;
        }

        public static List<CFAndFlagCountSupplier> GetFlagCountPerSupplier()
        {
            var result = new List<CFAndFlagCountSupplier>();

            using (var reader = DbAccess.GetReaderSimple(FlagCountPerSupplierQuery))
            {
                try
                {
                    while (reader.Read())
                    {
                        var row = new CFAndFlagCountSupplier
                        {
                            ClientId = !reader.IsDBNull(reader.GetOrdinal("idClient"))
                                ? reader.GetInt32(reader.GetOrdinal("idClient"))
                                : -1,
                            ClientName = !reader.IsDBNull(reader.GetOrdinal("nameClient"))
                                ? reader.GetString(reader.GetOrdinal("nameClient"))
                                : string.Empty,
                            SupplierId = !reader.IsDBNull(reader.GetOrdinal("SupplierId"))
                                ? reader.GetInt32(reader.GetOrdinal("SupplierId"))
                                : -1,
                            FlagCount = !reader.IsDBNull(reader.GetOrdinal("FlagCount"))
                                ? reader.GetInt32(reader.GetOrdinal("FlagCount"))
                                : -1
                        };

                        result.Add(row);
                    }
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }

            return result;
        }
    }
}
