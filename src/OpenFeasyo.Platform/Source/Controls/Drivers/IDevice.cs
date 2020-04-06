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

namespace OpenFeasyo.Platform.Controls.Drivers
{
    public enum DeviceType {
        Skeleton = 0,
        BalanceBoard = 1,
        Accelerometer = 2,
        Electromyography = 3,
        Touchscreen = 4,
        Unknown = 5        
    }

    public interface IDevice
    {

        #region Properties

        /// <summary>
        /// Name of the driver. </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Vendor of the device. </summary>
        string Vendor 
        { 
            get; 
        }

        /// <summary>
        /// Description of the driver. </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Flag that signals whether the driver is loaded. </summary>
        bool IsLoaded 
        { 
            get; 
        }

        /// <summary>
        /// Flag that signals whether the driver is loaded. </summary>
        DeviceType DeviceType 
        { 
            get; 
        }

        /// <summary>
        /// GamingInput property. </summary>
        /// <exception cref="Vub.Etro.Gaming.GamingControls.Drivers.UninitializedDriverException" >
        /// Throws an exception if the driver was not previously loaded. </exception>
        IGamingInput GamingInput
        {
            get;
        }

        #endregion

        #region Methods for initialization and deinitialization

        void LoadDriver(Dictionary<string, string> parameters);

        void UnloadDriver();

        #endregion
    }


    #region Possible driver exceptions

    [Serializable()]
    public class UninitializedDriverException : System.Exception
    {
        public UninitializedDriverException() : base() { }
        public UninitializedDriverException(string message) : base(message) { }
        public UninitializedDriverException(string message,
                             System.Exception inner)
            : base(message, inner) { }
        protected UninitializedDriverException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    public class InitializationFailedException : System.Exception
    {
        public InitializationFailedException() : base() { }
        public InitializationFailedException(string message) : base(message) { }
        public InitializationFailedException(string message,
                             System.Exception inner)
            : base(message, inner) { }
        protected InitializationFailedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    #endregion
}
