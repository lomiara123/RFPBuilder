using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Data;

namespace RFPBuilder
{
    class RFPDocument:IDisposable
    {
        public string Path { get; }
        private Excel.Application xlApp;
        private Excel.Workbook xlWorkBook;

        public IEnumerator<Module> GetEnumerator()
        {
            DataTable dt = DBHandler.getPositionMap(System.IO.Path.GetFileNameWithoutExtension(Path));
            
            foreach(DataRow dr in dt.Rows)
            {
                yield return new Module("TEST",
                    xlWorkBook.Sheets[dr["SheetName"].ToString()].UsedRange, 
                    dr["Requirement"].ToString(),
                    dr["Response"].ToString(),
                    dr["Comments"].ToString(),
                    dr["Criticality"].ToString(),
                    dr["SkipRows"].ToString());
            }
        }

        public RFPDocument(string path)
        {
            Path = path;
            xlApp = new Excel.Application();
            xlApp.DisplayAlerts = false;
            xlWorkBook = xlApp.Workbooks.Open(Path);
        }
        
        public void Dispose()
        {
            xlWorkBook.Close();
            xlApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
        }

        public void update()
        {
            xlWorkBook.Save();
        }


    }
}
