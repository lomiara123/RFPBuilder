using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFPBuilder
{
    public partial class RFPForm : Form
    {
        public string RFPName { get; set; }
        DataSet ds;

        public RFPForm(string rfpName)
        {
            InitializeComponent();

            RFPName = rfpName;
            string viewRFPMember;

            (ds, viewRFPMember) = DBHandler.getRFP(RFPName);

            RFPGrid.DataSource = ds;
            RFPGrid.DataMember = viewRFPMember;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DBHandler.updateRFP(ds);
            this.Close();
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
