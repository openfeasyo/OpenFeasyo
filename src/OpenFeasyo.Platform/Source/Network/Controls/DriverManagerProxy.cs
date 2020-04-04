using OpenFeasyo.Platform.Controls.Drivers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Network.Controls
{
    public class DriverManagerProxy : IDriverManager
    {
        private IObject _sharedObj;
        public DriverManagerProxy(IObject sharedObj) { 
            _sharedObj = sharedObj;
        }

        private ObservableCollection<IDevice> _devices;

        public ObservableCollection<IDevice> Drivers
        {
            get
            {
                if (_devices == null) {
                    _devices = LoadRemoteDevices();
                }
                return _devices;
            }
        }

        public void UnloadAll()
        {
            foreach (IDevice d in _devices)
            {
                if (d.IsLoaded)
                {
                    d.UnloadDriver();
                }
            }

        }

        private ObservableCollection<IDevice> LoadRemoteDevices()
        {
            ObservableCollection<IDevice> devs = new ObservableCollection<IDevice>();
            if (_sharedObj != null) { 
                string [] encodedDevices = _sharedObj.GetAvailableDevices();
                IDevice[] devices = DeviceHelper.Deserialize(_sharedObj, encodedDevices);
                foreach (IDevice d in devices)
                {
                    devs.Add(d);
                }
            }
            return devs;
        }

    }
}
