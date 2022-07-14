using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace RFPBuilder
{
    class DBHandler
    {
        private static SqlCommandBuilder moduleBuilder, responseBuilder, positionBuilder, viewRFPBuilder;
        private static SqlDataAdapter moduleAdapter, responseAdapter, positionAdapter, viewRFPAdapter;
        private static readonly string DB_CONNECTION_MASTER = DBHandler.initMasterConnectionString();
        private static readonly string DB_CONNECTION_RFP = DBHandler.initDatabaseConnectionString();
        private const string moduleMember = "Module";
        private const string responseMember = "Response";
        private const string positionMember = "Position";
        private const string viewRFPMember = "RFP";
        private static SqlConnection connection;
        private const string configFilename = "app.config";
        private static string initMasterConnectionString() {
            var path = Path.Combine(Directory.GetCurrentDirectory(), configFilename);
            if (File.Exists(path)) {
                string[] configurations = File.ReadAllLines(path);
                foreach (string config in configurations) {
                    if (config.Split('=').Length == 1) {
                        continue;
                    }
                    string propetyName = config.Split('=')[0];
                    string propertyValue = config.Split('=')[1];
                    if (propetyName.ToUpper().Equals("SERVERNAME")) {
                        return String.Format(@"Server={0};Integrated security=SSPI;database=master", propertyValue);
                    }
                }
            }
            return "";
        }
        private static string initDatabaseConnectionString() {
            var path = Path.Combine(Directory.GetCurrentDirectory(), configFilename);
            if (File.Exists(path)) {
                string[] configurations = File.ReadAllLines(path);
                foreach (string config in configurations) {
                    if (config.Split('=').Length == 1) {
                        continue;
                    }
                    string propetyName = config.Split('=')[0];
                    string propertyValue = config.Split('=')[1];
                    if (propetyName.ToUpper().Equals("SERVERNAME")) {
                        return String.Format(@"Server={0};Integrated security=SSPI;database=RequestForProposal", propertyValue);
                    }
                }
            }
            return "";
        }

        public static void deleteDB() {
            string commandSTR = "alter database RequestForProposal set single_user with rollback immediate";
            using (var conn = new SqlConnection(DB_CONNECTION_MASTER)) {
                conn.Open();
                using (var command = new SqlCommand(commandSTR, conn)) {
                    command.ExecuteNonQuery();
                }

                using (var command = new SqlCommand($"DROP DATABASE RequestForProposal", conn)) {
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public static bool checkDatabaseExist() {   
            using (var conn = new SqlConnection(DB_CONNECTION_MASTER)) {
                using (var command = new SqlCommand("SELECT db_id('RequestForProposal')", conn)) {
                    conn.Open();
                    return (command.ExecuteScalar() != DBNull.Value);
                }
            }
        }

        public static bool checkModuleMappingExist(string rfpName) {
            string sql = "select * " +
                         "from ModuleMap " +
                         "where RFPName = @rfpName";

            using (var conn = new SqlConnection(DB_CONNECTION_RFP)) {
                conn.Open();

                using (var command = new SqlCommand(sql, conn)) {
                    command.Parameters.AddWithValue("@rfpName", rfpName);

                    using (var reader = command.ExecuteReader()) {
                        return reader.HasRows;
                    }
                }
            }
        }

        public static bool checkResponseMappingExist(string rfpName) {
            string sql = "select * " +
                         "from ResponseMap " +
                         "where RFPName = @rfpName";

            using (var conn = new SqlConnection(DB_CONNECTION_RFP)) {
                conn.Open();

                using (var command = new SqlCommand(sql, conn)) {
                    command.Parameters.AddWithValue("@rfpName", rfpName);

                    using (var reader = command.ExecuteReader()) {
                        return reader.HasRows;
                    }
                }
            }
        }

        public static bool checkPositionMappingExist(string rfpName) {
            string sql = "select * " +
                         "from PositionMap " +
                         "where RFPName = @rfpName";

            using (var conn = new SqlConnection(DB_CONNECTION_RFP)) {
                conn.Open();
                using (var command = new SqlCommand(sql, conn)) {
                    command.Parameters.AddWithValue("@rfpName", rfpName);

                    using (var reader = command.ExecuteReader()) {
                        return reader.HasRows;
                    }
                }
            }
        }

        public static void initModuleMapping(string rfpName) {
            string selectModules = "select * " +
                                   "from ModuleLookup";
            string insertModuleMapping = "insert into ModuleMap" +
                                         "(RFPName, ModuleID) " +
                                         "VALUES ";
            using (var conn = new SqlConnection(DB_CONNECTION_RFP)) {
                conn.Open();

                using (var command = new SqlCommand(selectModules, conn)) {
                    var reader = command.ExecuteReader();

                    while(reader.Read()) {
                        string tmp = "('" + rfpName + "', '" + reader["ModuleId"].ToString() + "'),";

                        insertModuleMapping += tmp;
                    }

                    insertModuleMapping = insertModuleMapping.Remove(insertModuleMapping.Length - 1);

                    reader.Close();
                }

                using (var command = new SqlCommand(insertModuleMapping, conn)) {
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void initResponseMapping(string rfpName) {
            string selectModules = "select * " +
                                   "from ResponseLookup";
            string insertModuleMapping = "insert into ResponseMap" +
                                         "(RFPName, ResponseMaster) " +
                                         "VALUES ";
            using (var conn = new SqlConnection(DB_CONNECTION_RFP)) {
                conn.Open();

                using (var command = new SqlCommand(selectModules, conn)) {
                    var reader = command.ExecuteReader();

                    while (reader.Read()) {
                        string tmp = "('" + rfpName + "', '" + reader["ResponseId"].ToString() + "'),";

                        insertModuleMapping += tmp;
                    }

                    insertModuleMapping = insertModuleMapping.Remove(insertModuleMapping.Length - 1);

                    reader.Close();
                }

                using (var command = new SqlCommand(insertModuleMapping, conn)) {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void initPositionMapping(string rfpName, string pathToRFP) {
            string insertPositionMapping = "insert into PositionMap" +
                                            "(RFPName, SheetName, ModuleId) " +
                                            "values ";
            
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(pathToRFP);

            foreach(Excel.Worksheet xlWorkSheet in xlWorkBook.Worksheets) {
                insertPositionMapping += "('" + rfpName + "', '" + xlWorkSheet.Name + "', ''),";    
            }
            if (pathToRFP != "") {
                insertPositionMapping = insertPositionMapping.Remove(insertPositionMapping.Length - 1);

                using (var conn = new SqlConnection(DB_CONNECTION_RFP)) {
                    conn.Open();

                    using (var command = new SqlCommand(insertPositionMapping, conn)) {
                        command.ExecuteNonQuery();
                    }
                }
            }
            
        }

        public static void createDB() {
            string createDB = "CREATE DATABASE RequestForProposal";
            using (var conn = new SqlConnection(DB_CONNECTION_MASTER)) {
                using (var command = new SqlCommand(createDB, conn)) {
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
        
        private static void createMasterRFPTable(SqlConnection connection) {
            string createTable = "use RequestForProposal " + 
                                 "CREATE TABLE MasterRFP ( " +
                                    "RFPName varchar(255) NOT NULL," +
                                    "ModuleID varchar(255), " +
                                    "ReqID varchar(8000) NOT NULL, " +
                                    "Criticality varchar(255), " +
                                    "Response varchar(255), " +
                                    "Comments varchar(1000), " +
                                    "CONSTRAINT PK_RfpNameReqId PRIMARY KEY(RFPName, ReqId)" +
                                 "); ";
            using(var command = new SqlCommand(createTable, connection)) {
                command.ExecuteNonQuery();
            }
        }

        private static void createModuleMapTable(SqlConnection connection) {
            string createTable = "use RequestForProposal " +
                                 "CREATE TABLE ModuleMap ( " +
                                    "RFPName varchar(255) NOT NULL," +
                                    "ModuleID varchar(255) NOT NULL, " +
                                    "ModuleNameRFP varchar(255), " +
                                    "CONSTRAINT PK_RfpNameModuleId PRIMARY KEY(RFPName, ModuleID), " +
                                    "CONSTRAINT FK_ModuleLookup FOREIGN KEY (ModuleId) REFERENCES ModuleLookup(ModuleId)" + 
                                 "); ";
            using (var command = new SqlCommand(createTable, connection)) {
                command.ExecuteNonQuery();
            }
        }

        private static void createResponseMapTable(SqlConnection connection) {
            string createTable = "use RequestForProposal " +
                                 "CREATE TABLE ResponseMap ( " +
                                    "RFPName varchar(255) NOT NULL," +
                                    "ResponseMaster varchar(255) NOT NULL, " +
                                    "ResponseRFP varchar(255), " +
                                    "CONSTRAINT PK_RfpNameResponse PRIMARY KEY(RFPName, ResponseMaster)," +
                                    "CONSTRAINT FK_ResponseLookup FOREIGN KEY (ResponseMaster) REFERENCES ResponseLookup(ResponseId) " +
                                 "); ";
            using (var command = new SqlCommand(createTable, connection)) {
                command.ExecuteNonQuery();
            }
        }

        private static void createPositionMapTable(SqlConnection connection) {
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
            using (var command = new SqlCommand(createTable, connection)) {
                command.ExecuteNonQuery();
            }
        }
        
        private static void createModuleLookupTable(SqlConnection connection) {
            string createTable = "use RequestForProposal " +
                                 "CREATE TABLE ModuleLookup ( " +
                                    "ModuleId varchar(255) NOT NULL," +
                                    "Name varchar(255) NOT NULL, " +
                                    "CONSTRAINT PK_ModuleId PRIMARY KEY(ModuleId) " +
                                 "); ";
            string insertValues = "INSERT INTO ModuleLookup " +
                                 "(ModuleId, Name)" +
                                 "VALUES ";
            var map = new Dictionary<string, string>() {
                { "AP" , "Accounts payable" },
                { "AR" , "Accounts receivable" },
                { "GL" , "General ledger" },
                { "TEST", "TEST" }
            };

            foreach (var key in map) {
                insertValues += $"('{key.Key}','{key.Value}'), ";
            }
            insertValues = insertValues.Remove(insertValues.Length - 2);

            using (var command = new SqlCommand(createTable, connection)) {
                command.ExecuteNonQuery();
            }
            /*
            using (var command = new SqlCommand(insertValues, connection)) {
                int recordsAffectd = command.ExecuteNonQuery();
            }
            */
        }
        
        private static void createResponseLookupTable(SqlConnection connection) {
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
            
            foreach (var key in map) {
                insertValues += $"('{key.Key}','{key.Value}'), ";
            }
            insertValues = insertValues.Remove(insertValues.Length - 2);

            using (var command = new SqlCommand(createTable, connection)) {
                command.ExecuteNonQuery();
            }
            using (var command = new SqlCommand(insertValues, connection)) {
                int recordsAffectd = command.ExecuteNonQuery();
            }
        }

        public static void saveRequirementsToDb(string filePath, string RFPName) {
            string insert = "if not exists (select * from MasterRFP " +
                                                "where RFPName = @RFPName and ModuleId = @ModuleId and ReqId = @ReqId) " +
                            "begin " +
                                "insert into MasterRFP " +
                                    "(RFPName, ModuleId, ReqId, Criticality, Response, Comments)" +
                                    " values(@RFPName, @ModuleId, @ReqId, @Criticality, @Response, @Comments) " +
                            "end " +
                            "else " +
                            "begin " +
                                "update MasterRFP set " +
                                    "Criticality = @Criticality, " +
                                    "Response = @Response, " +
                                    "Comments = @Comments " +
                                "where RFPName = @RFPName and ModuleId = @ModuleId and ReqId = @ReqId " +
                            "end";
            if (filePath == null || filePath == "")
                return;
            using (var conn = new SqlConnection(DB_CONNECTION_RFP)) {
                conn.Open();
                using (var transaction = conn.BeginTransaction()) {
                    using (var command = new SqlCommand(insert, conn)) {
                        command.Transaction = transaction;
                        command.Parameters.Add(new SqlParameter("@RFPName", SqlDbType.VarChar));
                        command.Parameters.Add(new SqlParameter("@ModuleId", SqlDbType.VarChar));
                        command.Parameters.Add(new SqlParameter("@ReqId", SqlDbType.VarChar));
                        command.Parameters.Add(new SqlParameter("@Criticality", SqlDbType.VarChar));
                        command.Parameters.Add(new SqlParameter("@Response", SqlDbType.VarChar));
                        command.Parameters.Add(new SqlParameter("@Comments", SqlDbType.VarChar));

                        using (RFPDocument rfpDocument = new RFPDocument(filePath, RFPName)) {
                            try {
                                foreach (var module in rfpDocument) {
                                    foreach (var requirement in module) {
                                        command.Parameters[0].Value = RFPName;
                                        command.Parameters[1].Value = module.ModuleId;
                                        command.Parameters[2].Value = requirement.Id;
                                        command.Parameters[3].Value = requirement.Criticality != null ? requirement.Criticality : (object)DBNull.Value;
                                        command.Parameters[4].Value = requirement.Response != null ? DBHandler.getMasterResponse(RFPName, requirement.Response) : (object)DBNull.Value;
                                        command.Parameters[5].Value = requirement.Comments != null ? requirement.Comments : (object)DBNull.Value;
                                        command.ExecuteNonQuery();
                                    }
                                }
                                transaction.Commit();
                            } catch (Exception ex) {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
            }
        }

        public static DataTable getPositionMap(string RFPName) {
            string selectModules = "select * " +
                            "from PositionMap " +
                            "where RFPName = '" + RFPName + "'";
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(DB_CONNECTION_RFP)) {
                conn.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(selectModules, conn);
                adapter.Fill(dt);
            }

            return dt;
        }

        public static (DataSet, string, string, string) getMapping(string RFPName) {
            DataSet ds = new DataSet();
            string selectModulesStr, selectResponsesStr, selectPositionStr;

            if (RFPName != "") {
                selectModulesStr = "select * from ModuleMap where RFPName = @RFPName;";
                selectResponsesStr = "select RFPName as [RFP name], ResponseMaster as [Master response indicator], ResponseRFP as [Customer response indicator] from ResponseMap where RFPName = @RFPName;";
                selectPositionStr = "select * from PositionMap where RFPName = @RFPName;";
            }
            else {
                selectModulesStr = "select * from ModuleMap;";
                selectResponsesStr = "select RFPName as [RFP name], ResponseMaster as [Master response indicator], ResponseRFP as [Customer response indicator] from ResponseMap;";
                selectPositionStr = "select * from PositionMap;";
            }

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

            moduleAdapter.Fill(ds, moduleMember);
            responseAdapter.Fill(ds, responseMember);
            positionAdapter.Fill(ds, positionMember);

            moduleBuilder = new SqlCommandBuilder(moduleAdapter);
            responseBuilder = new SqlCommandBuilder(responseAdapter);
            positionBuilder = new SqlCommandBuilder(positionAdapter);

            return (ds, moduleMember, responseMember, positionMember);
        }

        public static (DataSet, string) getRFP(string RFPName) {
            DataSet ds = new DataSet();
            string selectRFPStr;

            if(RFPName != "") {
                selectRFPStr = "select * from MasterRFP where RFPName = @RFPName";
            }
            else {
                selectRFPStr = "select * from MasterRFP";
            }

            connection = new SqlConnection(DB_CONNECTION_RFP);
            connection.Open();

            SqlCommand selectRFP = new SqlCommand(selectRFPStr, connection);
            selectRFP.Parameters.AddWithValue("@RFPName", RFPName);

            viewRFPAdapter = new SqlDataAdapter(selectRFP);
            viewRFPAdapter.Fill(ds, viewRFPMember);

            viewRFPBuilder = new SqlCommandBuilder(viewRFPAdapter);

            return(ds, viewRFPMember);
        }

        public static void updateMapping(DataSet ds) {
            moduleAdapter.UpdateCommand = moduleBuilder.GetUpdateCommand();
            responseAdapter.UpdateCommand = responseBuilder.GetUpdateCommand();
            positionAdapter.UpdateCommand = positionBuilder.GetUpdateCommand();
            moduleAdapter.Update(ds, moduleMember);
            responseAdapter.Update(ds, responseMember);
            positionAdapter.Update(ds, positionMember);
        }

        public static void updateRFP(DataSet ds) {
            viewRFPAdapter.UpdateCommand = viewRFPBuilder.GetUpdateCommand();
            viewRFPAdapter.Update(ds, viewRFPMember);
        }

        public static (string response, string comments) getRequirement(string RFPName, string ModuleId ,string ReqID ) {
            string sql = "select * from MasterRFP where ReqId = @ReqId and ModuleId = @ModuleId";
            using (var con = new SqlConnection(DB_CONNECTION_RFP)) {
                con.Open();
                using (var command = new SqlCommand(sql, con))  {
                    command.Parameters.AddWithValue("@ReqId", ReqID);
                    command.Parameters.AddWithValue("@ModuleId", ModuleId);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows) {
                        reader.Read();
                        string response = DBHandler.getCustResponse(RFPName, reader["Response"].ToString());
                        return (response, reader["Comments"].ToString());
                    }
                }
            }

            return ("", "");
        }

        public static string getCustResponse(string RFPName, string Response) {
            string sql = "select * from ResponseMap where RFPName = @RFPName and ResponseMaster = @Response";

            using (var con = new SqlConnection(DB_CONNECTION_RFP)) {
                con.Open();
                using (var command = new SqlCommand(sql, con)) {
                    command.Parameters.AddWithValue("@RFPName", RFPName);
                    command.Parameters.AddWithValue("@Response", Response);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows) {
                        reader.Read();
                        return reader["ResponseRFP"].ToString();
                    }
                }
            }

            return Response;
        }

        public static string getMasterResponse(string RFPName, string Response)
        {
            string sql = "select * from ResponseMap where RFPName = @RFPName and ResponseRFP = @Response";

            using (var con = new SqlConnection(DB_CONNECTION_RFP))
            {
                con.Open();
                using (var command = new SqlCommand(sql, con))
                {
                    command.Parameters.AddWithValue("@RFPName", RFPName);
                    command.Parameters.AddWithValue("@Response", Response);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return reader["ResponseMaster"].ToString();
                    }
                }
            }

            return Response;
        }

        public static string getResponseDescription(string responseId) {
            string sql = "select * from ResponseLookup where ResponseId = @ResponseId";
            string description = "";
            using (var con = new SqlConnection(DB_CONNECTION_RFP)) {
                con.Open();
                using (var command = new SqlCommand(sql, con)) {
                    command.Parameters.AddWithValue("@ResponseId", responseId);

                    SqlDataReader reader = command.ExecuteReader();
                    if(reader.HasRows) {
                        reader.Read();
                        description = reader["Description"].ToString();
                    }
                }
            }

            return description;
        }

        public static bool checkResponseExists(string responseId) {
            string sql = "select * from ResponseLookup where ResponseId = @ResponseId";
            using (var con = new SqlConnection(DB_CONNECTION_RFP)) {
                con.Open();
                using (var command = new SqlCommand(sql, con)) {
                    command.Parameters.AddWithValue("@ResponseId", responseId);

                    SqlDataReader reader = command.ExecuteReader();

                    return reader.HasRows;
                }
            }
        }
    }
}
