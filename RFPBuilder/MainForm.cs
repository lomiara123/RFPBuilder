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
            ExcelManager manager = new ExcelManager(filePath);
            manager.setWorksheet("4. Procurement", "Req #", "Response", "Comments", "Criticality", "1-9");
            DBHandler.saveRFPtoDB(manager, RFPName);
        }

    }
}
