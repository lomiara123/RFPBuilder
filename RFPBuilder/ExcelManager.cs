using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace RFPBuilder
{
    class ExcelManager
    {
        int requirementColumn, responseColumn, commentsColumn, criticalityColumn;
        int row = 0;
        Excel.Application xlApp;
        Excel.Workbook xlWorkBook;
        Excel.Worksheet xlWorkSheet;
        Excel.Range xlRange;
        HashSet<int> skipRows;
        public string requirement, response, comments, criticality, moduleId ;
        public ExcelManager(string path)
        {
            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(path);
            skipRows = new HashSet<int> { 1,2,3,4,5,6,7,8,9};
        }

        ~ExcelManager()
        {
            xlWorkBook.Close();
        }

        public void setWorksheet(string worksheet, string requirement, string response, string comments, string criticality, string skipRows)
        {
            xlWorkSheet = xlWorkBook.Sheets[worksheet];
            xlRange = xlWorkSheet.UsedRange;
            requirementColumn = findColumnIndex(xlRange, requirement);
            responseColumn = findColumnIndex(xlRange, response);
            commentsColumn = findColumnIndex(xlRange, comments);
            criticalityColumn = findColumnIndex(xlRange, criticality);
            moduleId = worksheet;
            requirement = "";
            response = "";
            comments = "";
            criticality = "";
        }
        public bool nextRequirement()
        {
            while (row < xlRange.Rows.Count)
            {
                row++;

                if (skipRows.Contains(row))
                {
                    continue;
                }

                requirement = xlRange.Cells[row, requirementColumn].Value2;
                response = xlRange.Cells[row, responseColumn].Value2;
                comments = xlRange.Cells[row, commentsColumn].Value2;
                criticality = xlRange.Cells[row, criticalityColumn].Value2;

                return true;
            }

            return false;
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
