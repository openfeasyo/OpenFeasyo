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
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Drivers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenFeasyo.Platform.Network.Controls
{
    public class DeviceProxy : IDevice
    {
        private IObject _remoteObj;
        private bool _isLoaded = false;
        private IGamingInput _input = null;

        public DeviceProxy(IObject remoteObj, string name, string description, string vendor, DeviceType type)
        {
            _remoteObj = remoteObj;
            _name = name;
            _vendor = vendor;
            _type = type;
        }

        

        private string _name;
        public string Name
        {
            get { return _name; } 
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public IGamingInput GamingInput
        {
            get { 
                if (_isLoaded)
                {
                    return _input;
                }
                throw new UninitializedDriverException(
                                            "Driver must be loaded first!");
            }
        }

        public void LoadDriver(Dictionary<string, string> parameters)
        {
            if (_remoteObj == null) return;

            _isLoaded = _remoteObj.LoadDevice(_name);
            if (_isLoaded) {
                InitializeInput();
            }
        }

        public void UnloadDriver()
        {
            if (_remoteObj == null) return;

            if (_isLoaded) {
                ReleaseInput();
            }
            if (!_remoteObj.UnloadDevice(_name)) { 
                Trace.WriteLine("Error: Unable to unload device: " + _name);
            }
            _isLoaded = false;
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
        }

        private string _vendor;
        public string Vendor
        {
            get { return _vendor; }
        }

        private DeviceType _type;
        public DeviceType DeviceType
        {
            get { return _type; }
        }

        private void InitializeInput() {
            switch (_type) { 
                case DeviceType.Skeleton:
                    _input = new SkeletonInputProxy(this);
                    _remoteObj.NewSkeleton += (_input as SkeletonInputProxy).OnNewSkeleton;
                    break;
                case DeviceType.BalanceBoard:
                    _input = new BalanceBoardInputProxy(this);
                    _remoteObj.NewBalanceBoard += (_input as BalanceBoardInputProxy).OnNewBalanceBoard;
                    break;
                default:
                    Trace.WriteLine("Proxy not supported for this device - " + _type.ToString());
                    break;
            }

        }

        private void ReleaseInput()
        {
            switch (_type)
            {
                case DeviceType.Skeleton:
                    _input = new SkeletonInputProxy(this);
                    _remoteObj.NewSkeleton -= (_input as SkeletonInputProxy).OnNewSkeleton;
                    break;
                case DeviceType.BalanceBoard:
                    _input = new BalanceBoardInputProxy(this);
                    _remoteObj.NewBalanceBoard -= (_input as BalanceBoardInputProxy).OnNewBalanceBoard;
                    break;
                default:
                    Trace.WriteLine("Proxy not supported for this device - " + _type.ToString());
                    break;
            }
        }

    }

}
