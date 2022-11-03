using F23.StringSimilarity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace RFPBuilder
{
    class DBHandler
    {
        private static SqlCommandBuilder _moduleBuilder, _responseBuilder, _positionBuilder, _viewRFPBuilder;
        private static SqlDataAdapter _moduleAdapter, _responseAdapter, _positionAdapter, _viewRFPAdapter;
        private static readonly string DbConnectionMaster = DBHandler.InitMasterConnectionString();
        private static readonly string DbConnectionRFP = DBHandler.InitDatabaseConnectionString();
        private const string ModuleMember = "Module";
        private const string ResponseMember = "Response";
        private const string PositionMember = "Position";
        private const string ViewRFPMember = "RFP";
        private static SqlConnection _connection;
        private const string ConfigFilename = "app.config";
        private static string InitMasterConnectionString()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigFilename);
            if (File.Exists(path))
            {
                var configurations = File.ReadAllLines(path);
                foreach (var config in configurations)
                {
                    if (config.Split('=').Length == 1)
                    {
                        continue;
                    }
                    var propertyName = config.Split('=')[0];
                    var propertyValue = config.Split('=')[1];
                    if (propertyName.ToUpper().Equals("SERVERNAME"))
                    {
                        return $@"Server={propertyValue};Integrated security=SSPI;database=master";
                    }
                }
            }
            return "";
        }
        private static string InitDatabaseConnectionString()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigFilename);
            if (File.Exists(path))
            {
                var configurations = File.ReadAllLines(path);
                foreach (var config in configurations)
                {
                    if (config.Split('=').Length == 1)
                    {
                        continue;
                    }
                    var propertyName = config.Split('=')[0];
                    var propertyValue = config.Split('=')[1];
                    if (propertyName.ToUpper().Equals("SERVERNAME"))
                    {
                        return $@"Server={propertyValue};Integrated security=SSPI;database=RequestForProposal";
                    }
                }
            }
            return "";
        }

        public static void DeleteDb()
        {
            const string commandSTR = "alter database RequestForProposal set single_user with rollback immediate";
            using (var conn = new SqlConnection(DbConnectionMaster))
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

        public static bool CheckDatabaseExist()
        {
            using (var conn = new SqlConnection(DbConnectionMaster))
            {
                using (var command = new SqlCommand("SELECT db_id('RequestForProposal')", conn))
                {
                    conn.Open();
                    return (command.ExecuteScalar() != DBNull.Value);
                }
            }
        }

        public static bool CheckModuleMappingExist(string rfpName)
        {
            const string sql = "select * " +
                               "from ModuleMap " +
                               "where RFPName = @rfpName";

            using (var conn = new SqlConnection(DbConnectionRFP))
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

        public static bool CheckResponseMappingExist(string rfpName)
        {
            const string sql = "select * " +
                               "from ResponseMap " +
                               "where RFPName = @rfpName";

            using (var conn = new SqlConnection(DbConnectionRFP))
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

        public static bool CheckPositionMappingExist(string rfpName)
        {
            const string sql = "select * " +
                               "from PositionMap " +
                               "where RFPName = @rfpName";

            using (var conn = new SqlConnection(DbConnectionRFP))
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

        public static void InitModuleMapping(string rfpName)
        {
            const string selectModules = "select * " +
                                         "from ModuleLookup";
            var insertModuleMapping = "insert into ModuleMap" +
                                      "(RFPName, ModuleID) " +
                                      "VALUES ";
            using (var conn = new SqlConnection(DbConnectionRFP))
            {
                conn.Open();

                using (var command = new SqlCommand(selectModules, conn))
                {
                    var reader = command.ExecuteReader();

                    while (reader.Read())
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
        public static void InitResponseMapping(string rfpName)
        {
            if (string.IsNullOrEmpty(rfpName))
            {
                return;
            }
            const string selectMapCount = "select top 1 RFPName, count(RFPName) as mapCount " +
                                          "from ResponseMap " +
                                          "group by RFPName " +
                                          "order by count(RFPName) desc";
            var rfpToCopyFrom = "";
            using (var conn = new SqlConnection(DbConnectionRFP))
            {
                conn.Open();

                using (var command = new SqlCommand(selectMapCount, conn))
                {
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        rfpToCopyFrom = reader["RFPName"].ToString();
                    }

                    reader.Close();
                }
                if (rfpToCopyFrom == "")
                {
                    InitBlankResponseMapping(conn, rfpName);
                }
                else
                {
                    InitResponseMappingFromExistingMapping(conn, rfpName, rfpToCopyFrom);
                }
            }
        }

        public static void InitResponseMappingFromExistingMapping(SqlConnection connection, string newRFP, string rfpToInitFrom)
        {
            const string selectMap = "select * " +
                                     "from ResponseMap " +
                                     "where RFPName = @Rfpname";
            var insertResponseMapping = "insert into ResponseMap" +
                                        "(RFPName, ResponseMaster, ResponseRFP) " +
                                        "VALUES ";
            using (var command = new SqlCommand(selectMap, connection))
            {
                command.Parameters.AddWithValue("@RFPName", rfpToInitFrom);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        insertResponseMapping += NewInsertLine(newRFP, reader["ResponseMaster"], reader["ResponseRFP"]);
                    }

                    insertResponseMapping = insertResponseMapping.Remove(insertResponseMapping.Length - 1);
                }
                reader.Close();
            }

            using (var command = new SqlCommand(insertResponseMapping, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static void InitBlankResponseMapping(SqlConnection connection, string rfpName)
        {
            const string selectModules = "select * " +
                                         "from ResponseLookup";
            var insertResponseMapping = "insert into ResponseMap" +
                                        "(RFPName, ResponseMaster) " +
                                        "VALUES ";
            using (var command = new SqlCommand(selectModules, connection))
            {
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    insertResponseMapping += NewInsertLine(rfpName, reader["ResponseId"]);
                }

                insertResponseMapping = insertResponseMapping.Remove(insertResponseMapping.Length - 1);

                reader.Close();
            }

            using (var command = new SqlCommand(insertResponseMapping, connection))
            {
                command.ExecuteNonQuery();
            }

        }
        private static string NewInsertLine(params object[] list)
        {
            var ret = list.Aggregate("(", (current, param) => current + ("'" + Convert.ToString(param) + "',"));
            ret = ret.Remove(ret.Length - 1);
            ret += "),";
            return ret;
        }
        public static void InitPositionMapping(string rfpName, string pathToRFP)
        {
            if (string.IsNullOrEmpty(rfpName) || string.IsNullOrEmpty(pathToRFP))
            {
                return;
            }
            const string selectMapCount = "select top 1 RFPName, count(RFPName) as mapCount " +
                                          "from PositionMap " +
                                          "group by RFPName " +
                                          "order by count(RFPName) desc";
            var rfpToCopyFrom = "";

            using (var conn = new SqlConnection(DbConnectionRFP))
            {
                conn.Open();

                using (var command = new SqlCommand(selectMapCount, conn))
                {
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        rfpToCopyFrom = reader["RFPName"].ToString();
                    }

                    reader.Close();
                }
                if (rfpToCopyFrom == "")
                {
                    InitBlankPositionMapping(conn, rfpName, pathToRFP);
                }
                else
                {
                    InitPositionMappingFromExistingMapping(conn, rfpName, rfpToCopyFrom);
                }
            }
        }

        public static void InitPositionMappingFromExistingMapping(SqlConnection connection, string newRFP, string rfpToInitFrom)
        {
            const string selectMap = "select * " +
                                     "from PositionMap " +
                                     "where RFPName = @Rfpname";
            var insertPositionMapping = "insert into PositionMap" +
                                        "(RFPName, SheetName, ModuleId, Requirement, Response, Comments, SkipRows, Criticality) " +
                                        "VALUES ";
            using (var command = new SqlCommand(selectMap, connection))
            {
                command.Parameters.AddWithValue("@RFPName", rfpToInitFrom);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        insertPositionMapping += NewInsertLine(newRFP, reader["SheetName"], reader["ModuleId"], reader["Requirement"],
                            reader["Response"], reader["Comments"], reader["SkipRows"], reader["Criticality"]);
                    }

                    insertPositionMapping = insertPositionMapping.Remove(insertPositionMapping.Length - 1);
                }
                reader.Close();
            }

            using (var command = new SqlCommand(insertPositionMapping, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static void InitBlankPositionMapping(SqlConnection connection, string rfpName, string pathToRFP)
        {
            var insertPositionMapping = "insert into PositionMap" +
                                        "(RFPName, SheetName, ModuleId) " +
                                        "values ";
            var xlApp = new Excel.Application();
            var xlWorkBook = xlApp.Workbooks.Open(pathToRFP);
            xlApp.DisplayAlerts = false;

            foreach (Excel.Worksheet xlWorkSheet in xlWorkBook.Worksheets)
            {
                insertPositionMapping += NewInsertLine(rfpName, xlWorkSheet.Name, "");
            }

            insertPositionMapping = insertPositionMapping.Remove(insertPositionMapping.Length - 1);

            using (var command = new SqlCommand(insertPositionMapping, connection))
            {
                command.ExecuteNonQuery();
            }

            xlWorkBook.Close();
            xlApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
        }

        public static void CreateDb()
        {
            const string createDB = "CREATE DATABASE RequestForProposal";
            using (var conn = new SqlConnection(DbConnectionMaster))
            {
                using (var command = new SqlCommand(createDB, conn))
                {
                    conn.Open();
                    command.ExecuteNonQuery();
                    DBHandler.CreateModuleLookupTable(conn);
                    DBHandler.CreateResponseLookupTable(conn);
                    DBHandler.CreateMasterRFPTable(conn);
                    DBHandler.CreateModuleMapTable(conn);
                    DBHandler.CreateResponseMapTable(conn);
                    DBHandler.CreatePositionMapTable(conn);
                }
            }
        }

        private static void CreateMasterRFPTable(SqlConnection connection)
        {
            const string createTable = "use RequestForProposal " +
                                       "CREATE TABLE MasterRFP ( " +
                                       "RFPName varchar(255) NOT NULL," +
                                       "ModuleID varchar(255), " +
                                       "ReqID varchar(8000) NOT NULL, " +
                                       "Criticality varchar(255), " +
                                       "Response varchar(255), " +
                                       "Comments varchar(1000), " +
                                       "CreatedDatetime datetime2 DEFAULT getdate(), " +
                                       "CreatedBy varchar(100) DEFAULT system_user, " +
                                       "ModifiedDatetime datetime2 DEFAULT getdate(), " +
                                       "ModifiedBy varchar(100) DEFAULT system_user, " +
                                       "CONSTRAINT PK_RfpNameReqId PRIMARY KEY(RFPName, ModuleId, ReqId)" +
                                       "); ";
            const string createAfterUpdateTrigger = "CREATE TRIGGER updateTrigger ON MasterRFP " +
                                                    "AFTER UPDATE " +
                                                    "AS " +
                                                    "BEGIN " +
                                                    "SET NOCOUNT ON; " +
                                                    "UPDATE MasterRFP " +
                                                    "SET ModifiedBy = system_user, ModifiedDatetime = getdate() " +
                                                    "FROM MasterRFP " +
                                                    "INNER JOIN inserted i " +
                                                    "ON MasterRFP.RFPName = i.RFPName and MasterRFP.ModuleID = i.ModuleID and MasterRFP.ReqID = i.ReqID " +
                                                    "END";

            using (var command = new SqlCommand(createTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SqlCommand(createAfterUpdateTrigger, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void CreateModuleMapTable(SqlConnection connection)
        {
            const string createTable = "use RequestForProposal " +
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

        private static void CreateResponseMapTable(SqlConnection connection)
        {
            const string createTable = "use RequestForProposal " +
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

        private static void CreatePositionMapTable(SqlConnection connection)
        {
            const string createTable = "use RequestForProposal " +
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

        private static void CreateModuleLookupTable(SqlConnection connection)
        {
            const string createTable = "use RequestForProposal " +
                                       "CREATE TABLE ModuleLookup ( " +
                                       "ModuleId varchar(255) NOT NULL," +
                                       "Name varchar(255) NOT NULL, " +
                                       "CONSTRAINT PK_ModuleId PRIMARY KEY(ModuleId) " +
                                       "); ";
            var insertValues = "INSERT INTO ModuleLookup " +
                               "(ModuleId, Name)" +
                               "VALUES ";
            var map = new Dictionary<string, string>() {
                { "AP" , "Accounts payable" },
                { "AR" , "Accounts receivable" },
                { "GL" , "General ledger" }
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
        }

        private static void CreateResponseLookupTable(SqlConnection connection)
        {
            const string createTable = "use RequestForProposal " +
                                       "CREATE TABLE ResponseLookup ( " +
                                       "ResponseId varchar(255) NOT NULL," +
                                       "Description varchar(255) NOT NULL, " +
                                       "CONSTRAINT PK_ResponseId PRIMARY KEY(ResponseId) " +
                                       "); ";
            var insertValues = "INSERT INTO ResponseLookup " +
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
                var recordsAffected = command.ExecuteNonQuery();
            }
        }

        public static void SaveRequirementsToDb(string filePath, string RFPName)
        {
            const string insert = "if not exists (select * from MasterRFP " +
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
            if (string.IsNullOrEmpty(filePath))
                return;
            using (var conn = new SqlConnection(DbConnectionRFP))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    using (var command = new SqlCommand(insert, conn))
                    {
                        command.Transaction = transaction;
                        command.Parameters.Add(new SqlParameter("@RFPName", SqlDbType.VarChar));
                        command.Parameters.Add(new SqlParameter("@ModuleId", SqlDbType.VarChar));
                        command.Parameters.Add(new SqlParameter("@ReqId", SqlDbType.VarChar));
                        command.Parameters.Add(new SqlParameter("@Criticality", SqlDbType.VarChar));
                        command.Parameters.Add(new SqlParameter("@Response", SqlDbType.VarChar));
                        command.Parameters.Add(new SqlParameter("@Comments", SqlDbType.VarChar));

                        using (var rfpDocument = new RFPDocument(filePath, RFPName))
                        {
                            try
                            {
                                foreach (var module in rfpDocument)
                                {
                                    foreach (var requirement in module)
                                    {
                                        if (string.IsNullOrEmpty(requirement.Id) ||
                                            string.IsNullOrEmpty(requirement.Response))
                                        {
                                            continue;
                                        }
                                        command.Parameters[0].Value = RFPName;
                                        command.Parameters[1].Value = module.ModuleId;
                                        command.Parameters[2].Value = requirement.Id;
                                        command.Parameters[3].Value = requirement.Criticality != null ? requirement.Criticality : "";
                                        command.Parameters[4].Value = requirement.Response != null ? DBHandler.getMasterResponse(RFPName, requirement.Response) : "";
                                        command.Parameters[5].Value = requirement.Comments != null ? requirement.Comments[0] : "";
                                        command.ExecuteNonQuery();
                                    }
                                }
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                _ = MessageBox.Show("Exception encountered during updating the databse \n Error: " + ex.Message);
                                throw;
                            }
                        }
                    }
                }
            }
        }

        public static DataTable GetPositionMap(string RFPName)
        {
            var selectModules = "select RFPName as [RFP], SheetName as [Sheet], ModuleId as [Module], Requirement, Response, Comments, Criticality, SkipRows as [Skip rows] " +
                                "from PositionMap " +
                                "where RFPName = '" + RFPName + "'";
            var dt = new DataTable();
            using (var conn = new SqlConnection(DbConnectionRFP))
            {
                conn.Open();

                var adapter = new SqlDataAdapter(selectModules, conn);
                adapter.Fill(dt);
            }

            return dt;
        }

        public static (DataSet, string, string, string) GetMapping(string RFPName)
        {
            var ds = new DataSet();
            string selectModulesStr, selectResponsesStr, selectPositionStr;

            if (RFPName != "")
            {
                selectModulesStr = "select * from ModuleMap where RFPName = @RFPName;";
                selectResponsesStr = "select RFPName as [RFP name], ResponseMaster as [Master response indicator], ResponseRFP as [Customer response indicator] from ResponseMap where RFPName = @RFPName;";
                selectPositionStr = "select RFPName as [RFP], SheetName as [Sheet], ModuleId as [Module], Requirement, Response, Comments, Criticality, SkipRows as [Skip rows] from PositionMap where RFPName = @RFPName;";
            }
            else
            {
                selectModulesStr = "select * from ModuleMap;";
                selectResponsesStr = "select RFPName as [RFP name], ResponseMaster as [Master response indicator], ResponseRFP as [Customer response indicator] from ResponseMap;";
                selectPositionStr = "select RFPName as [RFP], SheetName as [Sheet], ModuleId as [Module], Requirement, Response, Comments, Criticality, SkipRows as [Skip rows] from PositionMap;";
            }

            _connection = new SqlConnection(DbConnectionRFP);
            _connection.Open();

            var selectModules = new SqlCommand(selectModulesStr, _connection);
            var selectResponses = new SqlCommand(selectResponsesStr, _connection);
            var selectPosition = new SqlCommand(selectPositionStr, _connection);

            selectModules.Parameters.AddWithValue("@RFPName", RFPName);
            selectResponses.Parameters.AddWithValue("@RFPName", RFPName);
            selectPosition.Parameters.AddWithValue("@RFPName", RFPName);

            _moduleAdapter = new SqlDataAdapter(selectModules);
            _responseAdapter = new SqlDataAdapter(selectResponses);
            _positionAdapter = new SqlDataAdapter(selectPosition);

            _moduleAdapter.Fill(ds, ModuleMember);
            _responseAdapter.Fill(ds, ResponseMember);
            _positionAdapter.Fill(ds, PositionMember);

            _moduleBuilder = new SqlCommandBuilder(_moduleAdapter);
            _responseBuilder = new SqlCommandBuilder(_responseAdapter);
            _positionBuilder = new SqlCommandBuilder(_positionAdapter);

            return (ds, ModuleMember, ResponseMember, PositionMember);
        }

        public static (DataSet, string) GetRFP(string RFPName)
        {
            var ds = new DataSet();
            string selectRFPStr;

            if (!string.IsNullOrEmpty(RFPName))
            {
                selectRFPStr = "select RFPName as [RFP], ModuleId as [Module] , ReqId as [Requirement], Response, Criticality, Comments from MasterRFP where RFPName = @RFPName";
            }
            else
            {
                selectRFPStr = "select RFPName as [RFP], ModuleId as [Module], ReqId as [Requirement], Response, Criticality, Comments from MasterRFP";
            }

            _connection = new SqlConnection(DbConnectionRFP);
            _connection.Open();

            var selectRFP = new SqlCommand(selectRFPStr, _connection);
            selectRFP.Parameters.AddWithValue("@RFPName", RFPName);

            _viewRFPAdapter = new SqlDataAdapter(selectRFP);
            _viewRFPAdapter.Fill(ds, ViewRFPMember);

            _viewRFPBuilder = new SqlCommandBuilder(_viewRFPAdapter);

            return (ds, ViewRFPMember);
        }

        public static void UpdateMapping(DataSet ds)
        {
            _moduleAdapter.UpdateCommand = _moduleBuilder.GetUpdateCommand();
            _responseAdapter.UpdateCommand = _responseBuilder.GetUpdateCommand();
            _positionAdapter.UpdateCommand = _positionBuilder.GetUpdateCommand();
            _moduleAdapter.Update(ds, ModuleMember);
            _responseAdapter.Update(ds, ResponseMember);
            _positionAdapter.Update(ds, PositionMember);
        }
        public static void UpdateRFP(DataSet ds)
        {
            _viewRFPAdapter.UpdateCommand = _viewRFPBuilder.GetUpdateCommand();
            _viewRFPAdapter.Update(ds, ViewRFPMember);
        }


        public static async Task<(string response, List<string> commentsList, bool multipleResponses)> GetRequirement(string RFPName, string ModuleId, string ReqID)
        {
            const string selectRequirementGlobal = "select * from MasterRFP order by ModifiedDatetime asc";
            var multipleResponse = false;
            double lastPrecision = 0;
            var response = "";
            var commentsList = new List<String>();
            using (var con = new SqlConnection(DbConnectionRFP))
            {
                con.Open();
                using (var command = new SqlCommand(selectRequirementGlobal, con))
                {
                    var reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        var currentReq = reader["ReqId"].ToString();
                        var similarity = await ApiManager.findRequirementsSimilarity(ReqID, currentReq);
                        if (similarity > 0.45)
                        {
                            if (lastPrecision != 0 && !response.Equals(reader["Response"].ToString()))
                            {
                                multipleResponse = true;
                            }
                            
                            var comments = reader["Comments"].ToString();
                            if (comments != null && comments != "")
                                commentsList.Add(comments);

                            lastPrecision = similarity;
                            response = reader["Response"].ToString();
                        }
                    }
                }
            }
                return (response, commentsList, multipleResponse);
        }

        public static string GetCustomerResponse(string RFPName, string Response)
        {
            const string sql = "select * from ResponseMap where RFPName = @RFPName and ResponseMaster = @Response";

            using (var con = new SqlConnection(DbConnectionRFP))
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

        public static string getMasterResponse(string RFPName, string Response)
        {
            const string sql = "select * from ResponseMap where RFPName = @RFPName and ResponseRFP = @Response";

            using (var con = new SqlConnection(DbConnectionRFP))
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
                        return reader["ResponseMaster"].ToString();
                    }
                }
            }

            return Response;
        }

        public static string GetResponseDescription(string responseId)
        {
            var sql = "select * from ResponseLookup where ResponseId = @ResponseId";
            var description = "";
            using (var con = new SqlConnection(DbConnectionRFP))
            {
                con.Open();
                using (var command = new SqlCommand(sql, con))
                {
                    command.Parameters.AddWithValue("@ResponseId", responseId);

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        description = reader["Description"].ToString();
                    }
                }
            }

            return description;
        }

        public static bool CheckResponseExists(string responseId)
        {
            const string sql = "select * from ResponseLookup where ResponseId = @ResponseId";
            using (var con = new SqlConnection(DbConnectionRFP))
            {
                con.Open();
                using (var command = new SqlCommand(sql, con))
                {
                    command.Parameters.AddWithValue("@ResponseId", responseId);

                    var reader = command.ExecuteReader();

                    return reader.HasRows;
                }
            }
        }
    }
}
