﻿using System;
using System.Threading.Tasks;

namespace Supay.Bot
{
  internal static class ThreadUtil
  {
    /// <summary>
    /// Executes the specified callback with the specified arguments
    /// asynchronously on a thread pool thread.
    /// </summary>
    public static void FireAndForget<T>(Action<T> callback, T state)
    {
      Task.Factory.StartNew(s => callback((T) s), state);
    }
  }
}
