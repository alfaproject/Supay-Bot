using System;
using System.Windows.Forms;

namespace Supay.Bot
{
  internal static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
      AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
        var ex = (Exception) e.ExceptionObject;
        MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
      };

      Application.Run(new Main());
    }
  }
}
