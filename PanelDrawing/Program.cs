using Panel_Drawing.Forms;
using PanelDrawing.Core.Constants;
using PanelDrawing.Logging;

namespace PanelDrawing
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
           //PanelConstants.Electre_Proj_Path = "C:\\ELECTRE\\electre_projects\\PANELDRAWING_17FEB26\\";

            if (args.Length > 0)
            {
                PanelConstants.Electre_Proj_Path = args[0];
            }

            ApplicationLogger.Initialize();

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}