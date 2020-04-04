using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Drivers
{
    public class InputDeviceManager
    {
        private static IDriverManager _instance = null;

        public static IDriverManager Instance {
            set {
                //if (_instance != null) {
                //    throw new ApplicationException("InputDeviceManager was already initialized");
                //}
                _instance = value;
            }
        }

        public static ObservableCollection<IDevice> Drivers
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DriverManager();
                }
                return _instance.Drivers;
            }
        }

        public static void UnloadAll()
        {
            if (_instance != null)
                _instance.UnloadAll();
        }

    }
}
