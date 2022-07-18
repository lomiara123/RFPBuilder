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
using System.Text.RegularExpressions;

namespace RFPBuilder
{
    public partial class MappingForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public string RFPName { get; set; }
        DataSet ds;

        public MappingForm(string rfpName) {
            InitializeComponent();

            RFPName = rfpName;
            string moduleMember, responseMember, positionMember;
            
            (ds, moduleMember, responseMember, positionMember) = DBHandler.getMapping(RFPName);

            ModulesMapGrid.DataSource = ds.Tables[0].DefaultView;
            ResponsesGrid.DataSource = ds.Tables[1].DefaultView;
            PositionMapGrid.DataSource = ds.Tables[2].DefaultView;

            Mapping.TabPages.RemoveAt(0);

            responseDescriptionTextBox.Enabled = false;
        }

        private void btnClose_Click(object sender, EventArgs e) {
            DBHandler.updateMapping(ds);
            this.Close();
        }

        private void buttonMinimize_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }

        private void PositionMapGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e) {
            ds.Tables[2].DefaultView.RowFilter = PositionMapGrid.FilterString;
        }

        private void PositionMapGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e) {
            ds.Tables[2].DefaultView.Sort = PositionMapGrid.SortString;
        }

        private void ResponsesGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e) {
            ds.Tables[1].DefaultView.RowFilter = PositionMapGrid.FilterString;
        }

        private void ResponsesGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e) {
            ds.Tables[1].DefaultView.Sort = ResponsesGrid.SortString;
        }

        private void ModulesMapGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e) {
            ds.Tables[0].DefaultView.RowFilter = ModulesMapGrid.FilterString;
        }

        private void ModulesMapGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e) {
            ds.Tables[0].DefaultView.Sort = ModulesMapGrid.SortString;
        }
        /// <summary>
        ///     Set response description text
        /// </summary>
        private void ResponsesGrid_RowEnter(object sender, DataGridViewCellEventArgs e) {
            Int32 selectedCellCount = ResponsesGrid.GetCellCount(DataGridViewElementStates.Selected);

            if (selectedCellCount > 0)
            {
                int selectedRow = ResponsesGrid.SelectedCells[0].RowIndex;
                if (ResponsesGrid.Rows[selectedRow].Cells["Master response indicator"].Value != null)
                {
                    string response = ResponsesGrid.Rows[selectedRow].Cells["Master response indicator"].Value.ToString();
                    responseDescriptionTextBox.Text = DBHandler.getResponseDescription(response);
                }
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void ResponsesGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
            string headerText = ResponsesGrid.Columns[e.ColumnIndex].HeaderText;

            if (ResponsesGrid.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            if (headerText.Equals("Master response indicator") && 
                !DBHandler.checkResponseExists(e.FormattedValue.ToString())) {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "Master response is incorrect";
                e.Cancel = true;
            }

            if (headerText.Equals("RFP name") && 
                string.IsNullOrEmpty(e.FormattedValue.ToString())) {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "RFP name must not be empty";
                e.Cancel= true;
            }

            if (!e.Cancel) {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "";
            }
        }


        private void ResponsesGrid_RowValidating(object sender, DataGridViewCellCancelEventArgs e) {
            if (this.checkDuplicateResponse(e.RowIndex)) {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "Multiple mapping for one response is not allowed";
                e.Cancel = true;
            }

            if (!e.Cancel) {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "";
            }
        }

        private bool checkDuplicateResponse(int currentRow)
        {
            string rfpName = ResponsesGrid.Rows[currentRow].Cells["RFP name"].Value.ToString();
            string masterResponse = ResponsesGrid.Rows[currentRow].Cells["Master response indicator"].Value.ToString();

            for (int rowToCompare = 0; rowToCompare < ResponsesGrid.Rows.GetRowCount(DataGridViewElementStates.Visible); rowToCompare++)
            {
                string rfpNameToCompare = ResponsesGrid.Rows[rowToCompare].Cells["RFP name"].Value.ToString();
                string masterResponseToCompare = ResponsesGrid.Rows[rowToCompare].Cells["Master response indicator"].Value.ToString();

                if (currentRow != rowToCompare && rfpName == rfpNameToCompare && masterResponse == masterResponseToCompare)
                {
                    return true;
                }
            }

            return false;
        }

        private void PositionMapGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
            string headerText = PositionMapGrid.Columns[e.ColumnIndex].HeaderText;

            if (ResponsesGrid.Rows[e.RowIndex].IsNewRow)
            {
                return;
            }

            if (headerText.Equals("SkipRows") &&
               !string.IsNullOrEmpty(e.FormattedValue.ToString()) &&
               !checkSkipRowsFormat(e.FormattedValue.ToString())) {
                PositionMapGrid.Rows[e.RowIndex].ErrorText = "Skip rows is formatted incorrectly. Example: 1-4,6,8,9-12";
                e.Cancel = true;
            }

            if (!e.Cancel) {
                PositionMapGrid.Rows[e.RowIndex].ErrorText = "";
            }
        }

        private bool checkSkipRowsFormat(string skipRows) {
            var regex = new Regex(@"((\d+-\d+)*(\d+)*)((,\d+-\d+)*(,\d+)*)*");
            var m = regex.Match(skipRows);
            while (m.Success)
            {
                if (m.Value == skipRows)
                {
                    return true;
                }
                m = m.NextMatch();
            }
            return false;
        }
    }
}
