using System;
using System.Windows.Forms;

namespace Supay.Bot {
  static class Program {

    /// <summary>
    ///   The main entry point for the application. </summary>
    [STAThread]
    static void Main() {
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
      Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Main());
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
      Exception ex = (Exception)e.ExceptionObject;
      MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
    static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
      MessageBox.Show(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
    }
  
  } //class Program
} //namespace Supay.Bot