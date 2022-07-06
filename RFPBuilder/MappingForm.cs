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
        DataSet ds;

        public MappingForm(string rfpName) {
            InitializeComponent();

            RFPName = rfpName;
            string moduleMember, responseMember, positionMember;
            
            (ds, moduleMember, responseMember, positionMember) = DBHandler.getMapping(RFPName);
            /*
            ModulesMapGrid.DataSource = ds;
            ModulesMapGrid.DataMember = moduleMember;

            ResponsesGrid.DataSource = ds;
            ResponsesGrid.DataMember = responseMember;

            PositionMapGrid.DataSource = ds;
            PositionMapGrid.DataMember = positionMember;
            */
            ModulesMapGrid.DataSource = ds.Tables[0].DefaultView;
            ResponsesGrid.DataSource = ds.Tables[1].DefaultView;
            PositionMapGrid.DataSource = ds.Tables[2].DefaultView;
        }

        private void btnClose_Click(object sender, EventArgs e) {
            DBHandler.updateMapping(ds);
            this.Close();
        }

        private void buttonMinimize_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }

        private void PositionMapGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e)
        {
            ds.Tables[2].DefaultView.RowFilter = PositionMapGrid.FilterString;
        }

        private void PositionMapGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e)
        {
            ds.Tables[2].DefaultView.Sort = PositionMapGrid.SortString;
        }

        private void ResponsesGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e)
        {
            ds.Tables[1].DefaultView.RowFilter = PositionMapGrid.FilterString;
        }

        private void ResponsesGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e)
        {
            ds.Tables[1].DefaultView.Sort = ResponsesGrid.SortString;
        }

        private void ModulesMapGrid_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e)
        {
            ds.Tables[0].DefaultView.RowFilter = ModulesMapGrid.FilterString;
        }

        private void ModulesMapGrid_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e)
        {
            ds.Tables[0].DefaultView.Sort = ModulesMapGrid.SortString;
        }

        private void ResponsesGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
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
    }
}
