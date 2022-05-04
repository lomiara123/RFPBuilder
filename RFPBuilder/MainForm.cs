using System;
using System.IO;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace RFPBuilder
{
    public partial class MainForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        string filePath = string.Empty;
        string fileName = string.Empty;
        string RFPName = string.Empty;

        public MainForm()
        {
            InitializeComponent();

            if (!DBHandler.checkDatabaseExist())
            {
                DBHandler.createDB();
            }
            else
            {
                DBHandler.deleteDB();
                DBHandler.createDB();
            }
            
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    fileName = Path.GetFileNameWithoutExtension(filePath);
                    RFPName = fileName;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMapping_Click(object sender, EventArgs e)
        {
          //  RFPName = "TEST";
            if (!DBHandler.checkModuleMappingExist(RFPName))
            {
                DBHandler.initModuleMapping(RFPName);
            }

            if (!DBHandler.checkResponseMappingExist(RFPName))
            {
                DBHandler.initResponseMapping(RFPName);
            }

            this.Hide();
            MappingForm mappingForm = new MappingForm();
            mappingForm.RFPName = RFPName;
            mappingForm.ShowDialog();
            this.Show();
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void topPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(filePath);
            Excel.Worksheet xlWorkSheet = xlWorkBook.Sheets[1];
            Excel.Range xlRange = xlWorkSheet.UsedRange;
            int requirement = findColumnIndex(xlRange, "Req #");
            int response = findColumnIndex(xlRange, "Response");
            int comments = findColumnIndex(xlRange, "Comments");
            int criticality = findColumnIndex(xlRange, "Criticality");
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            string insert = "insert into MasterRFP " +
                                   "(RFPName, ModuleId, ReqId, Criticality, Response, Comments)" +
                                   " values " +
                                   "(@RFPName, @ModuleId, @ReqId, @Criticality, @Response, @Comments)";
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                conn.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(insert, conn))
                {
                    for (int i = 10; i <= xlRange.Rows.Count; i++)
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@RFPName", RFPName);
                        command.Parameters.AddWithValue("@ModuleId", "Procurement");
                        command.Parameters.AddWithValue("@ReqId", xlRange.Cells[i, requirement].Value2);
                        command.Parameters.AddWithValue("@Criticality", xlRange.Cells[i, criticality].Value2 != null ? xlRange.Cells[i, criticality].Value2 : DBNull.Value);
                        command.Parameters.AddWithValue("@Response", xlRange.Cells[i, response].Value2 != null ? xlRange.Cells[i, response].Value2 : DBNull.Value);
                        command.Parameters.AddWithValue("@Comments", xlRange.Cells[i, comments].Value2 != null ? xlRange.Cells[i, comments].Value2 : DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                    
                }
                
            }
            
            xlWorkBook.Close();
        }

        private int findColumnIndex(Excel.Range xlRange, string value)
        {
            for (int i = 1; i <= xlRange.Rows.Count; i++)
            {
                for (int j = 1; j <= xlRange.Columns.Count; j++)
                {
                    if (xlRange.Cells[i, j].Value == value)
                    {
                        return j;
                    }
                }
            }
            return 0;
        }
    }
}
