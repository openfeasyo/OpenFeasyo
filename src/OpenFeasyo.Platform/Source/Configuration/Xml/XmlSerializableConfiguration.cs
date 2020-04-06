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
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Analysis;
using OpenFeasyo.Platform.Controls.Drivers;
using OpenFeasyo.Platform.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace OpenFeasyo.Platform.Configuration.Xml
{
    [XmlRoot("Configuration")]
    public class XmlSerializableConfiguration : Configuration, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Dictionary<string, IDevice> devices = PrepareDevicesByName();

            XPathDocument conf = new XPathDocument(reader);
            XPathNavigator nav = conf.CreateNavigator();

            // !!! IMPORTANT: The order of following method-calls matters
            ConfigureDevices(nav, devices);
            ConfigureBindings(nav, devices);

            // We've just read it so it must be current
            Configuration.CurrentConfigutration = this;
        }

        private void ConfigureDevices(XPathNavigator navigator, Dictionary<string, IDevice> devices)
        {
            XPathNodeIterator deviceIt = navigator.Select("/Configuration/devices/device[@name]");
            while (deviceIt.MoveNext())
            {
                // parsing of attributes
                string deviceName = deviceIt.Current.GetAttribute("name", "");
                ObservableDictionary<string, string> parametersGroup = ParseParams(deviceIt.Current);

                if (!devices.ContainsKey(deviceName))
                {
                    string strDevices = "";
                    foreach (string str in devices.Keys) strDevices += (str == "" ? "" : ",") + str;
                    UIThread.ShowMessage("", "Device " + deviceName + " was not found on this computer!\n " +
                        devices.Count + " devices installed. \n(" + strDevices + ")");
                    continue;
                }

                IDevice d = devices[deviceName];
                if (!d.IsLoaded)
                {
                    try
                    {
                        d.LoadDriver(parametersGroup);
                    }
                    catch (InitializationFailedException ex)
                    {
                        //   MessageBox.Show(ex.Message, "ConfigureDevices: Unable to load driver", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }
                    catch (Exception ex)
                    {
                        UIThread.ShowMessage("", ex.Message);
                        continue;
                    }
                }
                _deviceParameters.Add(deviceName, parametersGroup);

                if (Configuration.RegisteredInputs.Count == 0) continue;

                ObservableDictionary<string, ObservableDictionary<string, string>> parameters = ParseAnalyzersParams(deviceIt.Current);
                foreach (string analyzerFile in parameters.Keys)
                {
                    if (d.GamingInput is ISkeletonInput)
                    {
                        ISkeletonAnalyzer analyzer = InputAnalyzerManager.GetSkeletonAnalyzer(analyzerFile);
                        if (analyzer == null)
                        {
                            Console.Error.WriteLine("ERROR: Analyzer " + analyzerFile + " could not be loaded!!!");
                        }
                        else
                        {
                            _inputAnalyzers.Add(new InputAnalyzer(parameters[analyzerFile], d, analyzer));
                        }
                    }
                    else if (d.GamingInput is IAccelerometerInput)
                    {
                        _inputAnalyzers.Add(new InputAnalyzer(parameters[analyzerFile], d, InputAnalyzerManager.GetAccelerometerAnalyzer(analyzerFile)));
                    }
                    else if (d.GamingInput is IBalanceBoardInput)
                    {
                        _inputAnalyzers.Add(new InputAnalyzer(parameters[analyzerFile], d, InputAnalyzerManager.GetBalanceBoardAnalyzer(analyzerFile)));
                    }
                    else if (d.GamingInput is IEmgSensorInput)
                    {
                        _inputAnalyzers.Add(new InputAnalyzer(parameters[analyzerFile], d, InputAnalyzerManager.GetEmgSignalAnalyzer(analyzerFile)));
                    }
                }
            }
        }

        private ObservableDictionary<string, string> ParseParams(XPathNavigator navigator)
        {
            ObservableDictionary<string, string> parametersGroup = new ObservableDictionary<string, string>();
            XPathNodeIterator paramIt = navigator.Select("param[@name and @value]");
            while (paramIt.MoveNext())
            {
                string name = paramIt.Current.GetAttribute("name", "");
                string value = paramIt.Current.GetAttribute("value", "");
                parametersGroup.Add(name, value);
            }
            return parametersGroup;
        }

        private void ConfigureBindings(XPathNavigator navigator, Dictionary<string, IDevice> devices)
        {
            XPathNodeIterator bindingIt = navigator.Select(
                "/Configuration/bindings/binding[@device and @point and @sensitivity]");
            while (bindingIt.MoveNext())
            {
                string deviceName = bindingIt.Current.GetAttribute("device", "");

                if (!devices.ContainsKey(deviceName))
                {
                    UIThread.ShowMessage("", "Device " + deviceName + " was not found on this computer!");
                    continue;
                }

                IDevice device = devices[deviceName];
                if (!device.IsLoaded)
                {
                    try
                    {
                        device.LoadDriver(null);
                    }
                    catch (InitializationFailedException ex)
                    {
                        // MessageBox.Show(ex.Message, "ConfigureBindings: Unable to load driver", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }
                }

                IGamingInput gamingInput = device.GamingInput;
                InputBinding binding = null;

                string point = bindingIt.Current.GetAttribute("point", "");
                if (Configuration.GetHandle(point) == null) continue;

                if (device.GamingInput is ISkeletonInput)
                {
                    binding = ConfigureSkeleton(navigator, bindingIt, (ISkeletonInput)device.GamingInput);
                    ObservableDictionary<string, ObservableDictionary<string, string>> analyzersParams =
                                                        ParseAnalyzersParams(bindingIt.Current);
                    foreach (string analyzerFile in analyzersParams.Keys)
                    {
                        if (binding is SkeletonBinding)
                            (binding as SkeletonBinding).AddAnalyzer(
                                InputAnalyzerManager.GetSkeletonAnalyzer(analyzerFile), analyzersParams[analyzerFile]);
                        else if (binding is AbsoluteSkeletonBinding)
                            (binding as AbsoluteSkeletonBinding).AddAnalyzer(
                                InputAnalyzerManager.GetSkeletonAnalyzer(analyzerFile), analyzersParams[analyzerFile]);
                    }
                }
                else if (gamingInput is IAccelerometerInput)
                {
                    binding = ConfigureAccelerometer(navigator, bindingIt, (IAccelerometerInput)device.GamingInput);
                    ObservableDictionary<string, ObservableDictionary<string, string>> analyzersParams =
                                                        ParseAnalyzersParams(bindingIt.Current);
                    foreach (string analyzerFile in analyzersParams.Keys)
                    {
                        if (binding is AccelerometerBinding)
                            (binding as AccelerometerBinding).AddAnalyzer(
                                InputAnalyzerManager.GetAccelerometerAnalyzer(analyzerFile), analyzersParams[analyzerFile]);
                    }
                }
                else if (gamingInput is IBalanceBoardInput)
                {
                    binding = ConfigureBalanceBoard(navigator, bindingIt, (IBalanceBoardInput)device.GamingInput);
                    ObservableDictionary<string, ObservableDictionary<string, string>> analyzersParams =
                                                        ParseAnalyzersParams(bindingIt.Current);

                    //
                    //  TODO add balance board analyzers processing
                    //
                    //foreach (string analyzerFile in analyzersParams.Keys)
                    //{
                    //    if (binding is BalanceBoardBinding)
                    //        (binding as BalanceBoardBinding).AddAnalyzer(
                    //            AnalysisManager.GetBAccelerometerAnalyzer(analyzerFile), analyzersParams[analyzerFile]);
                    //}
                }
                else if (gamingInput is IEmgSensorInput)
                {
                    binding = ConfigureEmgSensor(navigator, bindingIt, (IEmgSensorInput)device.GamingInput);
                    //ObservableDictionary<string, ObservableDictionary<string, string>> analyzerParams = 
                    //                                    ParseAnalyzersParams(bindingIt.Current);
                }


                if (binding != null)
                {
                    this.AddOrReplace(binding);
                }
            }
        }

        private ObservableDictionary<string, ObservableDictionary<string, string>> ParseAnalyzersParams(XPathNavigator navigator)
        {
            ObservableDictionary<string, ObservableDictionary<string, string>> analyzersParams =
                new ObservableDictionary<string, ObservableDictionary<string, string>>();
            XPathNodeIterator analyzers = navigator.Select("analyzers/analyzer[@file]");

            while (analyzers.MoveNext())
            {
                string analyzer = analyzers.Current.GetAttribute("file", "");
                ObservableDictionary<string, string> parametersGroup = ParseParams(analyzers.Current);
                analyzersParams.Add(analyzer, parametersGroup);

            }

            return analyzersParams;
        }

        private InputBinding ConfigureAbsoluteSkeleton(XPathNavigator navigator, XPathNodeIterator bindingIt,
            AbsoluteSkeletonBinding.BindingAxis axis, string point, float zeroAngle, float sensitivity, ISkeletonInput input)
        {
            XPathNodeIterator sb1 = bindingIt.Current.Select("skeleton/trackedJoint");
            XPathNodeIterator sb2 = bindingIt.Current.Select("skeleton/baseJoint");

            if (sb1.Count > 0)
            {
                sb1.MoveNext();
                SkeletonMarkers trackedJoint = (SkeletonMarkers)Enum.Parse(typeof(SkeletonMarkers), sb1.Current.Value);
                if (sb2.Count > 0)
                {
                    sb2.MoveNext();
                    SkeletonMarkers? baseJoint = (SkeletonMarkers)Enum.Parse(typeof(SkeletonMarkers), sb2.Current.Value);
                    return new AbsoluteSkeletonBinding(input, Configuration.GetHandle(point), trackedJoint, baseJoint, axis, zeroAngle, sensitivity);
                }
                return new AbsoluteSkeletonBinding(input, Configuration.GetHandle(point), trackedJoint, null, axis, zeroAngle, sensitivity);
            }
            return null;
        }

        private InputBinding ConfigureSkeleton(
            XPathNavigator navigator, XPathNodeIterator bindingIt, ISkeletonInput input)
        {
            string point = bindingIt.Current.GetAttribute("point", "");
            float zeroAngle = float.Parse(bindingIt.Current.GetAttribute("zeroAngle", ""), CultureInfo.InvariantCulture);
            float sensitivity = float.Parse(bindingIt.Current.GetAttribute("sensitivity", ""), CultureInfo.InvariantCulture);

            XPathNodeIterator axis = bindingIt.Current.Select("skeleton/axis");
            if (axis.Count > 0)
            {
                axis.MoveNext();
                return ConfigureAbsoluteSkeleton(navigator, bindingIt,
                    (AbsoluteSkeletonBinding.BindingAxis)Enum.Parse(typeof(AbsoluteSkeletonBinding.BindingAxis), axis.Current.Value),
                    point, zeroAngle, sensitivity, input);
            }

            XPathNodeIterator sb1 = bindingIt.Current.Select("skeleton/firstBone");
            XPathNodeIterator sb2 = bindingIt.Current.Select("skeleton/secondBone");

            if (sb1.Count > 0)
            {
                sb1.MoveNext();
                BoneMarkers bone1 = (BoneMarkers)Enum.Parse(typeof(BoneMarkers), sb1.Current.Value);
                if (sb2.Count > 0)
                {
                    sb2.MoveNext();
                    BoneMarkers bone2 = (BoneMarkers)Enum.Parse(typeof(BoneMarkers), sb2.Current.Value);
                    return new SkeletonBinding(input, Configuration.GetHandle(point), bone1, bone2, zeroAngle, sensitivity);
                }
                return new SkeletonBinding(input, Configuration.GetHandle(point), bone1, zeroAngle, sensitivity);
            }
            return null;
        }

        private InputBinding ConfigureAccelerometer(
            XPathNavigator navigator, XPathNodeIterator bindingIt, IAccelerometerInput input)
        {
            string point = bindingIt.Current.GetAttribute("point", "");
            float zeroAngle = float.Parse(bindingIt.Current.GetAttribute("zeroAngle", ""), CultureInfo.InvariantCulture);
            float sensitivity = float.Parse(bindingIt.Current.GetAttribute("sensitivity", ""), CultureInfo.InvariantCulture);


            XPathNodeIterator acc = bindingIt.Current.Select("accelerometer");
            if (acc.Count > 0)
            {
                acc.MoveNext();
                string device = acc.Current.GetAttribute("device", "");
                string strAxis = acc.Current.GetAttribute("axis", "");
                AccelerometerBinding.BindingType axis = (AccelerometerBinding.BindingType)Enum.Parse(typeof(AccelerometerBinding.BindingType), strAxis);
                input.Start(device);
                return new AccelerometerBinding(input, Configuration.GetHandle(point), axis, zeroAngle, sensitivity);
            }
            return null;
        }

        private InputBinding ConfigureBalanceBoard(
            XPathNavigator navigator, XPathNodeIterator bindingIt, IBalanceBoardInput input)
        {
            string point = bindingIt.Current.GetAttribute("point", "");
            float zeroAngle = float.Parse(bindingIt.Current.GetAttribute("zeroAngle", ""), CultureInfo.InvariantCulture);
            float sensitivity = float.Parse(bindingIt.Current.GetAttribute("sensitivity", ""), CultureInfo.InvariantCulture);

            XPathNodeIterator acc = bindingIt.Current.Select("balanceBoard");
            if (acc.Count > 0)
            {
                acc.MoveNext();
                string device = acc.Current.GetAttribute("device", "");
                string strAxis = acc.Current.GetAttribute("axis", "");
                BalanceBoardBinding.MovementOrientation axis = (BalanceBoardBinding.MovementOrientation)Enum.Parse(typeof(BalanceBoardBinding.MovementOrientation), strAxis);
                return new BalanceBoardBinding(input, Configuration.GetHandle(point), axis, zeroAngle, sensitivity, sensitivity);
            }
            return null;
        }

        private InputBinding ConfigureEmgSensor(XPathNavigator navigator, XPathNodeIterator bindingIt, IEmgSensorInput input)
        {
            string point = bindingIt.Current.GetAttribute("point", "");
            //float zeroAngle = float.Parse(bindingIt.Current.GetAttribute("zeroAngle", ""), CultureInfo.InvariantCulture);
            float sensitivity = float.Parse(bindingIt.Current.GetAttribute("sensitivity", ""), CultureInfo.InvariantCulture);

            XPathNodeIterator acc = bindingIt.Current.Select("emgSensor");
            if (acc.Count > 0)
            {
                acc.MoveNext();
                string device = acc.Current.GetAttribute("device", "");
                int channel = Int32.Parse(acc.Current.GetAttribute("channel", ""));
                return new EmgBinding(input, Configuration.GetHandle(point), channel);
            }
            return null;
        }

        private void writeParams(XmlWriter writer, ObservableDictionary<string, string> parameters)
        {
            foreach (string key in parameters.Keys)
            {
                writer.WriteStartElement("param");
                writer.WriteAttributeString("name", key);
                writer.WriteAttributeString("value", parameters[key]);
                writer.WriteEndElement();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("devices");
            //foreach (string device in _deviceParameters.Keys) {
            //    if (_deviceParameters[device].Keys.Count > 0) 
            //    {
            //        writer.WriteStartElement("device");
            //        writer.WriteAttributeString("name",device);
            //        writeParams(writer,_deviceParameters[device]);
            //        SerializeDeviceAnalyzers(writer, device);
            //        writer.WriteEndElement();
            //    }

            //}
            foreach (string device in GetUsedDevices())
            {
                writer.WriteStartElement("device");
                writer.WriteAttributeString("name", device);
                if (_deviceParameters.ContainsKey(device) && _deviceParameters[device].Keys.Count > 0)
                {
                    writeParams(writer, _deviceParameters[device]);
                }
                SerializeDeviceAnalyzers(writer, device);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteStartElement("bindings");

            foreach (InputValueHandle handle in _usedHandles.Keys)
            {
                writer.WriteStartElement("binding");
                InputBinding binding = _usedHandles[handle];
                writer.WriteAttributeString("point", GetBindingPoint(handle));
                writer.WriteAttributeString("zeroAngle", binding.ZeroAngle.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("sensitivity", binding.Sensitivity.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("device", binding.Input.Device.Name);

                if (binding is AccelerometerBinding)
                {
                    SerializeAccelerometer(writer, binding as AccelerometerBinding);
                }
                else if (binding is SkeletonBinding)
                {
                    SerializeSkeleton(writer, binding as SkeletonBinding);
                }
                else if (binding is AbsoluteSkeletonBinding)
                {
                    SerializeAbsoluteSkeleton(writer, binding as AbsoluteSkeletonBinding);
                }
                else if (binding is BalanceBoardBinding)
                {
                    SerializeBalanceBoard(writer, binding as BalanceBoardBinding);
                }
                else if (binding is EmgBinding)
                {
                    SerializeEmgSensor(writer, binding as EmgBinding);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private HashSet<string> GetUsedDevices()
        {
            HashSet<string> ret = new HashSet<string>();
            foreach (InputValueHandle handle in _usedHandles.Keys)
            {
                InputBinding binding = _usedHandles[handle];
                if (!ret.Contains(binding.Input.Device.Name))
                {
                    ret.Add(binding.Input.Device.Name);
                }
            }
            return ret;
        }

        private void SerializeDeviceAnalyzers(XmlWriter writer, string deviceName)
        {
            bool sectionStarted = false;


            if (_inputAnalyzers.Count == 0)
            {
                // THIS IS TEMPORARY SOLUTION TO SAVE C3D ANALYZER FOR DEVICE AS DEFAULT
                writer.WriteStartElement("analyzers");
                writer.WriteStartElement("analyzer");
                writer.WriteAttributeString("file", "C3DSerializer.dll");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            else
            {
                foreach (InputAnalyzer a in _inputAnalyzers)
                {
                    if (a.Device.Name == deviceName)
                    {
                        if (!sectionStarted)
                        {
                            writer.WriteStartElement("analyzers");
                            sectionStarted = true;
                        }
                        writer.WriteStartElement("analyzer");
                        if (a.Analyzer is ISkeletonAnalyzer)
                        {
                            writer.WriteAttributeString("file", InputAnalyzerManager.GetAnalyzerModuleName(a.Analyzer as ISkeletonAnalyzer));
                        }
                        else if (a.Analyzer is IAccelerometerAnalyzer)
                        {
                            writer.WriteAttributeString("file", InputAnalyzerManager.GetAnalyzerModuleName(a.Analyzer as IAccelerometerAnalyzer));
                        }
                        else if (a.Analyzer is IBalanceBoardAnalyzer)
                        {
                            writer.WriteAttributeString("file", InputAnalyzerManager.GetAnalyzerModuleName(a.Analyzer as IBalanceBoardAnalyzer));
                        }
                        else if (a.Analyzer is IEmgSignalAnalyzer)
                        {
                            writer.WriteAttributeString("file", InputAnalyzerManager.GetAnalyzerModuleName(a.Analyzer as IEmgSignalAnalyzer));
                        }

                        if (a.Parameters.Keys.Count > 0)
                        {
                            writeParams(writer, a.Parameters);
                        }
                        writer.WriteEndElement();
                    }
                }


                if (sectionStarted)
                {
                    writer.WriteEndElement();
                }
            }
        }

        private void SerializeAnalyzers(XmlWriter writer, ObservableDictionary<IAnalyzer, ObservableDictionary<string, string>> analyzers)
        {
            writer.WriteStartElement("analyzers");
            foreach (IAnalyzer analyzer in analyzers.Keys)
            {
                writer.WriteStartElement("analyzer");

                if (analyzer is IAccelerometerAnalyzer)
                {
                    writer.WriteAttributeString("file", InputAnalyzerManager.GetAnalyzerModuleName(analyzer as IAccelerometerAnalyzer));
                }
                else if (analyzer is IBalanceBoardAnalyzer)
                {
                    writer.WriteAttributeString("file", InputAnalyzerManager.GetAnalyzerModuleName(analyzer as IBalanceBoardAnalyzer));
                }
                else if (analyzer is IEmgSignalAnalyzer)
                {
                    writer.WriteAttributeString("file", InputAnalyzerManager.GetAnalyzerModuleName(analyzer as IEmgSignalAnalyzer));
                }
                else
                {
                    writer.WriteAttributeString("file", InputAnalyzerManager.GetAnalyzerModuleName(analyzer as ISkeletonAnalyzer));
                }

                writeParams(writer, analyzers[analyzer]);

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void SerializeSkeleton(XmlWriter writer, SkeletonBinding binding)
        {
            SerializeAnalyzers(writer, binding.Analyzers);

            XmlSerializer boneMarkerSerializer =
                new XmlSerializer(typeof(BoneMarkers));

            XmlSerializer bindingTypeSerializer =
                new XmlSerializer(typeof(SkeletonBinding.BindingType));

            writer.WriteStartElement("skeleton");

            writer.WriteStartElement("type");
            bindingTypeSerializer.Serialize(writer, binding.Type);
            writer.WriteEndElement();

            writer.WriteStartElement("firstBone");
            boneMarkerSerializer.Serialize(writer, binding.FirstBone);
            writer.WriteEndElement();

            if (binding.Type == SkeletonBinding.BindingType.TwoBonesAngle)
            {
                writer.WriteStartElement("secondBone");
                boneMarkerSerializer.Serialize(writer, binding.SecondBone);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void SerializeAbsoluteSkeleton(XmlWriter writer, AbsoluteSkeletonBinding binding)
        {
            SerializeAnalyzers(writer, binding.Analyzers);

            XmlSerializer jointMarkerSerializer =
                new XmlSerializer(typeof(SkeletonMarkers));

            XmlSerializer axisTypeSerializer =
                new XmlSerializer(typeof(AbsoluteSkeletonBinding.BindingAxis));

            writer.WriteStartElement("skeleton");

            writer.WriteStartElement("axis");
            axisTypeSerializer.Serialize(writer, binding.Axis);
            writer.WriteEndElement();

            writer.WriteStartElement("trackedJoint");
            jointMarkerSerializer.Serialize(writer, binding.TrackedJoint);
            writer.WriteEndElement();

            if (binding.BaseJoint.HasValue)
            {
                writer.WriteStartElement("baseJoint");
                jointMarkerSerializer.Serialize(writer, binding.BaseJoint.Value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void SerializeAccelerometer(XmlWriter writer, AccelerometerBinding binding)
        {
            writer.WriteStartElement("accelerometer");
            writer.WriteAttributeString("device", (binding.Input as IAccelerometerInput).AccelerometerDevice);
            writer.WriteAttributeString("axis", binding.Type.ToString());
            writer.WriteEndElement();
        }

        private void SerializeBalanceBoard(XmlWriter writer, BalanceBoardBinding binding)
        {
            writer.WriteStartElement("balanceBoard");
            writer.WriteAttributeString("device", (binding.Input as IBalanceBoardInput).Device.Name);
            writer.WriteAttributeString("axis", binding.Orientation.ToString());
            writer.WriteEndElement();
        }

        private void SerializeEmgSensor(XmlWriter writer, EmgBinding binding)
        {
            writer.WriteStartElement("emgSensor");
            writer.WriteAttributeString("device", (binding.Input as IEmgSensorInput).Device.Name);
            writer.WriteAttributeString("channel", binding.Channel.ToString());
            writer.WriteEndElement();
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
    }
}
