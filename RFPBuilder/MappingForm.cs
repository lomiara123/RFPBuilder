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
        string moduleMember, responseMember, positionMember;

        public string RFPName { get; set; }
        DataSet ds;

        public MappingForm(string rfpName) {
            InitializeComponent();

            RFPName = rfpName;
            
            
            (ds, moduleMember, responseMember, positionMember) = DBHandler.getMapping(RFPName);

            ModulesMapGrid.DataSource = ds.Tables[moduleMember].DefaultView;
            ResponsesGrid.DataSource = ds.Tables[responseMember].DefaultView;
            PositionMapGrid.DataSource = ds.Tables[positionMember].DefaultView;

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
            ds.Tables[positionMember].DefaultView.RowFilter = PositionMapGrid.FilterString;
        }

        private void PositionMapGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e) {
            ds.Tables[positionMember].DefaultView.Sort = PositionMapGrid.SortString;
        }

        private void ResponsesGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e) {
            ds.Tables[responseMember].DefaultView.RowFilter = ResponsesGrid.FilterString;
        }

        private void ResponsesGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e) {
            ds.Tables[responseMember].DefaultView.Sort = ResponsesGrid.SortString;
        }

        private void ModulesMapGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e) {
            ds.Tables[moduleMember].DefaultView.RowFilter = ModulesMapGrid.FilterString;
        }

        private void ModulesMapGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e) {
            ds.Tables[moduleMember].DefaultView.Sort = ModulesMapGrid.SortString;
        }
        /// <summary>
        ///     Set response description text
        /// </summary>
        

        private void panel1_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        

        private void PositionMapGrid_RowValidating(object sender, DataGridViewCellCancelEventArgs e) {
            if (PositionMapGrid.Rows[e.RowIndex].Cells["Module"].Value == DBNull.Value &&
                PositionMapGrid.Rows[e.RowIndex].Cells["RFP"].Value != DBNull.Value) {
                PositionMapGrid.Rows[e.RowIndex].Cells["Module"].Value = "";
            }

            if (!e.Cancel && !string.IsNullOrEmpty(Convert.ToString(PositionMapGrid.Rows[e.RowIndex].Cells["Skip rows"].Value)) &&
               !checkSkipRowsFormat(Convert.ToString(PositionMapGrid.Rows[e.RowIndex].Cells["Skip rows"].Value))) {
                PositionMapGrid.Rows[e.RowIndex].ErrorText = "Skip rows is formatted incorrectly. Example: 1-4,6,8,9-12";
                e.Cancel = true;
            }

            if (!e.Cancel) {
                PositionMapGrid.Rows[e.RowIndex].ErrorText = "";
            }
        }
        

        private void PositionMapGrid_RowValidated(object sender, DataGridViewCellEventArgs e) {
            /*
            if (PositionMapGrid.Rows[e.RowIndex].Cells["RFP"].Value == DBNull.Value &&
                PositionMapGrid.Rows[e.RowIndex].Cells["Sheet"].Value == DBNull.Value) {
                BindingManagerBase bm = PositionMapGrid.BindingContext[PositionMapGrid.DataSource, PositionMapGrid.DataMember];
                ((DataRowView)bm.Current).Row.Delete();
            }
            */
        }

        private void ResponsesGrid_RowEnter(object sender, DataGridViewCellEventArgs e) {
            Int32 selectedCellCount = ResponsesGrid.GetCellCount(DataGridViewElementStates.Selected);

            if (selectedCellCount > 0) {
                int selectedRow = ResponsesGrid.SelectedCells[0].RowIndex;
                if (ResponsesGrid.Rows[selectedRow].Cells["Master response indicator"].Value != null) {
                    string response = ResponsesGrid.Rows[selectedRow].Cells["Master response indicator"].Value.ToString();
                    responseDescriptionTextBox.Text = DBHandler.getResponseDescription(response);
                }
                else {
                    responseDescriptionTextBox.Text = "";
                }
            }
        }

        private void ResponsesGrid_RowValidating(object sender, DataGridViewCellCancelEventArgs e) {
            if (e.RowIndex == 0) {
                return;
            }

            if (!e.Cancel && !DBHandler.checkResponseExists(Convert.ToString(ResponsesGrid.Rows[e.RowIndex].Cells["Master response indicator"].Value))) {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "Master response is incorrect";
                e.Cancel = true;
            }

            if (!e.Cancel && string.IsNullOrEmpty(Convert.ToString(ResponsesGrid.Rows[e.RowIndex].Cells["RFP name"].Value))) {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "RFP name must not be empty";
                e.Cancel = true;
            }

            if (!e.Cancel && this.checkDuplicateResponse(e.RowIndex)) {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "Multiple mapping for one response is not allowed";
                e.Cancel = true;
            }

            if (!e.Cancel) {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "";
            }
        }

        private void ResponsesGrid_RowValidated(object sender, DataGridViewCellEventArgs e) {
            /*
            if (ResponsesGrid.Rows[e.RowIndex].Cells["RFP name"].Value == DBNull.Value ||
                ResponsesGrid.Rows[e.RowIndex].Cells["Master response indicator"].Value == DBNull.Value) {
                BindingManagerBase bm = PositionMapGrid.BindingContext[ResponsesGrid.DataSource, ResponsesGrid.DataMember];
                ((DataRowView)bm.Current).Row.Delete();
            }
            */
        }

        private bool checkSkipRowsFormat(string skipRows) {
            var regex = new Regex(@"((\d+-\d+)*(\d+)*)((,\d+-\d+)*(,\d+)*)*");
            var match = regex.Match(skipRows);
            
            while (match.Success) {
                if (match.Value == skipRows) {
                    return true;
                }
                match = match.NextMatch();
            }
            return false;
        }

        private bool checkDuplicateResponse(int currentRow) {
            string rfpName = ResponsesGrid.Rows[currentRow].Cells["RFP name"].Value.ToString();
            string masterResponse = ResponsesGrid.Rows[currentRow].Cells["Master response indicator"].Value.ToString();

            for (int rowToCompare = 0; rowToCompare < ds.Tables[responseMember].DefaultView.Count; rowToCompare++) {
                string rfpNameToCompare = ResponsesGrid.Rows[rowToCompare].Cells["RFP name"].Value.ToString();
                string masterResponseToCompare = ResponsesGrid.Rows[rowToCompare].Cells["Master response indicator"].Value.ToString();

                if (currentRow != rowToCompare && rfpName == rfpNameToCompare && masterResponse == masterResponseToCompare) {
                    return true;
                }
            }

            return false;
        }
    }
}
