using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace RFPBuilder
{
    class Module
    {
        Excel.Range xlRange;
        HashSet<int> SkipRows;
        int Requirement, Response, Comments, Criticality;
        public string ModuleId;
        public int row = 1;
        public IEnumerator<Requirement> GetEnumerator() {
            if (Requirement == 0 || Response == 0) {
                yield break;
            }

            for (; row < xlRange.Rows.Count; row++) {
                if (!SkipRows.Contains(row)) {
                    string requirement = (string)xlRange.Cells[row, Requirement].Value2;
                    string response = (string)xlRange.Cells[row, Response].Value2;
                    string comments = Comments != 0 ? (string)xlRange.Cells[row, Comments].Value2 : "";
                    string criticality = Criticality != 0 ? (string)xlRange.Cells[row, Criticality].Value2 : "";

                    yield return new Requirement(requirement,
                                                 response,
                                                 comments,
                                                 criticality);
                }
            }
        }

        public Module(string moduleId, Excel.Range range, string requirement, string response, string comments, string criticality, string skipRows) {
            ModuleId = moduleId;
            xlRange = range;
            Requirement = findColumnIndex(range, requirement);
            Response = findColumnIndex(range, response);
            Comments = findColumnIndex(range, comments);
            Criticality = findColumnIndex(range, criticality);
            initSkipRows(skipRows);
        }

        public void updateRequirement(Requirement requirement) {
            if (Convert.ToString(xlRange.Cells[row, Response].Value) == "") {
                xlRange.Cells[row, Response].Value = requirement.Response;
                xlRange.Cells[row, Response].Interior.Color = Excel.XlRgbColor.rgbRed;
            }
            if (Comments != 0) {
                xlRange.Cells[row, Comments].Value = requirement.Comments;
            }
        }

        private int findColumnIndex(Excel.Range xlRange, string value) {
            for (int i = 1; i <= xlRange.Rows.Count; i++) {
                for (int j = 1; j <= xlRange.Columns.Count; j++) {
                    if (Convert.ToString(xlRange.Cells[i, j].Value) == value) {
                        return j;
                    }
                }
            }
            return 0;
        }
        //expected input: "1,2,4-10,13,15"
        private void initSkipRows(string skipRowsStr) {
            SkipRows = new HashSet<int>();

            if (skipRowsStr == null || skipRowsStr == "") {
                return;
            }

            var rows = skipRowsStr.Split('\u002C'); //comma
            foreach (var row in rows) {
                var periodArray = row.Split('\u002D'); //dash
                if (periodArray.Length > 1) {
                    int start = int.Parse(periodArray[0]);
                    int end = int.Parse(periodArray[1]);
                    while (start <= end) {
                        if (!SkipRows.Contains(start)) {
                            SkipRows.Add(start);
                        }
                        start++;
                    }
                } else {
                    int tmp = int.Parse(row);
                    if (!SkipRows.Contains(tmp)) {
                        SkipRows.Add(tmp);
                    }
                }
            }
        }
    }
}
