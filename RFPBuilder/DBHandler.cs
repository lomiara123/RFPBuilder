using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace RFPBuilder
{
    class DBHandler
    {
        public static bool checkIfDbExists()
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=master";
            string databaseName = "RequestForProposal";
            using (var conn = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand($"SELECT db_id('{databaseName}')", conn))
                {
                    conn.Open();
                    return (command.ExecuteScalar() != DBNull.Value);
                }
            }
        }

        public static void createDB()
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=master";
            string createDB = "CREATE DATABASE MyDatabase";
            using (var conn = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(createDB, conn))
                {
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        private static void createMasterRFP(SqlConnection connection)
        {

        }
    }
}
