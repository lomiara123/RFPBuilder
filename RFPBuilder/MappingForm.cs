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
        SqlDataAdapter dataAdapter;
        SqlCommandBuilder sqlCommandBuilder;
        SqlConnection connection;
        DataSet ds;

        public MappingForm()
        {
            InitializeComponent();
            // this.moduleMapBindingSource.Filter = 'RFPName = SOMEVALUE'

            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            string sql = "select * " +
                          "from ModuleMap where ModuleId = 'GL'";
            DBHandler.initModuleMapping("asd");
            connection = new SqlConnection(connectionString);
            
            dataAdapter = new SqlDataAdapter(sql, connection);
            ds = new DataSet();
            connection.Open();
            dataAdapter.Fill(ds, "TEST");

            sqlCommandBuilder = new SqlCommandBuilder(dataAdapter);
            moduleMapDataGridView.DataSource = ds;
            moduleMapDataGridView.DataMember = "TEST";

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            dataAdapter.UpdateCommand = sqlCommandBuilder.GetUpdateCommand();
            dataAdapter.Update(ds, "TEST");
            this.Close();
        }
    }
}
