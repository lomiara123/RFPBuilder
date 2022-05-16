using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace RFPBuilder
{
    class DBHandler
    {
        private static SqlCommandBuilder moduleBuilder, responseBuilder, positionBuilder;
        private static SqlDataAdapter moduleAdapter, responseAdapter, positionAdapter;
        private const string DB_CONNECTION_MASTER = @"Server=localhost;Integrated security=SSPI;database=master";
        private const string DB_CONNECTION_RFP = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
        private static SqlConnection connection;
        public static void deleteDB()
        {
            string commandSTR = "alter database RequestForProposal set single_user with rollback immediate";
            using (var conn = new SqlConnection(DB_CONNECTION_MASTER))
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
            using (var conn = new SqlConnection(DB_CONNECTION_MASTER))
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
            string sql = "select * " +
                          "from ModuleMap " +
                          "where RFPName = @rfpName";

            using (var conn = new SqlConnection(DB_CONNECTION_RFP))
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
            string sql = "select * " +
                          "from ResponseMap " +
                          "where RFPName = @rfpName";

            using (var conn = new SqlConnection(DB_CONNECTION_RFP))
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
            string sql = "select * " +
                          "from PositionMap " +
                          "where RFPName = @rfpName";

            using (var conn = new SqlConnection(DB_CONNECTION_RFP))
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
            string selectModules = "select * " +
                                   "from ModuleLookup";
            string insertModuleMapping = "insert into ModuleMap" +
                                         "(RFPName, ModuleID) " +
                                         "VALUES ";
            using (var conn = new SqlConnection(DB_CONNECTION_RFP))
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
            string selectModules = "select * " +
                                   "from ResponseLookup";
            string insertModuleMapping = "insert into ResponseMap" +
                                         "(RFPName, ResponseMaster) " +
                                         "VALUES ";
            using (var conn = new SqlConnection(DB_CONNECTION_RFP))
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
            string createDB = "CREATE DATABASE RequestForProposal";
            using (var conn = new SqlConnection(DB_CONNECTION_MASTER))
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
                                    "ModuleID varchar(255), " +
                                    "ReqID varchar(8000) NOT NULL, " +
                                    "Criticality varchar(255), " +
                                    "Response varchar(255), " +
                                    "Comments varchar(1000), " +
                                    "CONSTRAINT PK_RfpNameReqId PRIMARY KEY(RFPName, ReqId), " +
                                    "CONSTRAINT FK_ResponseLookupMaster FOREIGN KEY (Response) REFERENCES ResponseLookup(ResponseId) " +
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
                                    "CONSTRAINT PK_RfpNameResponse PRIMARY KEY(RFPName, ResponseMaster)," +
                                    "CONSTRAINT FK_ResponseLookup FOREIGN KEY (ResponseMaster) REFERENCES ResponseLookup(ResponseId) " +
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
                                    "Comments varchar(1000), " +
                                    "SkipRows varchar(255), " +
                                    "Criticality varchar(255), " +
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

        public static void saveRequirementToDb(string rfpName, string moduleid, Requirement requirement)
        {
            string insert = "insert into MasterRFP " +
                                   "(RFPName, ModuleId, ReqId, Criticality, Response, Comments)" +
                                   " values " +
                                   "(@RFPName, @ModuleId, @ReqId, @Criticality, @Response, @Comments)";
            using (var conn = new SqlConnection(DB_CONNECTION_RFP))
            {
                conn.Open();
                using (var command = new SqlCommand(insert, conn))
                {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@RFPName", rfpName);
                        command.Parameters.AddWithValue("@ModuleId", moduleid);
                        command.Parameters.AddWithValue("@ReqId", requirement.Id);
                        command.Parameters.AddWithValue("@Criticality", requirement.Criticality != null ? requirement.Criticality : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Response", requirement.Response != null ? requirement.Response : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Comments", requirement.Comments != null ? requirement.Comments : (object)DBNull.Value);
                        command.ExecuteNonQuery();
                }
            }
        }


        public static void populateModuleCell(System.Windows.Forms.DataGridViewComboBoxCell dgvCB)
        {
            string selectModules = "select *" +
                            "from ModuleLookup";

            using (var conn = new SqlConnection(DB_CONNECTION_RFP))
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

        public static DataTable getPositionMap(string RFPName)
        {
            string selectModules = "select *" +
                            "from PositionMap";
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(DB_CONNECTION_RFP))
            {
                conn.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(selectModules, conn);

                adapter.Fill(dt);
            }

            return dt;
        }

        public static DataSet getMapping(string RFPName)
        {
            DataSet ds = new DataSet();
            string selectModulesStr = "select * from ModuleMap where RFPName = @RFPName;";
            string selectResponsesStr = "select * from ResponseMap where RFPName = @RFPName;";
            string selectPositionStr = "select * from PositionMap where RFPName = @RFPName;";
            connection = new SqlConnection(DB_CONNECTION_RFP);
            connection.Open();

            SqlCommand selectModules = new SqlCommand(selectModulesStr, connection);
            SqlCommand selectResponses = new SqlCommand(selectResponsesStr, connection);
            SqlCommand selectPosition = new SqlCommand(selectPositionStr, connection);

            selectModules.Parameters.AddWithValue("@RFPName", RFPName);
            selectResponses.Parameters.AddWithValue("@RFPName", RFPName);
            selectPosition.Parameters.AddWithValue("@RFPName", RFPName);

            moduleAdapter = new SqlDataAdapter(selectModules);
            responseAdapter = new SqlDataAdapter(selectResponses);
            positionAdapter = new SqlDataAdapter(selectPosition);

            moduleAdapter.Fill(ds, "Module");
            responseAdapter.Fill(ds, "Response");
            positionAdapter.Fill(ds, "Position");

            moduleBuilder = new SqlCommandBuilder(moduleAdapter);
            responseBuilder = new SqlCommandBuilder(responseAdapter);
            positionBuilder = new SqlCommandBuilder(positionAdapter);

            return ds;
        }

        public static void updateMapping(DataSet ds)
        {
            moduleAdapter.UpdateCommand = moduleBuilder.GetUpdateCommand();
            responseAdapter.UpdateCommand = responseBuilder.GetUpdateCommand();
            positionAdapter.UpdateCommand = positionBuilder.GetUpdateCommand();
            moduleAdapter.Update(ds, "Module");
            responseAdapter.Update(ds, "Response");
            positionAdapter.Update(ds, "Position");
        }

        public static (string response, string comments) getRequirement(string ReqID)
        {
            string sql = "select * from MasterRFP where ReqId = @ReqId";

            using (var con = new SqlConnection(DB_CONNECTION_RFP))
            {
                con.Open();

                using (var command = new SqlCommand(sql, con)) 
                {
                    command.Parameters.AddWithValue("@ReqId", ReqID);

                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();

                        string response = DBHandler.getCustResponse(reader["RFPName"].ToString(), reader["Response"].ToString());

                        return (response, reader["Comments"].ToString());
                    }
                }
            }

            return ("", "");
        }

        public static string getCustResponse(string RFPName, string Response)
        {
            string sql = "select * from ResponseMap where RFPName = @RFPName and ResponseMaster = @Response";

            using (var con = new SqlConnection(DB_CONNECTION_RFP))
            {
                con.Open();

                using (var command = new SqlCommand(sql, con))
                {
                    command.Parameters.AddWithValue("@RFPName", RFPName);
                    command.Parameters.AddWithValue("@Response", Response);

                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();

                        return reader["ResponseRFP"].ToString();
                    }
                }
            }

            return Response;
        }
    }
}
