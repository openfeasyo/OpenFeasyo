﻿/*
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
using OpenFeasyo.Platform.Network;
using OpenFeasyo.Platform.Network.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Drivers
{
    public static class DeviceHelper
    {
        public static string Serialize(IDevice device) { 
            StringBuilder builder = new StringBuilder(device.Name);
            builder.AppendFormat(";{0};{1};",device.Vendor,(int)device.DeviceType);
            return builder.ToString();
        }

        public static string [] Serialize(IEnumerable<IDevice> devices)
        {
            string[] encodedDevices = new string[devices.Count<IDevice>()];
            int i = 0;
            foreach (IDevice d in devices)
            {
                encodedDevices[i++] = Serialize(d);
            }
            return encodedDevices;
        }

        public static IDevice Deserialize(IObject obj, string deviceString) {
            string[] tokens = deviceString.Split(';');
            DeviceType type = DeviceType.Unknown;
            string vendor = "Unknown";
            if (tokens.Length > 2) { 
                type = (DeviceType)Int32.Parse(tokens[2]);
            }
            if (tokens.Length > 1) {
                vendor = tokens[1];
            }
            IDevice d = new DeviceProxy(obj, tokens[0],"",vendor,type);
            return d;
        }

        public static IDevice [] Deserialize(IObject obj, string [] deviceString)
        {
            IDevice[] devices = new IDevice[deviceString.Length];
            int i = 0;
            foreach (string s in deviceString) 
            {
                devices[i++] = Deserialize(obj, s);
            }
            return devices;
        }

    }
}
