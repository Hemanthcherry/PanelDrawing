using Panel_Drawing.Forms;
using System;
using System.IO;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using System.IO.Packaging;
namespace PanelDrawing.CommonOperations
{
    public class ExcelOperations
    {
        //Excel file convert into List Collection
        public static List<string> ReadExcelFile(string csvFilePath)
        {
            Constants.lstInfo = new List<string>();
            string line = string.Empty;
            Cursor.Current = Cursors.WaitCursor;
            using (TextFieldParser reader = new TextFieldParser(csvFilePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    Constants.lstInfo.Add(line);
                }
            }
            Cursor.Current = Cursors.Default;
            return Constants.lstInfo;
        }

        //Excel CSV file data convert into 2D Array structure
        public static string[,] ConvertCSVDataInto2DArray(string csvFilePath, StringComparer ordinalIgnoreCase)
        {
            string[] strArrayCSV = ReadExcelFile(csvFilePath).ToArray();
            string [,] data = CommonOperation.Conver1DArrayto2DArray(strArrayCSV,Path.GetFileName(csvFilePath));
            return data;
        }        

    }
}
