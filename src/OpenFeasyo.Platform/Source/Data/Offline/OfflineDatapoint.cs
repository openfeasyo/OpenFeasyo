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

namespace OpenFeasyo.Platform.Data.Offline
{
    public class OfflineDatapoint : Datapoint
    {
        private Dictionary<Type, object> _offlineDefinitions;
        public OfflineDatapoint() {
            _offlineDefinitions = new Dictionary<Type, object>();

            // Configured games
            ConfiguredGame cg = new ConfiguredGame();
            cg.Name = "HitTheBoxes";
            cg.Parameters = "<?xml version=\"1.0\" encoding=\"utf - 8\"?><Configuration><devices><device name=\"Kinect\"><param name=\"ShowPreview\" value=\"true\" /><analyzers><analyzer file=\"C3DSerializer.dll\" /></analyzers></device></devices><bindings><binding point=\"Horizontal\" zeroAngle=\"90.58064\" sensitivity=\"0.7419356\" device=\"Kinect\"><skeleton><type><BindingType>SingleBoneAngle</BindingType></type><firstBone><BoneMarkers>Spine</BoneMarkers></firstBone></skeleton></binding></bindings></Configuration>";
            _offlineDefinitions.Add(typeof(ConfiguredGame), new ConfiguredGame[] { cg });

            _offlineDefinitions.Add(typeof(ExtendedPatient), new ExtendedPatient[] { });
            _offlineDefinitions.Add(typeof(MaxScore), new MaxScore[] { });
        }

        public void Insert<T>(T obj, bool forcePrimaryKey = true)
        {
            // Do nothing since we are offline
        }

        public void Remove<T>(T obj)
        {
            // Do nothing since we are offline
        }

        public T[] SelectAll<T>()
        {
            if (_offlineDefinitions.ContainsKey(typeof(T)))
            {
                return (T[])_offlineDefinitions[typeof(T)];
            }
            else {
                throw new NotImplementedException("Add definitions for type: " + typeof(T));
            }
        }

        public void Update<T>(T obj)
        {
            // Do nothing since we are offline
        }
    }
}
