using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RFPBuilder
{
    public partial class MappingForm : Form
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")] public const int WM_NCLBUTTONDOWN = 0xA1;
        [SuppressMessage("ReSharper", "InconsistentNaming")] public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private readonly string _moduleMember;
        private readonly string _responseMember;
        private readonly string _positionMember;

        public string RFPName { get; set; }
        private readonly DataSet _ds;

        public MappingForm(string rfpName)
        {
            InitializeComponent();

            RFPName = rfpName;


            (_ds, _moduleMember, _responseMember, _positionMember) = DBHandler.GetMapping(RFPName);

            ModulesMapGrid.DataSource = _ds.Tables[_moduleMember].DefaultView;
            ResponsesGrid.DataSource = _ds.Tables[_responseMember].DefaultView;
            PositionMapGrid.DataSource = _ds.Tables[_positionMember].DefaultView;

            Mapping.TabPages.RemoveAt(0);

            responseDescriptionTextBox.Enabled = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DBHandler.UpdateMapping(_ds);
            this.Close();
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void PositionMapGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e)
        {
            _ds.Tables[_positionMember].DefaultView.RowFilter = PositionMapGrid.FilterString;
        }

        private void PositionMapGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e)
        {
            _ds.Tables[_positionMember].DefaultView.Sort = PositionMapGrid.SortString;
        }

        private void ResponsesGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e)
        {
            _ds.Tables[_responseMember].DefaultView.RowFilter = ResponsesGrid.FilterString;
        }

        private void ResponsesGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e)
        {
            _ds.Tables[_responseMember].DefaultView.Sort = ResponsesGrid.SortString;
        }

        private void ModulesMapGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e)
        {
            _ds.Tables[_moduleMember].DefaultView.RowFilter = ModulesMapGrid.FilterString;
        }

        private void ModulesMapGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e)
        {
            _ds.Tables[_moduleMember].DefaultView.Sort = ModulesMapGrid.SortString;
        }
        /// <summary>
        ///     Set response description text
        /// </summary>
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }


        private void PositionMapGrid_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (PositionMapGrid.Rows[e.RowIndex].Cells["Module"].Value == DBNull.Value &&
                PositionMapGrid.Rows[e.RowIndex].Cells["RFP"].Value != DBNull.Value &&
                PositionMapGrid.Rows[e.RowIndex].Cells["Sheet"].Value != DBNull.Value)
            {
                PositionMapGrid.Rows[e.RowIndex].Cells["Module"].Value = "";
            }

            if (!e.Cancel && !string.IsNullOrEmpty(Convert.ToString(PositionMapGrid.Rows[e.RowIndex].Cells["Skip rows"].Value)) &&
               !CheckSkipRowsFormat(Convert.ToString(PositionMapGrid.Rows[e.RowIndex].Cells["Skip rows"].Value)))
            {
                PositionMapGrid.Rows[e.RowIndex].ErrorText = "Skip rows is formatted incorrectly. Example: 1-4,6,8,10-12";
                e.Cancel = true;
            }

            if (!e.Cancel)
            {
                PositionMapGrid.Rows[e.RowIndex].ErrorText = "";
            }
        }

        private void ResponsesGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            var selectedCellCount = ResponsesGrid.GetCellCount(DataGridViewElementStates.Selected);

            if (selectedCellCount > 0)
            {
                int selectedRow = ResponsesGrid.SelectedCells[0].RowIndex;
                if (ResponsesGrid.Rows[selectedRow].Cells["Master response indicator"].Value != null)
                {
                    string response = ResponsesGrid.Rows[selectedRow].Cells["Master response indicator"].Value.ToString();
                    responseDescriptionTextBox.Text = DBHandler.GetResponseDescription(response);
                }
                else
                {
                    responseDescriptionTextBox.Text = "";
                }
            }
        }

        private void ResponsesGrid_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == 0)
            {
                return;
            }

            if (!e.Cancel && !DBHandler.CheckResponseExists(Convert.ToString(ResponsesGrid.Rows[e.RowIndex].Cells["Master response indicator"].Value)))
            {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "Master response is incorrect";
                e.Cancel = true;
            }

            if (!e.Cancel && string.IsNullOrEmpty(Convert.ToString(ResponsesGrid.Rows[e.RowIndex].Cells["RFP name"].Value)))
            {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "RFP name must not be empty";
                e.Cancel = true;
            }

            if (!e.Cancel && this.CheckDuplicateResponse(e.RowIndex))
            {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "Multiple mapping for one response is not allowed";
                e.Cancel = true;
            }

            if (!e.Cancel)
            {
                ResponsesGrid.Rows[e.RowIndex].ErrorText = "";
            }
        }
        private bool CheckSkipRowsFormat(string skipRows)
        {
            var regex = new Regex(@"((\d+-\d+)*(\d+)*)((,\d+-\d+)*(,\d+)*)*");
            var match = regex.Match(skipRows);

            while (match.Success)
            {
                if (match.Value == skipRows)
                {
                    return true;
                }
                match = match.NextMatch();
            }
            return false;
        }

        private bool CheckDuplicateResponse(int currentRow)
        {
            var rfpName = ResponsesGrid.Rows[currentRow].Cells["RFP name"].Value.ToString();
            var masterResponse = ResponsesGrid.Rows[currentRow].Cells["Master response indicator"].Value.ToString();

            for (int rowToCompare = 0; rowToCompare < _ds.Tables[_responseMember].DefaultView.Count; rowToCompare++)
            {
                var rfpNameToCompare = ResponsesGrid.Rows[rowToCompare].Cells["RFP name"].Value.ToString();
                var masterResponseToCompare = ResponsesGrid.Rows[rowToCompare].Cells["Master response indicator"].Value.ToString();

                if (currentRow != rowToCompare && rfpName == rfpNameToCompare && masterResponse == masterResponseToCompare)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
