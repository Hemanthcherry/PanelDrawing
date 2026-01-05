using System.Diagnostics;

namespace PanelDrawing.CommonOperations
{
    public class CommonOperation
    {
        public static string[,] Conver1DArrayto2DArray(string[] OneDimArray, string fileName)
        {
            int rows = OneDimArray.Length;
            int cols = 0;
            string fileExtention = string.Empty;
            fileExtention = Path.GetExtension(fileName).Trim();
            if (fileExtention.Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
            { cols = OneDimArray[0].Split(",").Count(); }
            else if (fileExtention.Equals(".csv", StringComparison.CurrentCultureIgnoreCase))
            { cols = OneDimArray[0].Split(";").Count(); }
            //NOTE :This BELOW  Only for PanelDetails.txt file not applicable for other text files,pls be causion - START 
            int colincrese = 0; int totalcols = 0;
            if (fileExtention.Equals(".txt", StringComparison.CurrentCultureIgnoreCase) && fileName.Contains(Constants.panelDetailsTextFileName, StringComparison.CurrentCultureIgnoreCase))
            {
                colincrese = Constants.colPD_Max_Columns - cols;
                totalcols = cols + colincrese;
            }
            //NOTE :This BELOW  Only for PanelDetails.txt file not applicable for other text files,pls be causion - END
            if (fileExtention.Equals(".txt", StringComparison.CurrentCultureIgnoreCase) && fileName.Contains(Constants.panelDetailsTextFileName, StringComparison.CurrentCultureIgnoreCase))
            {
                Constants.arr2Dinfo = new string[rows, totalcols];
            }
            else
                Constants.arr2Dinfo = new string[rows, cols];
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                for (int r = 0; r <= rows - 1; r++)
                {
                    if (fileExtention.Equals(".txt"))
                    { Constants.arrInfo = OneDimArray[r].Trim().Split(","); }
                    else if (fileExtention.Equals(".csv"))
                    { Constants.arrInfo = OneDimArray[r].Trim().Split(";"); }
                    for (int col = 0; col <= cols - 1; col++)
                    {
                        Constants.arr2Dinfo[r, col] = Constants.arrInfo[col];
                    }
                    //NOTE :This BELOW  Only for PanelDetails.txt file not applicable for other text files,pls be causion - START 
                    if (fileExtention.Equals(".txt", StringComparison.CurrentCultureIgnoreCase) && fileName.Contains(Constants.panelDetailsTextFileName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        for (int clmn = cols; clmn <= totalcols-1; clmn++)
                        {
                            Constants.arr2Dinfo[r, clmn] = string.Empty;
                        }

                    }
                    //NOTE :This BELOW  Only for PanelDetails.txt file not applicable for other text files,pls be causion - END
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            Cursor.Current = Cursors.Default;
            return Constants.arr2Dinfo;
        }

        public static  void CloseVbsFiles(string VbsFilePath)
        { 
            // Start the VBScript file
            Process vbScriptProcess = new Process();
            vbScriptProcess.StartInfo.FileName = "cscript";
            vbScriptProcess.StartInfo.Arguments = "//B //Nologo "+""+ VbsFilePath;
            vbScriptProcess.StartInfo.UseShellExecute = false;
            vbScriptProcess.Start();

            // Wait for the script to execute (optional)
            //vbScriptProcess.WaitForExit();

            // Close the process if still running
            if (!vbScriptProcess.HasExited)
            {
                vbScriptProcess.Kill();
            }

            vbScriptProcess.Dispose();
        }

        public static bool IsFileExist(string filepath)
        {
            if(!File.Exists(filepath)) 
                return false;
            else return true;
        }

        public static void  CreateTemp_El_Exec_File(string filepath)
        {
            Constants.el_Exec_Temp_FilePath = string.Concat(@"C:\Users\", Environment.GetEnvironmentVariable("USERNAME", EnvironmentVariableTarget.Machine), @"\", filepath);//@"C:\Users\Admin\el_exec";
            if (File.Exists(Constants.el_Exec_Temp_FilePath)) File.Delete(Constants.el_Exec_Temp_FilePath);
            File.Copy(Constants.el_ExecFilePath, Constants.el_Exec_Temp_FilePath);
        }
    }
}

