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
using System;
using System.Collections.Generic;

namespace OpenFeasyo.Platform.Controls
{
    public interface IDiscoverable
    {
        void ScanAsync();

        event EventHandler<ScanResultsEventArgs> ScanFinished;

        void ConnectAsync(string device);

        event EventHandler<ConnectionEventArgs> ConnectionEstablished;

        event EventHandler<ConnectionEventArgs> ConnectionFailed;

    }

    public class ScanResultsEventArgs : EventArgs
    {
        private ICollection<string> _devices;
        public ICollection<string> Devices { get { return _devices; } }

        public ScanResultsEventArgs(ICollection<string> devices) {
            _devices = devices;
        }
    }

    public class ConnectionEventArgs : EventArgs
    {
        private Object _sensor;
        private string _device;
        private string _details;
        public string Devices { get { return _device; } }
        public string Details { get { return _details; } }
        public Object Sensor { get { return _sensor; } }

        public ConnectionEventArgs(string device, string details="", Object sensor=null)
        {
            _device = device;
            _details = details;
            _sensor = sensor;
        }
    }

}
