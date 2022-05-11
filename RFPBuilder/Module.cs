using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace RFPBuilder
{
    class Module
    {
        Excel.Range xlRange;
        HashSet<int> SkipRows;
        int Requirement, Response, Comments, Criticality;

        public IEnumerator<Requirement> GetEnumerator()
        {
            int row = 0;

            while (row < xlRange.Rows.Count)
            {
                row++;

                if (SkipRows.Contains(row))
                {
                    yield return new Requirement(xlRange.Cells[row, Requirement].Value2,
                                                 xlRange.Cells[row, Response].Value2,
                                                 xlRange.Cells[row, Comments].Value2,
                                                 xlRange.Cells[row, Criticality].Value2);
                }
            }
        }

        public Module(Excel.Range range, string requirement, string response, string comments, string criticality, string skipRows)
        {
            xlRange = range;
            Requirement = findColumnIndex(range,requirement);
            Response = findColumnIndex(range, response);
            Comments = findColumnIndex(range, comments);
            Criticality = findColumnIndex(range, criticality);
            initSkipRows(skipRows);
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
        //expected input: "1,2,4-10,13,15"
        private void initSkipRows(string skipRowsStr)
        {
            SkipRows = new HashSet<int>();
            var rows = skipRowsStr.Split('\u002C'); //comma ,
            foreach (var row in rows)
            {
                var periodArray = row.Split('\u002D'); //dash -
                if (periodArray.Length > 1)
                {
                    int start = int.Parse(periodArray[0]);
                    int end = int.Parse(periodArray[1]);
                    while (start <= end)
                    {
                        if (!SkipRows.Contains(start))
                        {
                            SkipRows.Add(start);
                        }
                        start++;
                    }
                }
                else
                {
                    int tmp = int.Parse(row);
                    if (!SkipRows.Contains(tmp))
                    {
                        SkipRows.Add(tmp);
                    }
                }
            }
        }
    }
}
