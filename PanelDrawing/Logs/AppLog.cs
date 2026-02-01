using PanelDrawing.CommonOperations;
using System;
using System.IO;
using System.Text;

namespace PanelDrawing.Logs
{
    public static class AppLog
    {
        private static readonly object _lock = new object();
        private static string _logFilePath = string.Empty;

        public static void Initialize()
        {
            string baseFolder =  Path.Combine(Constants.Env_Variable_Electre_Proj_Path, "Logs");

            if (!Directory.Exists(baseFolder))
            {
                Directory.CreateDirectory(baseFolder);
            }

            _logFilePath = Path.Combine(baseFolder, "paneldrawing.log");

           // Write("INFO", "Logging initialized");
        }

        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Warn(string message)
        {
            Write("WARN", message);
        }

        public static void Error(string message, Exception? ex = null)
        {
            Write("ERROR", message, ex);
        }

        private static void Write(string level, string message, Exception? ex = null)
        {
            if (string.IsNullOrEmpty(_logFilePath)) return;

            var sb = new StringBuilder();
            sb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] ");
            sb.Append(message);

            if (ex != null)
            {
                sb.AppendLine();
                sb.Append(ex);
            }

            lock (_lock)
            {
                File.AppendAllText(_logFilePath, sb.ToString() + Environment.NewLine);
            }
        }
    }
}
