using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;

namespace RFPBuilder
{
    class RFPDocument
    {
        public string Path { get; }
        private Excel.Application xlApp;
        private Excel.Workbook xlWorkBook;
        private Excel.Worksheet xlWorkSheet;
        private Excel.Range xlRange;

        public IEnumerator<Module> GetEnumerator()
        {
            SqlDataReader reader;
            SqlCommand command = new SqlCommand();
            reader = command.ExecuteReader(); 
            
            while(reader.Read())
            {
                yield return new Module(xlWorkBook.Sheets[reader["SheetName"]].UsedRange, 
                    reader["Requirement"].ToString(),
                    reader["Response"].ToString(),
                    reader["Comments"].ToString(),
                    reader["Criticality"].ToString(),
                    reader["SkipRows"].ToString());
            }
        }

        public RFPDocument(string path)
        {
            Path = path;
            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(Path);
        }


    }
}
