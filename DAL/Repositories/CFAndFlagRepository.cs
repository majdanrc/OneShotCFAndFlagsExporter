using DAL.Blocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DAL.Repositories
{
    public class CFAndFlagRepository
    {
        //private const string QuerySql = @"select * from {0} where TextData not like 'exec sp_reset_connection' and LoginName is not NULL and LoginName<>'NT SERVICE\ReportServer'";
        private const string QuerySql = @"SELECT 
	                                        CL.idClient,
	                                        CL.nameClient,

	                                        COALESCE(STUFF((SELECT distinct ';' + CF1.codeCPS
                                                 FROM CPS CF1
                                                 WHERE CL.idClient = CF1.idClient AND CF1.listCPS <> 2
                                                    FOR XML PATH(''), TYPE
                                                    ).value('.', 'NVARCHAR(MAX)')
                                                ,1,1,''), '') CF1s,

	                                        COALESCE(STUFF((SELECT distinct ';' + CF2.codeCPS
                                                 FROM CPS CF2
                                                 WHERE CL.idClient = CF2.idClient AND CF2.listCPS <> 1
                                                    FOR XML PATH(''), TYPE
                                                    ).value('.', 'NVARCHAR(MAX)')
                                                ,1,1,''), '') CF2s,

	                                        COALESCE(STUFF((SELECT distinct ';' + FT.ShortName
                                                 FROM ClientFlag FLAG
		                                         INNER JOIN FlagT FT on FLAG.idflag = FT.idflag
                                                 WHERE CL.idClient = FLAG.idClient AND FT.idlanguage = 1
                                                    FOR XML PATH(''), TYPE
                                                    ).value('.', 'NVARCHAR(MAX)')
                                                ,1,1,''), '') FLAGs

                                        FROM Client CL
                                        ORDER BY nameClient
                                        ";

        public static List<CFAndFlag> GetRows()
        {
            var result = new List<CFAndFlag>();

            using (var reader = DbAccess.GetReaderSimple(QuerySql))
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
                            CF1s = !reader.IsDBNull(reader.GetOrdinal("CF1s"))
                                ? reader.GetString(reader.GetOrdinal("CF1s"))
                                : string.Empty,
                            CF2s = !reader.IsDBNull(reader.GetOrdinal("CF2s"))
                                ? reader.GetString(reader.GetOrdinal("CF2s"))
                                : string.Empty,
                            Flags = !reader.IsDBNull(reader.GetOrdinal("FLAGs"))
                                ? reader.GetString(reader.GetOrdinal("FLAGs"))
                                : string.Empty
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
