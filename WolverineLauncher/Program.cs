#define CHECK_STARTUP_ARGUMENTS
#define EXCEPTION_HANDLER

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace WolverineLauncher
{
    static class Program
    {
        static void ExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            string exceptionData = e.Exception.ToString();

            try
            {
                string tempFolder = Path.GetTempPath();
                string logFile = Path.Combine(tempFolder, $"[{DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss")}] WolverineLauncher Fatal Error.txt");

                File.WriteAllText(logFile, exceptionData);

                using (Process textviewer = Process.Start(new ProcessStartInfo(logFile)))
                {
                    textviewer.Dispose();
                }
            }
            catch
            {
                MessageBox.Show(exceptionData, "WolverineLauncher Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            
        }

        [STAThread]
        static void Main(string[] args)
        {
#if CHECK_STARTUP_ARGUMENTS
            foreach (string argument in args)
            {
                switch (argument)
                {
                    case "-nolauncher":
                        Overrides._noLauncher = true;
                        break;

                    case "-promode":
                        Overrides._proMode = true;
                        break;

                    case "-unsafe":
                        Overrides._unsafeMode = true;
                        break;
                }
            }
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if EXCEPTION_HANDLER
            Application.ThreadException += new ThreadExceptionEventHandler(ExceptionHandler);
            try
            {
                Application.Run(new Launcher());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "WolverineLauncher Application.Run() Failed", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
#else
            Application.Run(new Launcher());
#endif
        }
    }
}
