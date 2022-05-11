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
        SqlDataAdapter adapter;
        SqlCommandBuilder sqlModulesCommandBuilder;
        SqlConnection connection;
        DataSet ds;

        public MappingForm()
        {
            InitializeComponent();

            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";

            string sql = "select * from ModuleMap;"   +
                         "select * from ResponseMap;" +
                         "select * from PositionMap;" +
                         "select * from ModuleLookup";

            connection = new SqlConnection(connectionString);
            ds = new DataSet();

            connection.Open();

            adapter = new SqlDataAdapter(sql, connection);
            
            adapter.TableMappings.Add("ModuleMap", "Modules");
            adapter.TableMappings.Add("ResponseMap", "Responses");
            adapter.TableMappings.Add("PositionMap", "Positions");


            adapter.Fill(ds);
            sqlModulesCommandBuilder = new SqlCommandBuilder(adapter);
            DataGridViewComboBoxColumn dgvCB = new DataGridViewComboBoxColumn();
            dgvCB.Name = "Table3.ModuleId";
            dgvCB.HeaderText = "Table3.ModuleId";
            dgvCB.DataPropertyName = "Table3.ModuleId";
              DBHandler.populateModuleColumn(dgvCB);

            ModulesMapGrid.DataSource = ds;
            ModulesMapGrid.DataMember = "Table";
            ModulesMapGrid.Columns["ModuleId"].Visible = false;
            ModulesMapGrid.Columns.Add(dgvCB);
            ModulesMapGrid.Columns["Table3.ModuleId"].DisplayIndex = 1;
            ModulesMapGrid.Columns["ModuleNameRFP"].DisplayIndex = 2;

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
