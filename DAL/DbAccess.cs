using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DAL.Blocks
{
    public class DbAccess
    {
        private static string ConnectionString = ConfigurationManager.ConnectionStrings["EcoPortal"].ConnectionString;

        public static SqlDataReader GetReaderSimple(string query)
        {
            return GetReader(query, null, CommandType.Text);
        }

        public static SqlDataReader GetReaderWithProcedure(string query, ArrayList parameters)
        {
            return GetReader(query, parameters, CommandType.StoredProcedure);
        }

        private static SqlDataReader GetReader(string query, ArrayList parameters, CommandType commandType)
        {
            var connection = new SqlConnection(ConnectionString);

            var command = new SqlCommand(query, connection)
            {
                CommandTimeout = 120,
                CommandType = commandType
            };

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            SqlDataReader reader = null;

            try
            {
                connection.Open();
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            return reader;
        }
    }
}
