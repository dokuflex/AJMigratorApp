using DokuFlex.Windows.Common.Log;
using System;
using System.Threading;
using System.Windows.Forms;

namespace AJMigratorApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += new ThreadExceptionEventHandler(UIThreadException);
            // Set the unhandled exception mode to force all Windows Forms errors to go through
            // our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            // Add the event handler for handling non-UI thread exceptions to the event.
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.Run(new MainForm());
        }

        private static void UIThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var ex = (Exception)e.Exception;
            LogFactory.CreateLog().LogError(ex);
            MessageBox.Show(string.Format("{0}\n\nInformación:\n{1}", "Ha ocurrido un error en la ejecución de la aplicación", ex.Message), "AJMigratorApp", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            LogFactory.CreateLog().LogError(ex);
            MessageBox.Show(string.Format("{0}\n\nInformación:\n{1}", "Ha ocurrido un error en la ejecución de la aplicación", ex.Message), "AJMigratorApp", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
