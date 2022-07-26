using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace RFPBuilder
{
    public partial class RFPForm : Form
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")] public const int WM_NCLBUTTONDOWN = 0xA1;
        [SuppressMessage("ReSharper", "InconsistentNaming")] public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public string RFPName { get; set; }
        private readonly DataSet _ds;
        private readonly string _viewRfpMember;

        public RFPForm(string rfpName)
        {
            InitializeComponent();

            RFPName = rfpName;


            (_ds, _viewRfpMember) = DBHandler.GetRFP(RFPName);
            RFPGrid.DataSource = _ds.Tables[0].DefaultView;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DBHandler.UpdateRFP(_ds);
            this.Close();
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void RFPGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e)
        {
            _ds.Tables[0].DefaultView.RowFilter = RFPGrid.FilterString;
        }

        private void RFPGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e)
        {
            _ds.Tables[0].DefaultView.Sort = RFPGrid.SortString;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void RFPGrid_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == 0)
            {
                return;
            }
            if (this.CheckDuplicateResponse(e.RowIndex))
            {
                RFPGrid.Rows[e.RowIndex].ErrorText = "Multiple responses for one requirement is not allowed";
                e.Cancel = true;
            }

            if (!e.Cancel)
            {
                RFPGrid.Rows[e.RowIndex].ErrorText = "";
            }
        }

        private bool CheckDuplicateResponse(int currentRow)
        {
            var rfpName = Convert.ToString(RFPGrid.Rows[currentRow].Cells["RFPName"].Value);
            var reqId = Convert.ToString(RFPGrid.Rows[currentRow].Cells["ReqId"].Value);
            var moduleId = Convert.ToString(RFPGrid.Rows[currentRow].Cells["ModuleId"].Value);

            for (var rowToCompare = 0; rowToCompare < _ds.Tables[_viewRfpMember].Rows.Count; rowToCompare++)
            {
                var rfpNameToCompare = Convert.ToString(RFPGrid.Rows[rowToCompare].Cells["RFPName"].Value);
                var reqIdToCompare = Convert.ToString(RFPGrid.Rows[rowToCompare].Cells["ReqId"].Value);
                var moduleIdToCompare = Convert.ToString(RFPGrid.Rows[rowToCompare].Cells["ModuleId"].Value);

                if (reqId == reqIdToCompare && moduleId == moduleIdToCompare && rfpName == rfpNameToCompare && currentRow != rowToCompare)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
