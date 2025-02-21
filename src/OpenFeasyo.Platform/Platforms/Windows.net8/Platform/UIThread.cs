/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
 
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
