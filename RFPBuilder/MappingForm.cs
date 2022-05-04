using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RFPBuilder
{
    public partial class MappingForm : Form
    {
        public string RFPName { get; set; }
        SqlDataAdapter modulesDataAdapter, responsesDataAdapter, positionDataAdapter, adapter;
        SqlCommandBuilder sqlModulesCommandBuilder;
        SqlConnection connection;
        DataSet ds;

        public MappingForm()
        {
            InitializeComponent();

            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            string sqlModuleMap = "select * " +
                          "from ModuleMap";
            string sqlResponseMap = "select * " +
                          "from ResponseMap";
            string sqlPositionMap = "select * " +
                          "from PositionMap";

            string sql = "select * from ModuleMap; select * from ResponseMap; select * from PositionMap";

            connection = new SqlConnection(connectionString);
            ds = new DataSet();

            connection.Open();

            adapter = new SqlDataAdapter(sql, connection);
            
            adapter.TableMappings.Add("ModuleMap", "Modules");
            adapter.TableMappings.Add("ResponseMap", "Responses");
            adapter.TableMappings.Add("PositionMap", "Positions");


            adapter.Fill(ds);
            /*
            modulesDataAdapter = new SqlDataAdapter(sqlModuleMap, connection);
            modulesDataAdapter.Fill(ds, "Modules");

            responsesDataAdapter = new SqlDataAdapter(sqlResponseMap, connection);
            responsesDataAdapter.Fill(ds, "Responses");

            positionDataAdapter = new SqlDataAdapter(sqlPositionMap, connection);
            positionDataAdapter.Fill(ds, "Positions");
            */
            sqlModulesCommandBuilder = new SqlCommandBuilder(adapter);
            
            ModulesMapGrid.DataSource = ds;
            ModulesMapGrid.DataMember = "Table";

            ResponsesGrid.DataSource = ds;
            ResponsesGrid.DataMember = "Table1";

            PositionMapGrid.DataSource = ds;
            PositionMapGrid.DataMember = "Table2";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            adapter.UpdateCommand = sqlModulesCommandBuilder.GetUpdateCommand();
            adapter.Update(ds, "Table");
            adapter.Update(ds, "Table1");
            adapter.Update(ds, "Table2");
            this.Close();
        }
    }
}
