using Microsoft.VisualBasic.FileIO;
using PanelDrawing.Core.Constants;

namespace PanelDrawing.Services.Infrastructure
{
    public class ExcelService
    {
        //Excel file convert into List Collection
        public static List<string> ReadExcelFile(string csvFilePath)
        {
            PanelConstants.lstInfo = new List<string>();
            string line = string.Empty;
            Cursor.Current = Cursors.WaitCursor;
            using (TextFieldParser reader = new TextFieldParser(csvFilePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    PanelConstants.lstInfo.Add(line);
                }
            }
            Cursor.Current = Cursors.Default;
            return PanelConstants.lstInfo;
        }

        //Excel CSV file data convert into 2D Array structure
        //public static string[,] ConvertCSVDataInto2DArray(string csvFilePath, StringComparer ordinalIgnoreCase)
        //{
        //    string[] strArrayCSV = ReadExcelFile(csvFilePath).ToArray();
        //    string [,] data = CommonOperation.Conver1DArrayto2DArray(strArrayCSV,Path.GetFileName(csvFilePath));
        //    return data;
        //}        

    }
}
