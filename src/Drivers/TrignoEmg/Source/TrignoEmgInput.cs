/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Katarina Kostkova, Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DelsysAPI.Channels.Transform;
using DelsysAPI.Components.TrignoBT;
using DelsysAPI.Configurations;
using DelsysAPI.Configurations.Component;
using DelsysAPI.Configurations.DataSource;
using DelsysAPI.Contracts;
using DelsysAPI.DelsysDevices;
using DelsysAPI.Events;
using DelsysAPI.Pipelines;
using DelsysAPI.Transforms;
using DelsysAPI.Utils;
using DelsysAPI.Utils.TrignoBt;
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Drivers;

namespace TrignoEmg
{

    public enum CalibrationState {
        Uncalibrated,
        Calibrating,
        Calibrated
    }

    public class TrignoEmgInput : IEmgSensorInput, IDiscoverable
    {

        #region EMG signal processing related variables
        private int _samplingRate = 1000;

        private BandPassFilter[] _bandPassFilters;
        private int _nChannels =2;

        private double _movingWindowLength = 1;
        private List<double>[] _movingWindowData;

        private List<double>[] _baselineData;
        private int[] _baselineDataCounters;

        private int _baselineDataLength = 100;
        private int _baselineThrowOut = 20;

        private double[] _baselineMean;
        private double[] _baselineStdev;
        #endregion

        private TrignoEmgSignal[] _signals = new TrignoEmgSignal[4];
        private TrignoEmgDevice _device;

        private CalibrationState _calibrationState = CalibrationState.Uncalibrated;

        private Dictionary<Guid, string> _guidToSensor = new Dictionary<Guid, string>();
        private Dictionary<string,int> _sensorToChannel = new Dictionary<string,int>();

        public IDevice Device {
            get { return _device; }
        }

        public float[] ActivationThreshold { get; set; }

        private Dictionary<string, SensorTrignoBt> _sensors = new Dictionary<string, SensorTrignoBt>();


        public TrignoEmgInput(TrignoEmgDevice device, Dictionary<string, string> parameters) {
            _device = device;
            InitializeSignalProcessing();
            InitializeDataSource();
            ActivationThreshold = new float[2];
        }

        #region Input events

        public event EventHandler<MuscleActivationChangedEventArgs> MuscleActivationChanged;
        private void OnMuscleActivationChanged(MuscleActivationChangedEventArgs args) {
            if (MuscleActivationChanged != null) {
                MuscleActivationChanged(this, args);
            }
        }

        public event EventHandler<ScanResultsEventArgs> ScanFinished;
        private void OnScanFinished(ICollection<string> sensors) {
            if (ScanFinished != null) {
                ScanFinished(this, new ScanResultsEventArgs(sensors));
            }
        }

        public event EventHandler<ConnectionEventArgs> ConnectionEstablished;
        private void OnConnectionEstablished(string device)
        {
            if (ConnectionEstablished != null)
            {
                ConnectionEstablished(this, new ConnectionEventArgs(device));
            }
        }

        public event EventHandler<ConnectionEventArgs> ConnectionFailed;
        private void OnConnectionFailed(string device) {
            if (ConnectionFailed != null) {
                ConnectionFailed(this, new ConnectionEventArgs(device));
            }
        }

        public event EventHandler<CalibrationChangedEventArgs> CalibrationChanged;
        private void OnCalibrationChanged(CalibrationResults calibEvent) {
            if (CalibrationChanged != null) {
                CalibrationChangedEventArgs args = new CalibrationChangedEventArgs(calibEvent);
                if (calibEvent == CalibrationResults.Finished) {
                    args.CalibrationsData = _baselineData;
                    args.ZeroMean = _baselineMean;
                    args.ZeroStandardDeviation = _baselineStdev;
                }
                CalibrationChanged(this, args);
            }
        }

        #endregion 

        internal void Destroy() {
            BTPipeline.Stop();
        }

        #region Delsys related code
 
        private Pipeline BTPipeline;
        private ITransformManager  TransformManager;
        private string[] DeviceFilters = new string[] {};
        private IDelsysDevice DeviceSource = null;

        public void InitializeDataSource()
        {
            var assembly = Assembly.GetCallingAssembly();
            string key;
            using (Stream stream = assembly.GetManifestResourceStream("TrignoEmg.PublicKey.lic"))
            {
                StreamReader sr = new StreamReader(stream);
                key = sr.ReadLine();
            }
            string license;
            using (Stream stream = assembly.GetManifestResourceStream("TrignoEmg.OpenFeasyo.lic"))
            {
                StreamReader sr = new StreamReader(stream);
                license = sr.ReadToEnd();
            }

#if ANDROID
            var deviceSourceCreator = new DelsysAPI.Android.DeviceSourcePortable(key, license);
#else
            var deviceSourceCreator = new DelsysAPI.NET.DeviceSourcePortable(key, license);
#endif
            deviceSourceCreator.SetDebugOutputStream(Console.WriteLine);
            
            DeviceSource = deviceSourceCreator.GetDataSource(SourceType.TRIGNO_BT);
            DeviceSource.Key = key;
            DeviceSource.License = license;
            LoadDataSource(DeviceSource);
        }

        public void LoadDataSource(IDelsysDevice ds)
        {
            // if the pipeline has not been initialized yet
            if(PipelineController.Instance.PipelineIds.Count == 0)
            { 
                PipelineController.Instance.AddPipeline(ds);
            }

            BTPipeline = PipelineController.Instance.PipelineIds[0];
            TransformManager = PipelineController.Instance.PipelineIds[0].TransformManager;

            // Device Filters allow you to specify which sensors to connect to
            foreach (var filter in DeviceFilters)
            {
                BTPipeline.TrignoBtManager.AddDeviceIDFilter(filter);
            }

            BTPipeline.CollectionStarted += CollectionStarted;
            BTPipeline.CollectionDataReady += CollectionDataReady;
            BTPipeline.CollectionComplete += CollectionComplete;

            BTPipeline.TrignoBtManager.ComponentScanComplete += ComponentScanComplete;
        }

        private void ComponentScanComplete(object sender, DelsysAPI.Events.ComponentScanCompletedEventArgs e)
        {
            Console.WriteLine("ComponentScanComplete: " + e.ComponentDictionary.Count.ToString());
            _sensors.Clear();
            for (int i = 0; i < BTPipeline.TrignoBtManager.Components.Count; i++)
            {
                Console.WriteLine(" - ComponentScanComplete: " + BTPipeline.TrignoBtManager.Components[i].Properties.SerialNumber.ToString() + (i == BTPipeline.TrignoBtManager.Components.Count - 1 ? "" : ", "));
                Console.WriteLine(" - ComponentScanComplete: " + "Added a type {0} sensor . . . ", BTPipeline.TrignoBtManager.Components[i].Properties.SensorType);
                _sensors.Add(BTPipeline.TrignoBtManager.Components[i].Properties.SerialNumber.ToString(), BTPipeline.TrignoBtManager.Components[i]);
            }
            OnScanFinished(_sensors.Keys);
        }

        public void ComponentAdded(object sender, ComponentAddedEventArgs e)
        {
            Console.WriteLine("ComponentAdded");
        }

        public void ComponentLost(object sender, ComponentLostEventArgs e)
        {
            Console.WriteLine("ComponentLost");
        }

        public void ComponentRemoved(object sender, ComponentRemovedEventArgs e)
        {
            Console.WriteLine("ComponentRemoved");
        }

        public void CollectionDataReady(object sender, ComponentDataReadyEventArgs e)
        {
           // Console.WriteLine("@");

            TransformData[] tData = new TransformData[e.Data.Length];
            foreach (TransformData td in e.Data) {
                tData[_sensorToChannel[_guidToSensor[td.Id]]] = td;
            }
            double[][] data = new double[e.Data.Length][];
            //if (tData.Length >= 2 && tData[0].Data.Count == tData[1].Data.Count)
            //{
            //    var channelData = tData[0];
            //    for (int i = 0; i < channelData.Data.Count; i++)
            //    {
            //        for (int j = 0; j < tData.Length; j++)
            //        {
            //            data[j][i] = tData[j].Data[i];
            //        }
            //        switch (_calibrationState)
            //        {
            //            case CalibrationState.Calibrating:
            //                Train(data);
            //                OnMuscleActivationChanged(new MuscleActivationChangedEventArgs(null));
            //                break;
            //            case CalibrationState.Calibrated:
            //                OnMuscleActivationChanged(new MuscleActivationChangedEventArgs(Process(data)));
            //                break;
            //            default: //CalibrationState.Uncalibrated
            //                for (int c = 0; c < data.Length; c++)
            //                {
            //                    _signals[c] = new TrignoEmgSignal(data[c]);
            //                }
            //                OnMuscleActivationChanged(new MuscleActivationChangedEventArgs(_signals));
            //                break;
            //        }

            //    }
            //}
            if (tData.Length >= 2 && tData[0].Data.Count == tData[1].Data.Count)
            {
                for (int channel = 0; channel < tData.Length; channel++)
                {
                    data[channel] = new double[tData[channel].Data.Count];
                    for (int sample = 0; sample < tData[channel].Data.Count; sample++)
                    {
                        data[channel][sample] = tData[channel].Data[sample];
                    }
                }
                switch (_calibrationState)
                {
                    case CalibrationState.Calibrating:
                        Train(data);
                        OnMuscleActivationChanged(new MuscleActivationChangedEventArgs(null));
                        break;
                    case CalibrationState.Calibrated:
                        OnMuscleActivationChanged(new MuscleActivationChangedEventArgs(Process(data)));
                        break;
                    default: //CalibrationState.Uncalibrated
                        for (int c = 0; c < data.Length; c++)
                        {
                            _signals[c] = new TrignoEmgSignal(data[c]);
                        }
                        OnMuscleActivationChanged(new MuscleActivationChangedEventArgs(_signals));
                        break;
                }
            }
            else {
                Console.Write("E");
            }

        }

        private void CollectionStarted(object sender, DelsysAPI.Events.CollectionStartedEvent e)
        {
            Console.WriteLine("CollectionStarted");
            //var comps = PipelineController.Instance.PipelineIds[0].TrignoBtManager.Components;
        }

        private void CollectionComplete(object sender, DelsysAPI.Events.CollectionCompleteEvent e)
        {
            Console.WriteLine("CollectionComplete");
        }

        private bool CallbacksAdded = false;
        private bool ConfigurePipeline()
        {
            if (CallbacksAdded)
            {
                BTPipeline.TrignoBtManager.ComponentAdded -= ComponentAdded;
                BTPipeline.TrignoBtManager.ComponentLost -= ComponentLost;
                BTPipeline.TrignoBtManager.ComponentRemoved -= ComponentRemoved;
            }
            BTPipeline.TrignoBtManager.ComponentAdded += ComponentAdded;
            BTPipeline.TrignoBtManager.ComponentLost += ComponentLost;
            BTPipeline.TrignoBtManager.ComponentRemoved += ComponentRemoved;
            CallbacksAdded = true;

            PipelineController.Instance.PipelineIds[0].TrignoBtManager.Configuration = new TrignoBTConfig() { EOS = EmgOrSimulate.EMG };

            var inputConfiguration = new BTDsConfig();
            inputConfiguration.NumberOfSensors = BTPipeline.TrignoBtManager.Components.Count;
            foreach (var somecomp in BTPipeline.TrignoBtManager.Components)
            {
                if (somecomp.State != SelectionState.Allocated) continue;

                string selectedMode = "EMG (1000 Hz)";//"EMG+IMU,ACC:+/-2g,GYRO:+/-250dps";
                somecomp.SensorConfiguration.SelectSampleMode(selectedMode);

                if (somecomp.SensorConfiguration == null)
                {
                    return false;
                }
            }

            PipelineController.Instance.PipelineIds[0].ApplyInputConfigurations(inputConfiguration);
            var transformTopology = GenerateTransforms();
            PipelineController.Instance.PipelineIds[0].ApplyOutputConfigurations(transformTopology);
            PipelineController.Instance.PipelineIds[0].RunTime = Double.MaxValue;
            return true;
        }

        private OutputConfig GenerateTransforms()
        {
            // Clear the previous transforms should they exist.
            TransformManager.TransformList.Clear();

            int channelNumber = 0;
            // Obtain the number of channels based on our sensors and their mode.
            for (int i = 0; i < BTPipeline.TrignoBtManager.Components.Count; i++)
            {
                if (BTPipeline.TrignoBtManager.Components[i].State == SelectionState.Allocated)
                {
                    var tmp = BTPipeline.TrignoBtManager.Components[i];

                    BTCompConfig someconfig = tmp.SensorConfiguration as BTCompConfig;
                    if (someconfig.IsComponentAvailable())
                    {
                        channelNumber += BTPipeline.TrignoBtManager.Components[i].BtChannels.Count;
                    }

                }
            }

            // Create the raw data transform, with an input and output channel for every
            // channel that exists in our setup. This transform applies the scaling to the raw
            // data from the sensor.
            var rawDataTransform = new TransformRawData(channelNumber, channelNumber);
            PipelineController.Instance.PipelineIds[0].TransformManager.AddTransform(rawDataTransform);

            // The output configuration for the API to use.
            _guidToSensor.Clear();
            var outconfig = new OutputConfig();
            outconfig.NumChannels = channelNumber;

            int channelIndex = 0;
            for (int i = 0; i < BTPipeline.TrignoBtManager.Components.Count; i++)
            {
                if (BTPipeline.TrignoBtManager.Components[i].State == SelectionState.Allocated)
                {
                    BTCompConfig someconfig = BTPipeline.TrignoBtManager.Components[i].SensorConfiguration as BTCompConfig;
                    if (someconfig.IsComponentAvailable())
                    {
                        // For every channel in every sensor, we gather its sampling information (rate, interval, units) and create a
                        // channel transform (an abstract channel used by transforms) from it. We then add the actual component's channel
                        // as an input channel, and the channel transform as an output. 
                        // Finally, we map the channel counter and the output channel. This mapping is what determines the channel order in
                        // the CollectionDataReady callback function.
                        for (int k = 0; k < BTPipeline.TrignoBtManager.Components[i].BtChannels.Count; k++)
                        {
                            var chin = BTPipeline.TrignoBtManager.Components[i].BtChannels[k];
                            ChannelTransform chout = new ChannelTransform(chin.FrameInterval, chin.SamplesPerFrame, BTPipeline.TrignoBtManager.Components[i].BtChannels[k].Unit);
                            TransformManager.AddInputChannel(rawDataTransform, chin);
                            TransformManager.AddOutputChannel(rawDataTransform, chout);
                            Guid tmpKey = outconfig.MapOutputChannel(channelIndex, chout);
                            _guidToSensor.Add(chout.Id, BTPipeline.TrignoBtManager.Components[i].Properties.SerialNumber);
                            channelIndex++;
                        }
                    }
                }
            }
            return outconfig;
        }

        public void ScanAsync()
        {
            BTPipeline.Scan();
        }

        public void ConnectAsync(string devices)
        {
            string[] sensors = devices.Split(';');
            for (int i = 0; i < sensors.Length; i++)
            {
                if (!_sensors.ContainsKey(sensors[i]))
                {
                    OnConnectionFailed(sensors[i]);
                    continue;
               }
                Console.WriteLine("Selecting sensor '" + sensors[i] + "' - '" + _sensors[sensors[i]].Id + "'");
                BTPipeline.TrignoBtManager.SelectComponentAsync(_sensors[sensors[i]]).Wait();
                _sensorToChannel.Add(sensors[i], i);
            }
            ConfigurePipeline();
            BTPipeline.Start();
        }


        #endregion

        #region EMG signal processing

        private void InitializeSignalProcessing()
        {
            this._movingWindowLength = Math.Floor((float)_samplingRate/ 10);
            this._baselineDataLength = (int)_samplingRate*2;
            this._baselineThrowOut = (int)Math.Floor((float)_baselineDataLength / 5);

            this._movingWindowData = new List<double>[_nChannels];
            this._baselineData = new List<double>[_nChannels];
            this._bandPassFilters = new BandPassFilter[_nChannels];

            this._baselineDataCounters = new int[_nChannels];
            this._baselineMean = new double[_nChannels];
            this._baselineStdev = new double[_nChannels];

            for (int i = 0; i < _nChannels; i++)
            {
                this._movingWindowData[i] = new List<double>();
                this._baselineData[i] = new List<double>();
                this._bandPassFilters[i] = new BandPassFilter(BandPassFilter.BAND_PASS, _samplingRate, new double[] { 5, 25 }, 6);

                this._baselineDataCounters[i] = 0;
                this._baselineMean[i] = -1;
                this._baselineStdev[i] = -1;
            }
        }

        private void Train(double[] rawData)
        {
            int count = Math.Min(_nChannels, rawData.Length);

            for (int index = 0; index < count; index++)
            {
                double filtered = _bandPassFilters[index].filterData(rawData[index]);

                double value = FullWaveRectification(filtered)[0];

                if (_baselineDataCounters[index] < _baselineThrowOut)
                {
                    //throw these first few away
                }
                else if (_baselineDataCounters[index] < _baselineDataLength + _baselineThrowOut)
                {
                    _baselineData[index].Add(value);
                }
                else if (_baselineDataCounters[index] == _baselineDataLength + _baselineThrowOut)
                {
                    _baselineMean[index] = Mean(_baselineData[index]);
                    _baselineStdev[index] = StandardDeviation(_baselineData[index], _baselineMean[index]);

                    OnCalibrationChanged(CalibrationResults.Finished);
                    _calibrationState = CalibrationState.Calibrated;
                }

                _baselineDataCounters[index]++;
            }
        }

        private void Train(double[][] rawData)
        {
            int count = Math.Min(_nChannels, rawData.Length);

            for (int channel = 0; channel < count; channel++)
            {
                double[] filtered = _bandPassFilters[channel].filterData(rawData[channel]);

                for(int i = 0; i < rawData[channel].Length; i++) { 
                    double value = FullWaveRectification(filtered[i])[0];


                    if (_baselineDataCounters[channel] < _baselineThrowOut)
                    {
                        //throw these first few away
                    }
                    else if (_baselineDataCounters[channel] < _baselineDataLength + _baselineThrowOut)
                    {
                        _baselineData[channel].Add(value);
                    }
                    else if (_baselineDataCounters[channel] == _baselineDataLength + _baselineThrowOut)
                    {
                        _baselineMean[channel] = Mean(_baselineData[channel]);
                        _baselineStdev[channel] = StandardDeviation(_baselineData[channel], _baselineMean[channel]);

                        OnCalibrationChanged(CalibrationResults.Finished);
                        _calibrationState = CalibrationState.Calibrated;
                    }
                    else {
                        break;
                    }

                    _baselineDataCounters[channel]++;
                }
            }
        }

        //private TrignoEmgSignal[] Process(double [] rawData)
        //{
        //    int count = Math.Min(_nChannels, rawData.Length);
        //    TrignoEmgSignal[] signals = new TrignoEmgSignal[count];


        //    for (int index = 0; index < count; index++)
        //    {
        //        TrignoEmgSignal signal = new TrignoEmgSignal();

        //        signal.RawSample = rawData[index];
        //        signal.BpfSample = _bandPassFilters[index].filterData(signal.RawSample );
        //        signal.FullWaveSample = FullWaveRectification(signal.BpfSample)[0];

        //        signals[index] = MovingWindowAverageFilter(signal, index);
        //    }

        //    return signals;
        //}

        private TrignoEmgSignal[] Process(double[][] rawData)
        {
            int count = Math.Min(_nChannels, rawData.Length);
            TrignoEmgSignal[] signals = new TrignoEmgSignal[count];


            for (int index = 0; index < count; index++)
            {
                TrignoEmgSignal signal = new TrignoEmgSignal(rawData[index]);
                
                signal.BpfSample = _bandPassFilters[index].filterData(signal.RawSample);
                FullWaveRectification(signal.BpfSample, signal.FullWaveSample);
                signals[index] = MovingWindowAverageFilter(signal, index);
                
            }

            return signals;
        }

        private double[] FullWaveRectification(Double value)
        {
            return new double[] { Math.Abs(value) };
        }

        private void FullWaveRectification(double [] values, double[] destination)
        {
            //TODO use vector approach
            for (int i = 0; i < values.Length; i++) {
                destination[i] = Math.Abs(values[i]);
            }
        }

        private TrignoEmgSignal MovingWindowAverageFilter(TrignoEmgSignal signal, int index)
        {
            bool activated = false;
            double onOff = 0;
            for (int i = 0; i < signal.FullWaveSample.Length; i++) {
                if (_movingWindowData[index].Count == _movingWindowLength)
                {
                    _movingWindowData[index].RemoveAt(0);
                    _movingWindowData[index].Add(signal.FullWaveSample[i]);
                }
                else
                {
                    _movingWindowData[index].Add(signal.FullWaveSample[i]);
                }

                double currentMean = Mean(_movingWindowData[index]);

                signal.AveragedSample[i] = currentMean;

                double threshold = ActivationThreshold[index] <= 0 ? (_baselineMean[index] + (3 * _baselineStdev[index])) : ActivationThreshold[index];

                if (currentMean > threshold)
                {
                    onOff = currentMean;
                }
                else
                {
                    onOff = 0;
                }

                signal.OnOff[i] = onOff;
                activated = activated || !(onOff == 0);
                signal.RestingMean[i] = _baselineMean[index];
                signal.RestingStdev[i] = _baselineStdev[index];

            }
            signal.MuscleActivated = activated;

            return signal;
        }

        private double Mean(List<double> data)
        {
            Double sum = 0;
            foreach (Double val in data)
            {
                sum += val;
            }

            return sum / data.Count;
        }

        private double StandardDeviation(List<double> data, double mean)
        {
            Double res = 0;
            foreach (Double value in data)
            {
                res += (value - mean) * (value - mean);
            }
            return Math.Sqrt(res / data.Count);
        }

        public void Calibrate()
        {
            _baselineDataCounters = new int[2] { 0, 0 };
            _baselineData = new List<double>[2] { new List<double>(), new List<double>() };
            _baselineMean = new double[] { -1, -1 };
            _baselineStdev = new double[] { -1, -1 };
            _calibrationState = CalibrationState.Calibrating;
            OnCalibrationChanged(CalibrationResults.Started);
        }
        #endregion
    }




}
