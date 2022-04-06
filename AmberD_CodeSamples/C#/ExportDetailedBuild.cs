/* 
Written by Amber D. 2019
C# class to write data to excel file
A grid view is passed to it, but 
this class creates a specific file structure
which does not make it reusable  for other projects
without recoding.
*/


using System;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;

namespace Tooling_Tracker_Info
{
    class export
    {
        //Class Variables
        Excel.Application oXL = new Excel.Application();
        Excel.Worksheet mWSheet1 = default(Excel.Worksheet);
        string strHeader = "";
        
        public void buildSheetComponents(DataGridView dg)
        {           
            strHeader = "Plant 8 Tool Components";//Header Label
            commonSetup();//Build Common elements for all exports

            // SHOW COLUMNS ON THE TOP.
            mWSheet1.Cells[2, 1].value = "Description";
            mWSheet1.Cells[2, 2].value = "E-Number";
            mWSheet1.Cells[2, 3].value = "Type";

            //SHOW COLUMN HEADERS AS BOLD
            mWSheet1.Range["A2:D2"].Font.Bold = true;

            //UNDERLINE COLUMN HEADERS
            mWSheet1.Range["A2:D2"].Font.Underline = true;

            exportFile(dg);//Call Method to populate cells
            oXL.Visible = true;//View Excell Sheet
        }

        public void buildToolSheet(DataGridView dgTools)
        {                       
            strHeader = "Plant 8 Tool Info";//Header Label
            commonSetup();//Build Common elements for all exports

            // SHOW COLUMNS ON THE TOP.
            mWSheet1.Cells[2, 1].value = "Tool ID";
            mWSheet1.Cells[2, 2].value = "Tool #";
            mWSheet1.Cells[2, 3].value = "Length";
            mWSheet1.Cells[2, 4].value = "Length Tol.";
            mWSheet1.Cells[2, 5].value = "Radius";
            mWSheet1.Cells[2, 6].value = "Radius Tol. Low";
            mWSheet1.Cells[2, 7].value = "Radius Tol. High";
            mWSheet1.Cells[2, 8].value = "Life";
            mWSheet1.Cells[2, 9].value = "Bump Allowed";           
            mWSheet1.Cells[2, 10].value = "Bump Max";
            mWSheet1.Cells[2, 11].value = "Radius Type";
            mWSheet1.Cells[2, 12].value = "Family";
            mWSheet1.Cells[2, 13].value = "Tool";
            mWSheet1.Cells[2, 14].value = "Holder";
            mWSheet1.Cells[2, 15].value = "Ret. Knob";
            mWSheet1.Cells[2, 16].value = "Collet 1";
            mWSheet1.Cells[2, 17].value = "Collet 2";
            mWSheet1.Cells[2, 18].value = "Collet Nut";
            mWSheet1.Cells[2, 19].value = "Extension";
            mWSheet1.Cells[2, 20].value = "Ext. Nut";
            mWSheet1.Cells[2, 21].value = "Bridge";
            mWSheet1.Cells[2, 22].value = "Regrind Offset";
            mWSheet1.Cells[2, 23].value = "Active";

            //Method to populate cells
            exportFile(dgTools);
            //View Excell Sheet
            oXL.Visible = true;
        }

        public void buildRecordSheet(DataGridView dgRecord)
        {
            strHeader = "Plant 8 Tool Record";//Header Label
            commonSetup();//Build Common elements for all exports

            // SHOW COLUMNS ON THE TOP.
            mWSheet1.Cells[2, 1].value = "Tool ID";
            mWSheet1.Cells[2, 2].value = "Serial #";
            mWSheet1.Cells[2, 3].value = "Date";
            mWSheet1.Cells[2, 4].value = "Length";
            mWSheet1.Cells[2, 5].value = "Radius";
            mWSheet1.Cells[2, 6].value = "Count";
            mWSheet1.Cells[2, 7].value = "Cell";

            //Method to populate cells
            exportFile(dgRecord);
            //View Excell Sheet
            oXL.Visible = true;
        }

        private void commonSetup()
        {
            //Create workbook and sheet
            oXL.Workbooks.Add();
            mWSheet1 = oXL.Sheets["Sheet1"];

            //SHOW CURRENT DATE
            DateTime dt3 = DateTime.Now;
            mWSheet1.Cells[1, 6].value = dt3.Date.ToString("MM/dd/yyyy");
            mWSheet1.Cells[1, 5].value = "TODAY'S DATE:";

            //// SHOW THE HEADER BOLD AND SET FONT AND SIZE.
            mWSheet1.Cells[1, 1].value = "Plant 8 Tool Info";
            mWSheet1.Cells[1, 1].FONT.NAME = "Calibri";
            mWSheet1.Cells[1, 1].Font.Bold = true;
            mWSheet1.Cells[1, 1].Font.Size = 20;

            // MERGE AND CENTER CELLS OF THE HEADER.
            mWSheet1.Range["A1:D1"].MergeCells = true;
            mWSheet1.Range["A1:D1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            //// CENTER COLUMN HEADERS.
            mWSheet1.Range["A2:W2"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            ////SHOW COLUMN HEADERS AS BOLD
            mWSheet1.Range["A2:W2"].Font.Bold = true;

            //UNDERLINE COLUMN HEADERS
            mWSheet1.Range["A2:W2"].Font.Underline = true;

        }
        private void exportFile(DataGridView dg)
        {
           // System.Data.DataTable dt = ds.Tables[0];

            // NOW WRITE DATA TO EACH CELL.
            int i = 0;
            int j = 0;

            for (i = 0; i <= dg.RowCount - 1; i++)
            {
                for (j = 0; j <= dg.ColumnCount - 1; j++)
                {
                    DataGridViewCell cell = dg[j, i];
                    mWSheet1.Cells[i + 3, j + 1] = cell.Value;
                    mWSheet1.Range["a:w"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;                  
                }
            }
            //FIT AND CENTER COLUMNS
            mWSheet1.Columns.AutoFit();
           
        }
    }
}
