using System;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace RFPBuilder
{
    class Module
    {
        Excel.Range xlRange;
        HashSet<int> SkipRows;
        string requirementColumn, responseColumn, commentsColumn, criticalityColumn;
        public string ModuleId;
        public int row = 1;
        public IEnumerator<Requirement> GetEnumerator()
        {
            /*
            if (Requirement == 0 || Response == 0) {
                yield break;
            }
            */
            for (; row <= xlRange.Rows.Count; row++)
            {
                if (!SkipRows.Contains(row))
                {
                    string requirement = Convert.ToString(xlRange.Cells[row, requirementColumn].Value2);
                    if (requirement == null)
                    {
                        continue;
                    }
                    string response = Convert.ToString(xlRange.Cells[row, responseColumn].Value2);
                    string comments = commentsColumn == "" || commentsColumn != null ? "" : Convert.ToString(xlRange.Cells[row, commentsColumn].Value2);
                    string criticality = criticalityColumn == null || criticalityColumn == "" ? "" : Convert.ToString(xlRange.Cells[row, criticalityColumn].Value2);

                    yield return new Requirement(requirement,
                                                 response,
                                                 new List<string> { comments != null ? comments : "" },
                                                 criticality);
                }
            }
        }

        public Module(string moduleId, Excel.Range range, string requirement, string response, string comments, string criticality, string skipRows)
        {
            ModuleId = moduleId;
            xlRange = range;
            requirementColumn = requirement;
            responseColumn = response;
            commentsColumn = comments;
            criticalityColumn = criticality;
            initSkipRows(skipRows);
        }

        public void updateRequirement(Requirement requirement, bool multipleResponses)
        {
            if (Convert.ToString(xlRange.Cells[row, responseColumn].Value) == "" || xlRange.Cells[row, responseColumn].Value == null)
            {
                if (multipleResponses)
                {
                    xlRange.Cells[row, responseColumn].Interior.Color = Excel.XlRgbColor.rgbRed;
                }
                else
                {
                    xlRange.Cells[row, responseColumn].Interior.Color = Excel.XlRgbColor.rgbGreen;
                }
                xlRange.Cells[row, responseColumn].Value = requirement.Response;
            }
            if (commentsColumn != null || commentsColumn != "")
            {
                for (int i = 0; i < requirement.Comments.Count; i++)
                {
                    xlRange.Cells[row, ColumnLetterToColumnIndex(commentsColumn) + i + 1].Value = requirement.Comments[i];
                }
            }
        }
        public static int ColumnLetterToColumnIndex(string columnLetter)
        {
            columnLetter = columnLetter.ToUpper();
            int sum = 0;

            for (int i = 0; i < columnLetter.Length; i++)
            {
                sum *= 26;
                sum += (columnLetter[i] - 'A' + 1);
            }
            return sum;
        }
        //expected input: "1,2,4-10,13,15"
        private void initSkipRows(string skipRowsStr)
        {
            SkipRows = new HashSet<int>();

            if (skipRowsStr == null || skipRowsStr == "")
            {
                return;
            }

            var rows = skipRowsStr.Split('\u002C'); //comma
            foreach (var row in rows)
            {
                var periodArray = row.Split('\u002D'); //dash
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
