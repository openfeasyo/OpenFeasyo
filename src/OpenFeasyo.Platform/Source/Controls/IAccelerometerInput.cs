using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls
{
    public enum AccelerometerState
    {
        Disconnected = 0,
        Disconnecting,
        Discovering,
        Connecting,
        Connected,
        NotAvailableOnPort,
        Error
    }

    ///<summary>
    /// Interface for accelerometer based inputs for rehabilitation games.</summary>
    public interface IAccelerometerInput : IGamingInput
    {
        event EventHandler<AccelerometerChangedEventArgs> AccelerometerChanged;

        event EventHandler<AccelerometerStateEventArgs> AccelerometerStateChanged;

        AccelerometerState State
        {
            get;
        }

        ObservableCollection<string> AvailableDevices
        {
            get;
        }

        string AccelerometerDevice
        {
            get;
        }

        void Start(string deviceName);
        void Stop();
    }

    ///<summary>
    /// Specialized event class for the accelerometerchanged event. It contains
    /// absolute angels in 3 axis. </summary>
    public class AccelerometerChangedEventArgs : EventArgs
    {

        ///<summary>
        /// Instance variable to store the unified accelerometer data. </summary>
        private IAccelerometer _accelerometer;

        ///<summary>
        /// Constructor that sets current accelerometer data for the event. </summary>
        public AccelerometerChangedEventArgs(IAccelerometer accelerometerData) { _accelerometer = accelerometerData; }

        ///<summary>
        /// Read only property for the accelerometer. </summary>
        public IAccelerometer AccelerometerData { get { return _accelerometer; } }

    }

    public class AccelerometerStateEventArgs : EventArgs
    {

        ///<summary>
        /// Instance variable to store the accelerometer state. </summary>
        private AccelerometerState _state;
        private string _message;

        ///<summary>
        /// Constructor that sets current accelerometer state for the event. </summary>
        public AccelerometerStateEventArgs(AccelerometerState state) { _state = state; _message = ""; }
        public AccelerometerStateEventArgs(AccelerometerState state, string message) { _state = state; _message = message; }

        ///<summary>
        /// Read only property for the state of an accelerometer. </summary>
        public AccelerometerState State { get { return _state; } }

    }
}
