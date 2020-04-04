#if ANDROID
    using Android.App;
    using Android.Widget;
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
                _uithread.InternalShowMessage(title,message);
            }
        }

        internal abstract void InternalInvoke(Action a);

        internal abstract void InternalShowMessage(String title, String message);
    }

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
