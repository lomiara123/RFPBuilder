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
        string RFPName = string.Empty;

        public MainForm()
        {
            InitializeComponent();
            
            if (!DBHandler.checkDatabaseExist()) {
                DBHandler.createDB();
            }
            /*
            else
            {
                DBHandler.deleteDB();
                DBHandler.createDB();
            }
            string connectionString = @"Server=localhost;Integrated security=SSPI;database=RequestForProposal";
            string insert = "insert into PositionMap " +
                                   "(RFPName, SheetName, ModuleId, Requirement, Response, Comments, SkipRows, Criticality)" +
                                   " values " +
                                   "('test', '4. Procurement', 'proc', 'Req #', 'Response', 'Comments', '1-9', 'Criticality')";
            
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                conn.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(insert, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
            */
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    RFPName = Path.GetFileNameWithoutExtension(filePath);
                }
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMapping_Click(object sender, EventArgs e)
        {
            if (RFPName != "" && !DBHandler.checkModuleMappingExist(RFPName))
            {
                DBHandler.initModuleMapping(RFPName);
            }

            if (RFPName != "" && !DBHandler.checkResponseMappingExist(RFPName))
            {
                DBHandler.initResponseMapping(RFPName);
            }

            if (RFPName != "" && !DBHandler.checkPositionMappingExist(RFPName))
            {
                DBHandler.initPositionMapping(RFPName, filePath);
            }

            this.Hide();
            MappingForm mappingForm = new MappingForm(RFPName);
            mappingForm.Left = this.Left;
            mappingForm.Top = this.Top;
            mappingForm.ShowDialog();
            this.StartPosition = FormStartPosition.Manual;
            this.Left = mappingForm.Left;
            this.Top = mappingForm.Top;
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
            DBHandler.saveRequirementsToDb(filePath, RFPName);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (RFPDocument rfpDocument = new RFPDocument(filePath, RFPName))
            {
                foreach (var module in rfpDocument)
                {
                    foreach (var requirement in module)
                    {
                        (requirement.Response, requirement.Comments) = DBHandler.getRequirement(requirement.Id, RFPName);

                        if (requirement.Response != "")
                        {
                            module.updateRequirement(requirement);
                        }
                    }
                }
                rfpDocument.update();
            }
            
            File.SetLastWriteTime(filePath, DateTime.Now);
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            this.Hide();
            RFPForm rfpForm = new RFPForm(RFPName);
            rfpForm.Left = this.Left;
            rfpForm.Top = this.Top;
            rfpForm.ShowDialog();
            this.StartPosition = FormStartPosition.Manual;
            this.Left = rfpForm.Left;
            this.Top = rfpForm.Top;
            this.Show();
        }
    }
}
