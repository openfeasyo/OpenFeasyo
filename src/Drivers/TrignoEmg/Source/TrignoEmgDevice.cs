/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Katarina Kostkova
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using DelsysAPI.Components.TrignoBT;
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Drivers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TrignoEmg
{
    public class TrignoEmgDevice : IDevice
    {
        #region Private members

        private bool _isLoaded;

        private Dictionary<string, SensorTrignoBt> _sensors = new Dictionary<string, SensorTrignoBt>();


        private TrignoEmgInput _emgSensorInput;

        #endregion

        #region Properties

        public string Description
        {
            get { return "TrignoEmg"; }
        }

        public DeviceType DeviceType
        {
            get { return DeviceType.Electromyography; }
        }

        public IGamingInput GamingInput
        {
            get
            {
                if (_isLoaded)
                {
                    return _emgSensorInput;
                }
                throw new Exception("Driver must be loaded first!");
            }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
        }

        public string Name
        {
            get { return "TrignoEmg"; }
        }

        public int RemainingBattery
        {
            get
            {
                return -1;
            }
        }

        public string Vendor
        {
            get { return "FlexVolt Bio Sensor"; }
        }

        #endregion

        #region Methods for initialization and deinitialization

        public void LoadDriver(Dictionary<string, string> parameters)
        {
            _emgSensorInput = new TrignoEmgInput(this, parameters);

            try
            {
                //_emgSensorInput.Start();
                _isLoaded = true;
            }
            catch (Exception e)
            {
                Trace.TraceError("FlexVolt Emg sensor: " + e.Message);
                Trace.TraceError(e.StackTrace);

                _emgSensorInput = null;
                _isLoaded = false;
            }
        }

        public void UnloadDriver()
        {
            if (_emgSensorInput != null)
            {
                _emgSensorInput.Destroy();
                _isLoaded = false;
                _emgSensorInput = null;
            }
        }


        #endregion

        public event EventHandler<EventArgs> Loaded;
        protected virtual void OnLoaded(EventArgs e)
        {
            if (Loaded != null)
            {
                Loaded(this, e);
            }
        }

        //public event EventHandler<HWEventArgs> UICallback;
        //protected virtual void OnUICallback(HWEventArgs e)
        //{
        //    if (UICallback != null)
        //    {
        //        UICallback(this, e);
        //    }
        //}

        //public event EventHandler<ErrorArgs> Error;
        //protected virtual void OnError(ErrorArgs e)
        //{
        //    if (Error != null)
        //    {
        //        Error(this, e);
        //    }
        //}

        
    }
}
