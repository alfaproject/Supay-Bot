using System;
using System.Threading;

namespace BigSister {
  class ThreadUtil {

    /// <summary>
    ///   Delegate to wrap another delegate and its arguments. </summary>
    private delegate void DelegateWrapper(Delegate d, object[] args);

    /// <summary>
    ///   An instance of DelegateWrapper which calls InvokeWrappedDelegate,
    ///   which in turn calls the DynamicInvoke method of the wrapped delegate. </summary>
    private static DelegateWrapper wrapperInstance = new DelegateWrapper(InvokeWrappedDelegate);

    /// <summary>
    ///   Callback used to call <code>EndInvoke</code> on the asynchronously
    ///   invoked DelegateWrapper. </summary>
    private static AsyncCallback callback = new AsyncCallback(EndWrapperInvoke);

    /// <summary>
    ///   Executes the specified delegate with the specified arguments
    ///   asynchronously on a thread pool thread. </summary>
    public static void FireAndForget(Delegate d, params object[] args) {
      // Invoke the wrapper asynchronously, which will then
      // execute the wrapped delegate synchronously (in the
      // thread pool thread)
      wrapperInstance.BeginInvoke(d, args, callback, null);
    }

    /// <summary>
    ///   Invokes the wrapped delegate synchronously. </summary>
    private static void InvokeWrappedDelegate(Delegate d, object[] args) {
      d.DynamicInvoke(args);
    }

    /// <summary>
    ///   Calls EndInvoke on the wrapper and Close on the resulting WaitHandle
    ///   to prevent resource leaks. </summary>
    static void EndWrapperInvoke(IAsyncResult ar) {
      wrapperInstance.EndInvoke(ar);
      ar.AsyncWaitHandle.Close();
    }

  } //class ThreadUtil
} //namespace BigSister