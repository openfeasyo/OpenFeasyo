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
    public interface IEmgSensorInput : IGamingInput
    {
        event EventHandler<MuscleActivationChangedEventArgs> MuscleActivationChanged;

        event EventHandler<CalibrationChangedEventArgs> CalibrationChanged;

        //event EventHandler<HWEventArgs> UICallback;

        //event EventHandler<ErrorArgs> Error;

        void Calibrate();
    }

    public class MuscleActivationChangedEventArgs : EventArgs
    {
        ///<summary>
        /// Instance variable to store the unified EMG sensor. </summary>
        private IEmgSignal[] _emgSensor;

        ///<summary>
        /// Constructor that sets current balance state for the event. </summary>
        public MuscleActivationChangedEventArgs(IEmgSignal[] emgSensor) { _emgSensor = emgSensor; }

        ///<summary>
        /// Read only property for the balance board. </summary>
        public IEmgSignal[] EMGSensor { get { return _emgSensor; } }
    }


    public enum CalibrationResults {
        Started,
        Finished,
        Error
    }

    public class CalibrationChangedEventArgs : EventArgs
    {
        public CalibrationResults CalibrationEvent { get; set; }
        public List<double>[] CalibrationsData { get; set; }
        public double[] ZeroMean { get; set; }
        public double[] ZeroStandardDeviation { get; set; }
        
public CalibrationChangedEventArgs(CalibrationResults calibEvent) {
            CalibrationEvent = calibEvent;
        }
    }

    //public enum State
    //{
    //    Connected,
    //    Disconnected,
    //    Streaming
    //}
    //public class HWEventArgs : EventArgs
    //{
    //    public State State { get; set; }

    //    public HWEventArgs(State state)
    //    {
    //        this.State = state;
    //    }
    //}

    //public enum ErrorType
    //{
    //    ComNotFound
    //}

    //public class ErrorArgs : EventArgs
    //{
    //    public ErrorArgs(ErrorType err) { this.Err = err; }

    //    public ErrorType Err { get; set; }
    //}
}
