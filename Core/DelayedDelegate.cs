using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Supay.Bot
{
  public static class DelayedDelegate
  {
    private static readonly Timer runDelegates;
    private static readonly Dictionary<MethodInvoker, DateTime> delayedDelegates = new Dictionary<MethodInvoker, DateTime>();

    static DelayedDelegate()
    {
      runDelegates = new Timer {
        Interval = 500
      };
      runDelegates.Tick += RunDelegates;
      runDelegates.Enabled = true;
    }

    public static void Add(MethodInvoker method, int delay)
    {
      delayedDelegates.Add(method, DateTime.Now + TimeSpan.FromSeconds(delay));
    }

    private static void RunDelegates(object sender, EventArgs e)
    {
      var removeDelegates = new List<MethodInvoker>();

      foreach (var method in delayedDelegates.Keys.Where(method => DateTime.Now >= delayedDelegates[method]))
      {
        method();
        removeDelegates.Add(method);
      }

      foreach (var method in removeDelegates)
      {
        delayedDelegates.Remove(method);
      }
    }
  }
}
