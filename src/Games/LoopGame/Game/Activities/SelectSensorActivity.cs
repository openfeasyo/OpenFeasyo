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
using OpenFeasyo.GameTools.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using OpenFeasyo.Platform.Controls.Drivers;
using OpenFeasyo.Platform.Controls;
using Microsoft.Xna.Framework.Input;

namespace LoopLib.Activities
{
    public class SelectSensorActivity : OpenFeasyo.GameTools.UI.Activity
    {
        private TextButton _scanButton;
        private TextButton _jumpingButton = null;
        private TextButton _shootingButton = null;
        private TextButton _nextButton;

        private Label _scanningLabel;
        private Label _connectingLabel;
        private Label _jumpLabel;
        private Label _shootLabel;
        private Label _jumpQuestionLabel;
        private Label _shootQuestionLabel;


        private IDiscoverable _discoveryProvider = null;
        private IEmgSensorInput _emgInput;
        private IDevice dev;

        //private Dictionary<string, SensorTrignoBt> _sensors = new Dictionary<string, SensorTrignoBt>();
        private ICollection<string> _sensors = new List<string>();
        private List<TextButton> _buttons = new List<TextButton>();

        private bool _emgConfiguredAndReady = false;

        // FPS measuring related variables
        private DateTime _lastTime = DateTime.Now; // marks the beginning the measurement began
        private int _framesReceived = 0; // an increasing count
        private int _fps = 0;


        public SelectSensorActivity(UIEngine engine) : base(engine)
        {

            float cell = engine.Screen.ScreenHeight / 8;
            int superScriptSize = new int[] { 12, 24, 36, 48, 64 }[engine.Screen.FontSize-1];
            Image backgroundImage = new Image(_engine.Content.LoadTexture("Textures/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            Dictionary<string, IDevice> devices = PrepareDevicesByName();
            if (!devices.ContainsKey("TrignoEmg")) {
                throw new ApplicationException("TrignoEmg not loaded");
            }
            dev = devices["TrignoEmg"];
            if (!dev.IsLoaded) {
                dev.LoadDriver(new Dictionary<string, string>());
            }
            
            _scanButton = new TextButton("Scan for sensors", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
                _scanButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                    ScanForSensors();
            };
            _scanButton.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 1) - _scanButton.Size / 2 ;
            

            _scanningLabel = new Label("Scanning ...", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            _scanningLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 1) - _scanningLabel.Size / 2 ;
            _scanningLabel.Visible = false;

            

            TextButton s = new TextButton("N / A", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            s.Clicked += S_Clicked;
            s.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 3) - s.Size / 2;
            s.Hidden = true;
            s.CursorEntered += (object sender, EventArgs e) => {
                engine.MusicPlayer.PlayEffect("hover");
            };
            _buttons.Add(s);
            Components.Add(s);

            s = new TextButton("N / A", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            s.Clicked += S_Clicked;
            s.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 4) - s.Size / 2;
            s.Hidden = true;
            _buttons.Add(s);
            Components.Add(s);

            s = new TextButton("N / A", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            s.Clicked += S_Clicked;
            s.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 5) - s.Size / 2;
            s.Hidden = true;
            _buttons.Add(s);
            Components.Add(s);

            s = new TextButton("N / A", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            s.Clicked += S_Clicked;
            s.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 6) - s.Size / 2;
            s.Hidden = true;
            _buttons.Add(s);
            Components.Add(s);

            _nextButton = new TextButton("Connect", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT+ LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            _nextButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                SelectSensor();
            };
            _nextButton.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 7) - _nextButton.Size / 2 + new Vector2(cell*2, 0);
            _nextButton.Visible = false;
            _nextButton.CursorEntered += (object sender, EventArgs e) => {
                engine.MusicPlayer.PlayEffect("hover");
            };

            _connectingLabel = new Label("Connecting to ....", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            _connectingLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 7) - _connectingLabel.Size/2 + new Vector2(cell*2, 0);
            _connectingLabel.Visible = false;


            _jumpLabel = new Label("Jump", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            _jumpLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 3) - _jumpLabel.Size / 2 + new Vector2(s.Size.X*1.5f, 0);
            _jumpLabel.Visible = false;

            _shootLabel = new Label("Shoot", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            _shootLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 4) - _shootLabel.Size / 2 + new Vector2(s.Size.X * 1.5f, 0);
            _shootLabel.Visible = false;

            _jumpQuestionLabel = new Label("Select sensor for Jumping", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            _jumpQuestionLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 2) - _jumpQuestionLabel.Size / 2;
            _jumpQuestionLabel.Visible = true;

            _shootQuestionLabel = new Label("Select sensor for Shooting", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            _shootQuestionLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 2) - _shootQuestionLabel.Size / 2;
            _shootQuestionLabel.Visible = false;

            Components.Add(_nextButton);
            Components.Add(_connectingLabel);
            Components.Add(_scanningLabel);
            Components.Add(_scanButton);

            Components.Add(_jumpLabel);
            Components.Add(_shootLabel);
            Components.Add(_jumpQuestionLabel);
            Components.Add(_shootQuestionLabel);
            
            UpdateButtons();
        }

        private void ScanForSensors()
        {
            _discoveryProvider.ScanAsync();
            _scanButton.Visible = false;
            _scanningLabel.Visible = true;
        }

        private void S_Clicked(object sender, TextButton.ClickedEventArgs e)
        {
            if (_jumpingButton == null)
            {
                _jumpingButton = (TextButton)sender;
                _jumpLabel.Position = new Vector2(_jumpLabel.Position.X, _jumpingButton.Position.Y + (_jumpingButton.Size.Y - _jumpLabel.Size.Y)/2);
            }
            else if (_jumpingButton == sender)
            {
                _jumpingButton = null;
            }
            else if (_shootingButton == null)
            {
                _shootingButton = (TextButton)sender;
                _shootLabel.Position = new Vector2(_shootLabel.Position.X, _shootingButton.Position.Y + (_shootingButton.Size.Y - _shootLabel.Size.Y) / 2);
            }
            else if (_shootingButton == sender)
            {
                _shootingButton = null;
            }
            UpdateButtons();
        }

        private void UpdateButtons() {
            _jumpLabel.Visible = _jumpingButton != null;
            _shootLabel.Visible = _shootingButton != null;
            if (_jumpingButton == null)
            {
                _jumpQuestionLabel.Visible = _sensors.Count >= 2;
                _shootQuestionLabel.Visible = false;
                _nextButton.Visible = false;
            } else if (_shootingButton == null) {

                _jumpQuestionLabel.Visible = false;
                _shootQuestionLabel.Visible = true;
                _nextButton.Visible = false;
            } else {
                _jumpQuestionLabel.Visible = false;
                _shootQuestionLabel.Visible = false;
                _nextButton.Visible = true;
            }
        }

        private void _discoveryProvider_ScanFinished(object sender, ScanResultsEventArgs e)
        {
            _scanButton.Visible = true;
            _scanningLabel.Visible = false;
            _sensors = e.Devices;
            UpdateSensors(e.Devices);
            UpdateButtons();
        }

        private static Dictionary<string, IDevice> PrepareDevicesByName()
        {
            Dictionary<string, IDevice> devicesByName = new Dictionary<string, IDevice>();
            foreach (IDevice device in InputDeviceManager.Drivers)
            {
                devicesByName.Add(device.Name, device);
            }
            return devicesByName;
        }

        public override void OnCreate()
        {
            if (dev.GamingInput is IDiscoverable)
            {
                _discoveryProvider = (IDiscoverable)dev.GamingInput;
                _discoveryProvider.ScanFinished += _discoveryProvider_ScanFinished;
            }
            if (dev.GamingInput is IEmgSensorInput)
            {
                _emgInput = dev.GamingInput as IEmgSensorInput;
                _emgInput.MuscleActivationChanged += _emgInput_MuscleActivationChanged;
            }

            _emgConfiguredAndReady = false;
            base.OnCreate();
            if (dev != null && !dev.IsLoaded) {
                dev.LoadDriver(new Dictionary<string, string>());
                if (_discoveryProvider != null) {
                    _discoveryProvider.ScanFinished += (object sender, ScanResultsEventArgs e) 
                        => {
                            _scanButton.Visible = true;
                            _scanningLabel.Visible = false;
                            UpdateSensors(e.Devices);
                        };
                    _discoveryProvider.ConnectionEstablished += _discoveryProvider_ConnectionEstablished;
                    _discoveryProvider.ConnectionFailed += _discoveryProvider_ConnectionFailed;
                }
            }

            
            ScanForSensors();
        }

        public override void OnDestroy()
        {
            if (_discoveryProvider != null)
            {
                _discoveryProvider.ScanFinished -= _discoveryProvider_ScanFinished;
            }
            if (_emgInput != null)
            {
                _emgInput.MuscleActivationChanged += _emgInput_MuscleActivationChanged;
            }
            base.OnDestroy();
        }

        private void _discoveryProvider_ConnectionFailed(object sender, ConnectionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void _discoveryProvider_ConnectionEstablished(object sender, ConnectionEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _engine.StartActivity(null);
            }

        }


        private void SelectSensor() {
            _discoveryProvider.ConnectAsync(_jumpingButton.Text + ";" +_shootingButton.Text);
            _scanButton.Visible = false;
            //_connectingLabel.Visible = true;
            
            // TODO check the connection
            StartActivity(new StartCalibrationActivity(_engine, dev.GamingInput as IEmgSensorInput));
        }

        private void UpdateSensors(ICollection<string> sensors) {
            IEnumerator<TextButton> it =  _buttons.GetEnumerator();
            foreach (string name in sensors) {
                if (!it.MoveNext())
                {
                    break;
                }
                it.Current.Text = name;
                it.Current.Visible = true;
            }
            while (it.MoveNext())
            {
                it.Current.Visible = false;
                it.Current.Text = "N/A";
            }

        }


        
        private void _emgInput_MuscleActivationChanged(object sender, MuscleActivationChangedEventArgs e)
        {
            _framesReceived++;
            
            if ((DateTime.Now - _lastTime).TotalSeconds >= 1)
            {
                // one second has elapsed 

                _fps = _framesReceived;
                _framesReceived = 0;
                _lastTime = DateTime.Now;
                Console.WriteLine("FPS : " + _fps);
            }
            //Console.WriteLine("Raw signal: " + e.EMGSensor[0].RawSample);
            //          _emgSignalViewer.AddValue(e.EMGSensor[0]);
        }


//        #region Delsys related code
//        Pipeline BTPipeline;
//        ITransformManager TransformManager;
//        /// <summary>
//        /// If there are no device filters, the central will connect to every Avanti sensor
//        /// it detects.
//        /// </summary>
//        string[] DeviceFilters = new string[]
//        {
//        };

//        IDelsysDevice DeviceSource = null;


//        public void InitializeDataSource()
//        {
//            // Load your key & license either through reflection as shown in the User Guide, or by hardcoding it to these strings.

//            var assembly = Assembly.GetExecutingAssembly();
//            var resources = assembly.GetManifestResourceNames();
//            string key;
//            using (Stream stream = assembly.GetManifestResourceStream("GhostlyLib.PublicKey.lic"))
//            {
//                StreamReader sr = new StreamReader(stream);
//                key = sr.ReadLine();
//            }
//            string license;
//            using (Stream stream = assembly.GetManifestResourceStream("GhostlyLib.vrije.lic"))
//            {
//                StreamReader sr = new StreamReader(stream);
//                license = sr.ReadToEnd();
//            }


//            var deviceSourceCreator =
//#if ANDROID
//                new DelsysAPI.Android.DeviceSourcePortable(key, license);
//#else
//                new DelsysAPI.NET.DeviceSourcePortable(key, license);
//#endif
//            deviceSourceCreator.SetDebugOutputStream(Console.WriteLine);
//            DeviceSource = deviceSourceCreator.GetDataSource(SourceType.TRIGNO_BT);
//            DeviceSource.Key = key;
//            DeviceSource.License = license;
//            LoadDataSource(DeviceSource);
//        }

//        public void LoadDataSource(IDelsysDevice ds)
//        {
//            PipelineController.Instance.AddPipeline(ds);

//            BTPipeline = PipelineController.Instance.PipelineIds[0];
//            TransformManager = PipelineController.Instance.PipelineIds[0].TransformManager;

//            // Device Filters allow you to specify which sensors to connect to
//            foreach (var filter in DeviceFilters)
//            {
//                BTPipeline.TrignoBtManager.AddDeviceIDFilter(filter);
//            }

//            BTPipeline.CollectionStarted += CollectionStarted;
//            BTPipeline.CollectionDataReady += CollectionDataReady;
//            BTPipeline.CollectionComplete += CollectionComplete;

//            BTPipeline.TrignoBtManager.ComponentScanComplete += ComponentScanComplete;
//        }


//        public void ComponentAdded(object sender, ComponentAddedEventArgs e)
//        {
//            Console.WriteLine("ComponentAdded");
//        }

//        public void ComponentLost(object sender, ComponentLostEventArgs e)
//        {
//            Console.WriteLine("ComponentLost");
//        }

//        public void ComponentRemoved(object sender, ComponentRemovedEventArgs e)
//        {
//            Console.WriteLine("ComponentRemoved");
//        }

//        private void ComponentScanComplete(object sender, DelsysAPI.Events.ComponentScanCompletedEventArgs e)
//        {
//            //Application.Current.Dispatcher.BeginInvoke(new Action(() => {
//            Console.WriteLine("ComponentScanComplete: " + e.ComponentDictionary.Count.ToString());
//            //    tbox_SensorsConnected.Text = e.ComponentDictionary.Count.ToString();
//            _sensors.Clear();
//            for (int i = 0; i < BTPipeline.TrignoBtManager.Components.Count; i++)
//            {
//                //        tbox_SensorsConnectedGUIDs.Text += BTPipeline.TrignoBtManager.Components[i].Properties.SerialNumber.ToString() + (i == BTPipeline.TrignoBtManager.Components.Count - 1 ? "" : ", ");
//                Console.WriteLine(" - ComponentScanComplete: " + BTPipeline.TrignoBtManager.Components[i].Properties.SerialNumber.ToString() + (i == BTPipeline.TrignoBtManager.Components.Count - 1 ? "" : ", "));
//                Console.WriteLine(" - ComponentScanComplete: " + "Added a type {0} sensor . . . ", BTPipeline.TrignoBtManager.Components[i].Properties.SensorType);
//              //  _sensors.Add(BTPipeline.TrignoBtManager.Components[i].Properties.SerialNumber.ToString(), BTPipeline.TrignoBtManager.Components[i]);
//            }
//            _scanButton.Visible = true;
//            _scanningLabel.Visible = false;
//            //UpdateSensors(_sensors.Keys);
//            //}));

//            //btn_Start.IsEnabled = BTPipeline.TrignoBtManager.Components.Count > 0;
//            //btn_Scan.IsEnabled = true;
//            //btn_SelectSensors.IsEnabled = true;
//        }

//        public void CollectionDataReady(object sender, ComponentDataReadyEventArgs e)
//        {
//            //int lostPackets = 0;
//            //int dataPoints = 0;

//            //// Check each data point for if it was lost or not, and add it to the sum totals.
//            //for (int j = 0; j < e.Data.Count(); j++)
//            //{
//            //    var channelData = e.Data[j];
//            //    Data[j].AddRange(channelData.Data);
//            //    dataPoints += channelData.Data.Count;
//            //    for (int i = 0; i < channelData.Data.Count; i++)
//            //    {
//            //        if (e.Data[0].IsLostData[i])
//            //        {
//            //            lostPackets++;
//            //        }
//            //    }
//            //}
//            //TotalLostPackets += lostPackets;
//            //TotalDataPoints += dataPoints;

//            //// No need to await this; it may affect our total throughput.
//            //Application.Current.Dispatcher.BeginInvoke(
//            //new Action(() =>
//            //{
//            //    tbox_DroppedFrameCounter.Text = TotalLostPackets.ToString() + "/" + TotalDataPoints.ToString();
//            //}
//            //));
//        }

//        private void CollectionStarted(object sender, DelsysAPI.Events.CollectionStartedEvent e)
//        {
//            var comps = PipelineController.Instance.PipelineIds[0].TrignoBtManager.Components;
//            //txt_SensorsStreaming.Text = comps.Count.ToString();

//            //// Refresh the counters for display.
//            //TotalDataPoints = 0;
//            //TotalLostPackets = 0;

//            //// Recreate the list of data channels for recording
//            //int totalChannels = 0;
//            //for (int i = 0; i < comps.Count; i++)
//            //{
//            //    for (int j = 0; j < comps[i].BtChannels.Count; j++)
//            //    {
//            //        if (Data.Count <= totalChannels)
//            //        {
//            //            Data.Add(new List<double>());
//            //        }
//            //        else
//            //        {
//            //            Data[totalChannels] = new List<double>();
//            //        }
//            //        totalChannels++;
//            //    }
//            //}
//            //Task.Factory.StartNew(() => {
//            //    Stopwatch batteryUpdateTimer = new Stopwatch();
//            //    batteryUpdateTimer.Start();
//            //    while (BTPipeline.CurrentState == Pipeline.ProcessState.Running)
//            //    {
//            //        if (batteryUpdateTimer.ElapsedMilliseconds >= 500)
//            //        {
//            //            foreach (var comp in BTPipeline.TrignoBtManager.Components)
//            //            {
//            //                if (comp == null)
//            //                    continue;
//            //                Console.WriteLine("Sensor {0}: {1}% Charge", comp.Properties.SerialNumber, BTPipeline.TrignoBtManager.QueryBatteryComponentAsync(comp).Result);
//            //            }
//            //            batteryUpdateTimer.Restart();
//            //        }
//            //    }
//            //});
//        }

//        private void CollectionComplete(object sender, DelsysAPI.Events.CollectionCompleteEvent e)
//        {
//            //for (int i = 0; i < Data.Count; i++)
//            //{
//            //    using (StreamWriter channelOutputFile = new StreamWriter("./channel" + i + "_data.csv"))
//            //    {
//            //        foreach (var pt in Data[i])
//            //        {
//            //            channelOutputFile.WriteLine(pt.ToString());
//            //        }
//            //    }
//            //}
//            //BTPipeline.DisarmPipeline().Wait();
//            //btn_Start.IsEnabled = true;
//        }


//        private bool CallbacksAdded = false;
//        private bool ConfigurePipeline()
//        {
//            if (CallbacksAdded)
//            {
//                BTPipeline.TrignoBtManager.ComponentAdded -= ComponentAdded;
//                BTPipeline.TrignoBtManager.ComponentLost -= ComponentLost;
//                BTPipeline.TrignoBtManager.ComponentRemoved -= ComponentRemoved;
//            }
//            BTPipeline.TrignoBtManager.ComponentAdded += ComponentAdded;
//            BTPipeline.TrignoBtManager.ComponentLost += ComponentLost;
//            BTPipeline.TrignoBtManager.ComponentRemoved += ComponentRemoved;
//            CallbacksAdded = true;

//            PipelineController.Instance.PipelineIds[0].TrignoBtManager.Configuration = new TrignoBTConfig() { EOS = EmgOrSimulate.EMG };

//            var inputConfiguration = new BTDsConfig();
//            inputConfiguration.NumberOfSensors = BTPipeline.TrignoBtManager.Components.Count;
//            foreach (var somecomp in BTPipeline.TrignoBtManager.Components)
//            {
//                if (somecomp.State != SelectionState.Allocated) continue;

//                string selectedMode = "EMG+IMU,ACC:+/-2g,GYRO:+/-500dps";
//                // Synchronize to the UI thread and check if the mode textbox value exists in the
//                // available sample modes for the sensor.
//                //Application.Current.Dispatcher.BeginInvoke(new Action(() => {
//                //    if (somecomp.SensorConfiguration.SampleModes.Contains(tbox_SetMode.Text))
//                //    {
//                //        selectedMode = tbox_SetMode.Text;
//                //    }
//                //}));
//                somecomp.SensorConfiguration.SelectSampleMode(selectedMode);

//                if (somecomp.SensorConfiguration == null)
//                {
//                    return false;
//                }
//            }

//            PipelineController.Instance.PipelineIds[0].ApplyInputConfigurations(inputConfiguration);
//            var transformTopology = GenerateTransforms();
//            PipelineController.Instance.PipelineIds[0].ApplyOutputConfigurations(transformTopology);
//            PipelineController.Instance.PipelineIds[0].RunTime = Double.MaxValue;
//            return true;
//        }

//        public OutputConfig GenerateTransforms()
//        {
//            // Clear the previous transforms should they exist.
//            TransformManager.TransformList.Clear();

//            int channelNumber = 0;
//            // Obtain the number of channels based on our sensors and their mode.
//            for (int i = 0; i < BTPipeline.TrignoBtManager.Components.Count; i++)
//            {
//                if (BTPipeline.TrignoBtManager.Components[i].State == SelectionState.Allocated)
//                {
//                    var tmp = BTPipeline.TrignoBtManager.Components[i];

//                    BTCompConfig someconfig = tmp.SensorConfiguration as BTCompConfig;
//                    if (someconfig.IsComponentAvailable())
//                    {
//                        channelNumber += BTPipeline.TrignoBtManager.Components[i].BtChannels.Count;
//                    }

//                }
//            }

//            // Create the raw data transform, with an input and output channel for every
//            // channel that exists in our setup. This transform applies the scaling to the raw
//            // data from the sensor.
//            var rawDataTransform = new TransformRawData(channelNumber, channelNumber);
//            PipelineController.Instance.PipelineIds[0].TransformManager.AddTransform(rawDataTransform);

//            // The output configuration for the API to use.
//            var outconfig = new OutputConfig();
//            outconfig.NumChannels = channelNumber;

//            int channelIndex = 0;
//            for (int i = 0; i < BTPipeline.TrignoBtManager.Components.Count; i++)
//            {
//                if (BTPipeline.TrignoBtManager.Components[i].State == SelectionState.Allocated)
//                {
//                    BTCompConfig someconfig = BTPipeline.TrignoBtManager.Components[i].SensorConfiguration as BTCompConfig;
//                    if (someconfig.IsComponentAvailable())
//                    {
//                        // For every channel in every sensor, we gather its sampling information (rate, interval, units) and create a
//                        // channel transform (an abstract channel used by transforms) from it. We then add the actual component's channel
//                        // as an input channel, and the channel transform as an output. 
//                        // Finally, we map the channel counter and the output channel. This mapping is what determines the channel order in
//                        // the CollectionDataReady callback function.
//                        for (int k = 0; k < BTPipeline.TrignoBtManager.Components[i].BtChannels.Count; k++)
//                        {
//                            var chin = BTPipeline.TrignoBtManager.Components[i].BtChannels[k];
//                            var chout = new ChannelTransform(chin.FrameInterval, chin.SamplesPerFrame, BTPipeline.TrignoBtManager.Components[i].BtChannels[k].Unit);
//                            TransformManager.AddInputChannel(rawDataTransform, chin);
//                            TransformManager.AddOutputChannel(rawDataTransform, chout);
//                            Guid tmpKey = outconfig.MapOutputChannel(channelIndex, chout);
//                            channelIndex++;
//                        }
//                    }
//                }
//            }
//            return outconfig;
//        }
//
//
    }
}