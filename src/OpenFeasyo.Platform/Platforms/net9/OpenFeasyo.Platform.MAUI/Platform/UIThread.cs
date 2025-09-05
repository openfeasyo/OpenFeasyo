#if ANDROID
    using Android.App;
 #endif

#if __IOS__
    using UIKit;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OpenFeasyo.Platform.Platform
{
    public abstract class UIThread
    {
        private static UIThread _uithread = null;

        public static UIThread Instance
        {
            get { return _uithread; }
            set { _uithread = value; }
        }

        public static void Invoke(Action a)
        {
            if (_uithread != null)
            {
                _uithread.InternalInvoke(a);
            }
        }

        public static void ShowMessage(String title, String message)
        {
            if (_uithread != null)
            {
                _uithread.InternalShowMessage(title, message);
            }
        }

        internal abstract void InternalInvoke(Action a);

        internal abstract void InternalShowMessage(String title, String message);
    }
#if __IOS__
    public class iOSUIThread : UIThread
    {
        private UIApplicationDelegate _controller;
        public iOSUIThread(UIApplicationDelegate controller) {
            _controller = controller;
        }

        internal override void InternalInvoke(Action a)
        {
            _controller.InvokeOnMainThread(a);
        }

        internal override void InternalShowMessage(string title, string message)
        {
            _controller.InvokeOnMainThread(() => {
                //Toast.MakeText(_activity, message, ToastLength.Long).Show();
                Console.WriteLine(message);
            }); 
        }
    }
#endif

#if ANDROID
    public class AndroidUIThread : UIThread
    {
        private Activity _activity;
        public AndroidUIThread(Activity activity) {
            _activity = activity;
        }

        internal override void InternalInvoke(Action a)
        {
            _activity.RunOnUiThread(a);
        }

        internal override void InternalShowMessage(string title, string message)
        {
            _activity.RunOnUiThread(() => {
                //Toast.MakeText(_activity, message, ToastLength.Long).Show();
                Console.WriteLine(message);
            }); 
        }
    }
#endif
}
