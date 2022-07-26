using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFPBuilder
{
    public partial class MainForm : Form
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")] public const int WM_NCLBUTTONDOWN = 0xA1;
        [SuppressMessage("ReSharper", "InconsistentNaming")] public const int HT_CAPTION = 0x2;
        private delegate void SafeCallDelegate();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        string _filePath = string.Empty;
        string _rfpName = string.Empty;

        public MainForm()
        {
            InitializeComponent();

            loadingPanel.Visible = false;

            if (!DBHandler.CheckDatabaseExist())
            {
                DBHandler.CreateDb();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _filePath = openFileDialog.FileName;
                    _rfpName = Path.GetFileNameWithoutExtension(_filePath);
                }
            }
            var isMappingInitialized = false;
            if (_rfpName != "" && !DBHandler.CheckResponseMappingExist(_rfpName))
            {
                isMappingInitialized = true;
                DBHandler.InitResponseMapping(_rfpName);
            }

            if (_rfpName != "" && !DBHandler.CheckPositionMappingExist(_rfpName))
            {
                isMappingInitialized = true;
                DBHandler.InitPositionMapping(_rfpName, _filePath);
            }

            if (isMappingInitialized)
            {
                MessageBox.Show("Mapping was automatically initialized. Please review it.");
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMapping_Click(object sender, EventArgs e)
        {
            this.Hide();

            var mappingForm = new MappingForm(_rfpName);
            mappingForm.Left = this.Left;
            mappingForm.Top = this.Top;
            mappingForm.ShowDialog();

            this.Left = mappingForm.Left;
            this.Top = mappingForm.Top;
            this.Show();
        }

        private void topPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            SetFormEnabled(false);
            await Task.Factory.StartNew(() => DBHandler.SaveRequirementsToDb(_filePath, _rfpName), TaskCreationOptions.LongRunning);
            SetFormEnabled(true);
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            SetFormEnabled(false);
            await Task.Factory.StartNew(UpdateRfpDocument, TaskCreationOptions.LongRunning);
            SetFormEnabled(true);
        }

        private void SetFormEnabled(bool set)
        {
            buttonsPanel.Enabled = set;
            mainPanel.Enabled = set;
            loadingPanel.Visible = !set;
        }

        private void UpdateRfpDocument()
        {
            try
            {
                if (string.IsNullOrEmpty(_filePath))
                {
                    return;
                }
                using (var rfpDocument = new RFPDocument(_filePath, _rfpName))
                {
                    foreach (var module in rfpDocument)
                    {
                        foreach (var requirement in module)
                        {
                            bool multipleResponses;
                            (requirement.Response, requirement.Comments, multipleResponses) = DBHandler.GetRequirement(_rfpName, module.ModuleId, requirement.Id);
                            if (requirement.Response != "")
                            {
                                module.updateRequirement(requirement, multipleResponses);
                            }
                        }
                    }
                    rfpDocument.update();
                }
                File.SetLastWriteTime(_filePath, DateTime.Now);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Error occurred during updating RFP document. \n Error: " + ex.Message);
            }
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            this.Hide();

            var rfpForm = new RFPForm(_rfpName);
            rfpForm.Left = this.Left;
            rfpForm.Top = this.Top;
            rfpForm.ShowDialog();

            this.Left = rfpForm.Left;
            this.Top = rfpForm.Top;
            this.Show();
        }
    }
}
