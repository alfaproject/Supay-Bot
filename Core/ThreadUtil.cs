using System;
using System.Threading;

namespace BigSister {
  static class ThreadUtil {

    /// <summary>
    ///   Executes the specified callback with the specified arguments
    ///   asynchronously on a thread pool thread. </summary>
    public static bool FireAndForget<T>(Action<T> callback, T state) {
      return ThreadPool.QueueUserWorkItem(s => callback((T)s), state);
    }

  } //class ThreadUtil
} //namespace BigSister