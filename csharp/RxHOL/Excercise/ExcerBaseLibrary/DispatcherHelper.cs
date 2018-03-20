using System;
using System.Windows.Threading;

namespace ExcerBaseLibrary
{
    public static class DispatcherHelper
    {
        public static Dispatcher UIDispatcher
        {
            get;
            private set;
        }

        public static void CheckBeginInvokeOnUI(Action action)
        {
            if (UIDispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                UIDispatcher.BeginInvoke(action);
            }
        }

        public static void Initialize()
        {
            if (UIDispatcher != null)
            {
                return;
            }
            UIDispatcher = Dispatcher.CurrentDispatcher;
        }
    }
}
