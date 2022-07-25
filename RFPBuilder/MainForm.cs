using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFPBuilder
{
    public partial class MainForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        private delegate void SafeCallDelegate();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        string filePath = string.Empty;
        string RFPName = string.Empty;

        public MainForm() {
            InitializeComponent();

            loadingPanel.Visible = false;

            if (!DBHandler.checkDatabaseExist()) {
                DBHandler.createDB();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    filePath = openFileDialog.FileName;
                    RFPName = Path.GetFileNameWithoutExtension(filePath);
                }
            }
            bool isMappingInitialized = false;
            if (RFPName != "" && !DBHandler.checkResponseMappingExist(RFPName))
            {
                isMappingInitialized = true;
                DBHandler.initResponseMapping(RFPName);
            }

            if (RFPName != "" && !DBHandler.checkPositionMappingExist(RFPName))
            {
                isMappingInitialized = true;
                DBHandler.initPositionMapping(RFPName, filePath);
            }

            if (isMappingInitialized)
            {
                MessageBox.Show("Mapping was automatically initialized. Please review it.");
            }
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnMapping_Click(object sender, EventArgs e) {
            this.Hide();

            MappingForm mappingForm = new MappingForm(RFPName);
            mappingForm.Left = this.Left;
            mappingForm.Top = this.Top;
            mappingForm.ShowDialog();

            this.Left = mappingForm.Left;
            this.Top = mappingForm.Top;
            this.Show();
        }

        private void topPanel_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private async void btnSave_Click(object sender, EventArgs e) {
            setFormEnabled(false);
            await Task.Factory.StartNew(() => DBHandler.saveRequirementsToDb(filePath, RFPName), TaskCreationOptions.LongRunning);
            setFormEnabled(true);
        }

        private async void btnUpdate_Click(object sender, EventArgs e) {
            setFormEnabled(false);
            await Task.Factory.StartNew(() => updateRfpDocument(), TaskCreationOptions.LongRunning);
            setFormEnabled(true);
        }

        private void setFormEnabled(bool set) {
            buttonsPanel.Enabled = set;
            mainPanel.Enabled = set;
            loadingPanel.Visible = !set;
        }

        private void updateRfpDocument() {
            try
            {
                if (filePath == null || filePath == "")
                {
                    return;
                }
                using (RFPDocument rfpDocument = new RFPDocument(filePath, RFPName))
                {
                    foreach (var module in rfpDocument)
                    {
                        foreach (var requirement in module)
                        {
                            bool multipleResponses;
                            (requirement.Response, requirement.Comments, multipleResponses) = DBHandler.getRequirement(RFPName, module.ModuleId, requirement.Id);
                            if (requirement.Response != "")
                            {
                                module.updateRequirement(requirement, multipleResponses);
                            }
                        }
                    }
                    rfpDocument.update();
                }
                File.SetLastWriteTime(filePath, DateTime.Now);
            } 
            catch (Exception ex)
            {
                _ = MessageBox.Show("Error occurred during updating RFP document. \n Error: " + ex.Message);
            }
        }

        private void buttonMinimize_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnView_Click(object sender, EventArgs e) {
            this.Hide();

            RFPForm rfpForm = new RFPForm(RFPName);
            rfpForm.Left = this.Left;
            rfpForm.Top = this.Top;
            rfpForm.ShowDialog();

            this.Left = rfpForm.Left;
            this.Top = rfpForm.Top;
            this.Show();
        }
    }
}
