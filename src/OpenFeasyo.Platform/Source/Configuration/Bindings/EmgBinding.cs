using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Analysis;
using OpenFeasyo.Platform.Platform;
using System;
using System.Collections.Generic;


namespace OpenFeasyo.Platform.Configuration.Bindings
{
    public class EmgBinding : AnalysableBinding
    {
        private IEmgSensorInput _input;

        internal override IGamingInput Input
        {
            get { return _input; }
        }

        private int _channel;
        public int Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                if (value != _channel)
                {
                    _input.MuscleActivationChanged -= _event_handler;
                    _channel = value;
                    _event_handler = new EventHandler<MuscleActivationChangedEventArgs>(_event_handle);
                    _input.MuscleActivationChanged += _event_handler;
                }
            }
        }


        private EventHandler<MuscleActivationChangedEventArgs> _event_handler;
        private EventHandler<MuscleActivationChangedEventArgs> _analyzer_handler;


        public EmgBinding(IEmgSensorInput input, Configuration.InputValueHandle handle, int channel) : base(handle, 0, 1)
        {
            this._input = input;
            this._channel = channel;

            _event_handler = new EventHandler<MuscleActivationChangedEventArgs>(_event_handle);
            _input.MuscleActivationChanged += _event_handler;

            _analyzer_handler = new EventHandler<MuscleActivationChangedEventArgs>(_analyzer_handle);
            input.MuscleActivationChanged += _analyzer_handler;
        }

        void _event_handle(object sender, MuscleActivationChangedEventArgs e)
        {
            if(e.EMGSensor.Length > _channel) { 
                CallHandle(0, e.EMGSensor[_channel].MuscleActivated ? 1 : 0);
            }
        }

        internal override void destroy()
        {
            this._input.MuscleActivationChanged -= _event_handler;
            this._input.MuscleActivationChanged -= _analyzer_handler;

            foreach (AnalyzerWrapper executor in _analyzerWrapers)
            {
                executor.Stop();
            }
        } 

        private HashSet<AnalyzerWrapper> _analyzerWrapers = new HashSet<AnalyzerWrapper>();

        public void AddAnalyzer(IBalanceBoardAnalyzer analyzer, ObservableDictionary<string, string> parameters)
        {
            AnalyzerWrapper executor = new AnalyzerWrapper(analyzer, parameters);
            executor.Run();
            _analyzerWrapers.Add(executor);
            _analyzers.Add(analyzer, parameters);
        }

        void _analyzer_handle(object sender, MuscleActivationChangedEventArgs e)
        {
            foreach (AnalyzerWrapper analyzer in _analyzerWrapers)
            {
                analyzer.ProcessEmgSignal(e.EMGSensor);
            }
        }
    }
}
