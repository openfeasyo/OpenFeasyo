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
using OpenFeasyo.Platform.Configuration.Bindings;
using OpenFeasyo.Platform.Configuration.Xml;
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Analysis;
using OpenFeasyo.Platform.Controls.Drivers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace OpenFeasyo.Platform.Configuration
{
    public class ObservableDictionary<K, V> : Dictionary<K, V>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        internal new void Add(K key, V value)
        {
            base.Add(key, value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, key));
        }

        internal new void Remove(K key)
        {
            base.Remove(key);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 0));
        }

        internal new void Clear()
        {
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool HasKey(K key)
        {
            return base.ContainsKey(key);
        }
    }

    public class InputAnalyzer
    {
        protected ObservableDictionary<string, string> _parameters;
        public ObservableDictionary<string, string> Parameters
        {
            get { return _parameters; }
        }



        private AnalyzerWrapper _analyzerWrapper;

        private IAnalyzer _analyzer;
        public IAnalyzer Analyzer { get { return _analyzer; } }

        private IGamingInput _input;

        private IDevice _device;
        public IDevice Device { get { return _device; } }

        private EventHandler<SkeletonChangedEventArgs> _skeleton_handler;
        private EventHandler<AccelerometerChangedEventArgs> _accelerometer_handler;

        private EventHandler<BalanceChangedEventArgs> _ballanceBoard_handler;
        private EventHandler<MuscleActivationChangedEventArgs> _emg_handler;


        internal InputAnalyzer(ObservableDictionary<string, string> parameters, IDevice device, IAnalyzer analyzer)
        {
            _parameters = parameters;
            _analyzer = analyzer;

            _analyzerWrapper = new AnalyzerWrapper(analyzer, parameters);
            _analyzerWrapper.Run();

            _device = device;
            _input = device.GamingInput;

            if (_input is ISkeletonInput && analyzer is ISkeletonAnalyzer)
            {
                (_input as ISkeletonInput).SkeletonChanged +=
                    (_skeleton_handler = new EventHandler<SkeletonChangedEventArgs>(InputAnalyzer_SkeletonChanged));
            }
            else if (_input is IAccelerometerInput && analyzer is IAccelerometerAnalyzer)
            {
                (_input as IAccelerometerInput).AccelerometerChanged +=
                    (_accelerometer_handler = new EventHandler<AccelerometerChangedEventArgs>(InputAnalyzer_AccelerometerChanged));
            }
            else if (_input is IBalanceBoardInput && analyzer is IBalanceBoardAnalyzer)
            {
                (_input as IBalanceBoardInput).BalanceChanged +=
                    (_ballanceBoard_handler = new EventHandler<BalanceChangedEventArgs>(InputAnalyzer_BalanceChanged));
            }
            else if (_input is IEmgSensorInput && analyzer is IEmgSignalAnalyzer)
            {
                (_input as IEmgSensorInput).MuscleActivationChanged +=
                    (_emg_handler = new EventHandler<MuscleActivationChangedEventArgs>(InputAnalyzer_MuscleActivationChanged));
            }
        }

        void InputAnalyzer_AccelerometerChanged(object sender, AccelerometerChangedEventArgs e)
        {
            _analyzerWrapper.ProcessAccelerometer(e.AccelerometerData);
        }

        void InputAnalyzer_SkeletonChanged(object sender, SkeletonChangedEventArgs e)
        {
            _analyzerWrapper.ProcessSkeleton(0, e.Skeleton);
        }

        void InputAnalyzer_BalanceChanged(object sender, BalanceChangedEventArgs e)
        {
            _analyzerWrapper.ProcessBalanceBoard(e.Balance);
        }

        void InputAnalyzer_MuscleActivationChanged(object sender, MuscleActivationChangedEventArgs e)
        {
            _analyzerWrapper.ProcessEmgSignal(e.EMGSensor);
        }

        public void Destroy()
        {

            if (_input is ISkeletonInput && _analyzer is ISkeletonAnalyzer)
            {
                (_input as ISkeletonInput).SkeletonChanged -= _skeleton_handler;
            }
            else if (_input is IAccelerometerInput && _analyzer is IAccelerometerAnalyzer)
            {
                (_input as IAccelerometerInput).AccelerometerChanged -= _accelerometer_handler;
            }
            else if (_input is IBalanceBoardInput && _analyzer is IBalanceBoardAnalyzer)
            {
                (_input as IBalanceBoardInput).BalanceChanged -= InputAnalyzer_BalanceChanged;
            }
            else if (_input is IEmgSensorInput && _analyzer is IEmgSignalAnalyzer)
            {
                (_input as IEmgSensorInput).MuscleActivationChanged -= InputAnalyzer_MuscleActivationChanged;
            }

            _analyzerWrapper.Stop();
        }

    }

    public class Configuration : INotifyPropertyChanged
    {

        #region Properties

        protected ObservableDictionary<InputValueHandle, InputBinding> _usedHandles;

        public ObservableDictionary<InputValueHandle, InputBinding> Bindings
        {
            get { return _usedHandles; }
        }

        protected ObservableDictionary<string, ObservableDictionary<string, string>> _analyzerParameters;
        public ObservableDictionary<string, ObservableDictionary<string, string>> AnalyzersParameters
        {
            get { return _analyzerParameters; }
        }

        protected ObservableCollection<InputAnalyzer> _inputAnalyzers;
        public ObservableCollection<InputAnalyzer> InputAnalyzers
        {
            get { return _inputAnalyzers; }
        }

        protected ObservableDictionary<string, ObservableDictionary<string, string>> _deviceParameters;
        public ObservableDictionary<string, ObservableDictionary<string, string>> DeviceParameters
        {
            get { return _deviceParameters; }
        }

        #endregion

        public Configuration()
        {
            _usedHandles = new ObservableDictionary<InputValueHandle, InputBinding>();
            _analyzerParameters = new ObservableDictionary<string, ObservableDictionary<string, string>>();
            _deviceParameters = new ObservableDictionary<string, ObservableDictionary<string, string>>();
            _inputAnalyzers = new ObservableCollection<InputAnalyzer>();
        }

        ~Configuration()
        {
            Destroy();
        }

        public void Destroy()
        {
            foreach (InputAnalyzer a in _inputAnalyzers)
            {
                a.Destroy();
            }
            _inputAnalyzers.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public void ForceUpdate()
        {
            OnPropertyChanged("Bindings");
        }

        public void AddOrReplace(InputBinding binding)
        {
            RemoveBindingIfExist(binding.Handle);
            _usedHandles.Add(binding.Handle, binding);
            OnPropertyChanged("Bindings");
        }

        internal bool RemoveBindingIfExist(InputValueHandle handle)
        {
            // remove binding for the handle is some exists 
            if (handle != null &&_usedHandles.Keys.Contains<InputValueHandle>(handle))
            {
                InputBinding oldBinding = _usedHandles[handle];
                _usedHandles.Remove(handle);
                oldBinding.destroy();
                OnPropertyChanged("Bindings");
                return true;
            }
            return false;
        }

        internal InputBinding GetBinding(InputValueHandle handle)
        {
            if (_usedHandles.Keys.Contains<InputValueHandle>(handle))
            {
                return _usedHandles[handle];
            }
            return null;
        }

        internal InputBinding GetBinding(string gameBindingPoint)
        {
            InputValueHandle handle = Configuration.GetHandle(gameBindingPoint);
            if (handle != null)
            {
                return this.GetBinding(handle);
            }
            return null;
        }

        public void RemoveAllBindings()
        {
            foreach (InputValueHandle handle in _usedHandles.Keys)
            {
                InputBinding binding = _usedHandles[handle];
                binding.destroy();
            }
            _usedHandles.Clear();
            OnPropertyChanged("Bindings");
        }



        public delegate void InputValueHandle(int source, float value);

        #region Input registration and storage of handlers (mostly static members)

        protected static ConcurrentDictionary<string, InputValueHandle> _handle2BindingPoint = new ConcurrentDictionary<string, InputValueHandle>();
        protected static ConcurrentDictionary<InputValueHandle, string> _bindingPoint2Handle = new ConcurrentDictionary<InputValueHandle, string>();

        public static ConcurrentDictionary<string, InputValueHandle> RegisteredInputs
        {
            get
            {
                return _handle2BindingPoint;
            }
        }

        public static InputValueHandle GetHandle(string bindingPoint)
        {
            if (_handle2BindingPoint.Keys.Contains(bindingPoint))
            {
                return _handle2BindingPoint[bindingPoint];
            }
            return null;
        }

        public static String GetBindingPoint(InputValueHandle handle)
        {
            if (_bindingPoint2Handle.Keys.Contains(handle))
            {
                return _bindingPoint2Handle[handle];
            }
            return null;
        }

        public static void ClearBindingPoints() {
            _handle2BindingPoint.Clear();
            _bindingPoint2Handle.Clear();
        }

        public static void RegisterBindingPoint(string key, InputValueHandle handle)
        {
            // TODO remove handle (if exists) from registered bindings
            //if (_handle2BindingPoint.ContainsKey(key))
            //{
            //    string k = key;
            //    InputValueHandle h = _handle2BindingPoint[key];
                
            //}
                
            _handle2BindingPoint.TryAdd(key, handle);
            _bindingPoint2Handle.TryAdd(handle, key);
            
        }

        private static Configuration _currentConfiguration = null;

        public static Configuration CurrentConfigutration
        {
            get
            {
                if (_currentConfiguration == null)
                {
                    _currentConfiguration = CreateDefaultConfiguration();

                }
                return _currentConfiguration;
            }

            set
            {
                _currentConfiguration = value;
            }
        }

        private static Configuration CreateDefaultConfiguration()
        {
            // TODO Any specific initialisation of default configuration
            return new XmlSerializableConfiguration();
        }

        #endregion



    }
}
