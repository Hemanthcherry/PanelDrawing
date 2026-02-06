using Panel_Drawing.Forms;
using PanelDrawing.CommonOperations;
using PanelDrawing.Logs;

namespace PanelDrawing
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
           Constants.Env_Variable_Electre_Proj_Path = "C:\\ELECTRE\\electre_projects\\PANELDRAWING_FEB07\\";

            if (args.Length > 0)
            {
                Constants.Env_Variable_Electre_Proj_Path = args[0];
            }

            AppLog.Initialize();

            ApplicationConfiguration.Initialize();
            Application.Run(new frmMain());
        }
    }
}