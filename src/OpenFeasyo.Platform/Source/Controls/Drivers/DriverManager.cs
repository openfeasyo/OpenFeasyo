using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections.ObjectModel;

namespace OpenFeasyo.Platform.Controls.Drivers
{
    public class DriverManager : IDriverManager
    {
        private LibraryLoader<IDevice> _inputDevices = new LibraryLoader<IDevice>();

        public static string DRIVERS_PATH = "InputDrivers";

        public ObservableCollection<IDevice> Drivers {
            get 
            {
                _inputDevices.UpdateModules(DriverManager.DRIVERS_PATH);
                return _inputDevices.LoadedModules; 
            }
        }

        public void UnloadAll()
        {
            foreach(IDevice d in Drivers){
                if (d.IsLoaded) {
                    d.UnloadDriver();
                }
            }

        }

    }
}
