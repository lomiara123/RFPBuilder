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
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public string RFPName { get; set; }
        DataSet ds;

        public RFPForm(string rfpName)
        {
            InitializeComponent();

            RFPName = rfpName;
            string viewRFPMember;

            (ds, viewRFPMember) = DBHandler.getRFP(RFPName);
            RFPGrid.DataSource = ds.Tables[0].DefaultView;
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

        private void RFPGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e)
        {
            ds.Tables[0].DefaultView.RowFilter = RFPGrid.FilterString;
        }

        private void RFPGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e)
        {
            ds.Tables[0].DefaultView.Sort = RFPGrid.SortString;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
