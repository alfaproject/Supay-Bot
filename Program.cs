using System;
using System.Threading;
using System.Windows.Forms;

namespace Supay.Bot {
  internal static class Program {
    /// <summary>
    ///   The main entry point for the application. </summary>
    [STAThread]
    private static void Main() {
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      Application.ThreadException += Application_ThreadException;

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Main());
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
      var ex = (Exception) e.ExceptionObject;
      MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) {
      MessageBox.Show(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
    }
  }
}
