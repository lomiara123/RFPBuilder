using System;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using System.Windows.Forms;

namespace RFPBuilder
{
    class RFPDocument : IDisposable
    {
        public string Path { get; }
        public string RFPName { get; }
        private Excel.Application xlApp;
        private Excel.Workbook xlWorkBook;

        public IEnumerator<Module> GetEnumerator() {
            DataTable dt = DBHandler.getPositionMap(RFPName);
            if (dt == null || dt.Rows.Count == 0) {
                MessageBox.Show("Please fill in the Position mapping");
                yield break;
            }
            foreach (DataRow dr in dt.Rows) {
                Excel.Worksheet xlWorkSheet;
                string reqirement, response, comments, criticality, skipRows, module;
                try {
                    xlWorkSheet = xlWorkBook.Sheets[dr["Sheet"].ToString()];
                    reqirement = dr["Requirement"].ToString();
                    response = dr["Response"].ToString();
                    comments = dr["Comments"].ToString();
                    criticality = dr["Criticality"].ToString();
                    module = dr["Module"].ToString();
                    skipRows = dr["Skip rows"].ToString();
                } catch (Exception ex) {
                    MessageBox.Show("Error occurred during module initialization. \n. Error: " + ex.Message);
                    throw;
                }

                yield return new Module(module,
                                        xlWorkSheet.UsedRange,
                                        reqirement,
                                        response,
                                        comments,
                                        criticality,
                                        skipRows);
            }
        }

        public RFPDocument(string path, string rfpname) {
            RFPName = rfpname;
            Path = path;
            xlApp = new Excel.Application();
            xlApp.DisplayAlerts = false;
            xlWorkBook = xlApp.Workbooks.Open(Path);
        }
        
        public void Dispose() {
            xlWorkBook.Close();
            xlApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
        }

        public void update() {
            xlWorkBook.Save();
        }


    }
}
