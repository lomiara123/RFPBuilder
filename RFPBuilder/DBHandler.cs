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
        public static void deleteDB()
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=master";
            string commandSTR = "alter database RequestForProposal set single_user with rollback immediate";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var command = new SqlCommand(commandSTR, conn))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SqlCommand($"DROP DATABASE RequestForProposal", conn))
                {
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public static bool checkDatabaseExist()
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=master";
            
            using (var conn = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("SELECT db_id('RequestForProposal')", conn))
                {
                    conn.Open();
                    return (command.ExecuteScalar() != DBNull.Value);
                }
            }
        }

        public static bool checkModuleMappingExist(string rfpName)
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            string sql = "select * " +
                          "from ModuleMap " +
                          "where RFPName = @rfpName";

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@rfpName", rfpName);

                    using (var reader = command.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        public static bool checkResponseMappingExist(string rfpName)
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            string sql = "select * " +
                          "from ResponseMap " +
                          "where RFPName = @rfpName";

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@rfpName", rfpName);

                    using (var reader = command.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        public static bool checkPositionMappingExist(string rfpName)
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            string sql = "select * " +
                          "from PositionMap " +
                          "where RFPName = @rfpName";

            using (var conn = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@rfpName", rfpName);

                    using (var reader = command.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        public static void initModuleMapping(string rfpName)
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            string selectModules = "select * " +
                                   "from ModuleLookup";
            string insertModuleMapping = "insert into ModuleMap" +
                                         "(RFPName, ModuleID) " +
                                         "VALUES ";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var command = new SqlCommand(selectModules, conn))
                {
                    var reader = command.ExecuteReader();

                    while(reader.Read())
                    {
                        string tmp = "('" + rfpName + "', '" + reader["ModuleId"].ToString() + "'),";

                        insertModuleMapping += tmp;
                    }

                    insertModuleMapping = insertModuleMapping.Remove(insertModuleMapping.Length - 1);

                    reader.Close();
                }

                using (var command = new SqlCommand(insertModuleMapping, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void initResponseMapping(string rfpName)
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            string selectModules = "select * " +
                                   "from ResponseLookup";
            string insertModuleMapping = "insert into ResponseMap" +
                                         "(RFPName, ResponseMaster) " +
                                         "VALUES ";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var command = new SqlCommand(selectModules, conn))
                {
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string tmp = "('" + rfpName + "', '" + reader["ResponseId"].ToString() + "'),";

                        insertModuleMapping += tmp;
                    }

                    insertModuleMapping = insertModuleMapping.Remove(insertModuleMapping.Length - 1);

                    reader.Close();
                }

                using (var command = new SqlCommand(insertModuleMapping, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void createDB()
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=master";
            string createDB = "CREATE DATABASE RequestForProposal";
            using (var conn = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(createDB, conn))
                {
                    conn.Open();
                    command.ExecuteNonQuery();
                    DBHandler.createModuleLookupTable(conn);
                    DBHandler.createResponseLookupTable(conn);
                    DBHandler.createMasterRFPTable(conn);
                    DBHandler.createModuleMapTable(conn);
                    DBHandler.createResponseMapTable(conn);
                    DBHandler.createPositionMapTable(conn);
                }
            }
        }
        
        private static void createMasterRFPTable(SqlConnection connection)
        {
            string createTable = "use RequestForProposal " + 
                                 "CREATE TABLE MasterRFP ( " +
                                    "RFPName varchar(255) NOT NULL," +
                                    "ModuleID varchar(255) NOT NULL, " +
                                    "ReqID varchar(255), " +
                                    "Criticality varchar(255), " +
                                    "Response varchar(255), " +
                                    "Comments varchar(max), " +
                                    "CONSTRAINT PK_RfpNameReqId PRIMARY KEY(RFPName, ReqId) " +
                                 "); ";
            using(var command = new SqlCommand(createTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void createModuleMapTable(SqlConnection connection)
        {
            string createTable = "use RequestForProposal " +
                                 "CREATE TABLE ModuleMap ( " +
                                    "RFPName varchar(255) NOT NULL," +
                                    "ModuleID varchar(255) NOT NULL, " +
                                    "ModuleNameRFP varchar(255), " +
                                    "CONSTRAINT PK_RfpNameModuleId PRIMARY KEY(RFPName, ModuleID), " +
                                    "CONSTRAINT FK_ModuleLookup FOREIGN KEY (ModuleId) REFERENCES ModuleLookup(ModuleId)" + 
                                 "); ";
            using (var command = new SqlCommand(createTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void createResponseMapTable(SqlConnection connection)
        {
            string createTable = "use RequestForProposal " +
                                 "CREATE TABLE ResponseMap ( " +
                                    "RFPName varchar(255) NOT NULL," +
                                    "ResponseMaster varchar(255) NOT NULL, " +
                                    "ResponseRFP varchar(255), " +
                                 "); ";
            using (var command = new SqlCommand(createTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void createPositionMapTable(SqlConnection connection)
        {
            string createTable = "use RequestForProposal " +
                                 "CREATE TABLE PositionMap ( " +
                                    "RFPName varchar(255) NOT NULL," +
                                    "SheetName varchar(255) NOT NULL, " +
                                    "ModuleId varchar(255) NOT NULL, " +
                                    "Requirement varchar(255), " +
                                    "Response varchar(255), " +
                                    "Comments varchar(max), " +
                                    "SkipRows varchar(255), " +
                                    "CONSTRAINT PK_RfpNameSheetName PRIMARY KEY(RFPName, SheetName, ModuleId) " +
                                 "); ";
            using (var command = new SqlCommand(createTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void createModuleLookupTable(SqlConnection connection)
        {
            string createTable = "use RequestForProposal " +
                                 "CREATE TABLE ModuleLookup ( " +
                                    "ModuleId varchar(255) NOT NULL," +
                                    "Name varchar(255) NOT NULL, " +
                                    "CONSTRAINT PK_ModuleId PRIMARY KEY(ModuleId) " +
                                 "); ";
            string insertValues = "INSERT INTO ModuleLookup " +
                                 "(ModuleId, Name)" +
                                 "VALUES ";
            var map = new Dictionary<string, string>()
            {
                { "AP" , "Accounts payable" },
                { "AR" , "Accounts receivable" },
                { "GL" , "General ledger" },
                { "TEST", "TEST" }
            };

            foreach (var key in map)
            {
                insertValues += $"('{key.Key}','{key.Value}'), ";
            }
            insertValues = insertValues.Remove(insertValues.Length - 2);
            using (var command = new SqlCommand(createTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SqlCommand(insertValues, connection))
            {
                int recordsAffectd = command.ExecuteNonQuery();
            }

        }

        private static void createResponseLookupTable(SqlConnection connection)
        {
            string createTable = "use RequestForProposal " +
                                 "CREATE TABLE ResponseLookup ( " +
                                    "ResponseId varchar(255) NOT NULL," +
                                    "Description varchar(255) NOT NULL, " +
                                    "CONSTRAINT PK_ResponseId PRIMARY KEY(ResponseId) " +
                                 "); ";
            string insertValues = "INSERT INTO ResponseLookup " +
                                 "(ResponseId, Description)" +
                                 "VALUES ";
            var map = new Dictionary<string, string>()
            {
                { "S" , "Standard" },
                { "F" , "Future" },
                { "C" , "Customization" },
                { "T" , "Third party" },
                { "N" , "No" }
            };
            
            foreach (var key in map)
            {
                insertValues += $"('{key.Key}','{key.Value}'), ";
            }
            insertValues = insertValues.Remove(insertValues.Length - 2);
            using (var command = new SqlCommand(createTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SqlCommand(insertValues, connection))
            {
                int recordsAffectd = command.ExecuteNonQuery();
            }
        }

        public static int executeCommand(string sql)
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            int ret = 0;
            using (var conn = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    ret = command.ExecuteNonQuery();
                }
            }

            return ret;
        }

        public static void saveRFPtoDB(ExcelManager manager, string rfpName)
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            string insert = "insert into MasterRFP " +
                                   "(RFPName, ModuleId, ReqId, Criticality, Response, Comments)" +
                                   " values " +
                                   "(@RFPName, @ModuleId, @ReqId, @Criticality, @Response, @Comments)";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var command = new SqlCommand(insert, conn))
                {
                    while (manager.nextRequirement())
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@RFPName", rfpName);
                        command.Parameters.AddWithValue("@ModuleId", manager.moduleId);
                        command.Parameters.AddWithValue("@ReqId", manager.requirement);
                        command.Parameters.AddWithValue("@Criticality", manager.criticality != null ? manager.criticality : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Response", manager.response != null ? manager.response : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Comments", manager.comments != null ? manager.comments : (object)DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void populateModuleColumn(System.Windows.Forms.DataGridViewComboBoxColumn dgvCB)
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            string selectModules = "select *" +
                            "from ModuleLookup";

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var command = new SqlCommand(selectModules, conn))
                {
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        dgvCB.Items.Add(reader["ModuleId"].ToString());
                    }

                    reader.Close();
                }
            }
        }

        public static void populateModuleCell(System.Windows.Forms.DataGridViewComboBoxCell dgvCB)
        {
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            string selectModules = "select *" +
                            "from ModuleLookup";

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var command = new SqlCommand(selectModules, conn))
                {
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        dgvCB.Items.Add(reader["ModuleId"].ToString());
                        dgvCB.Value = reader["ModuleId"].ToString();
                    }

                    reader.Close();
                }
            }
        }
    }
}
