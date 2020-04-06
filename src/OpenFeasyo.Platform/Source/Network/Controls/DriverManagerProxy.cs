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
using OpenFeasyo.Platform.Controls.Drivers;
using System.Collections.ObjectModel;

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
