using Panel_Drawing.Forms;
using PanelDrawing.CommonOperations;

namespace PanelDrawing
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //MessageBox.Show($"args {args[0]}");
            if(args.Length > 0)
            {
                Constants.Env_Variable_Electre_Proj_Path = args[0];
            }
            ApplicationConfiguration.Initialize();
            Application.Run(new frmMain());
        }
    }
}