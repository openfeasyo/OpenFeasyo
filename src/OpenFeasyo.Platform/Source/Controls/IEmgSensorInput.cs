using System;

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
