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
using OpenFeasyo.Platform.Controls;
using Microsoft.Xna.Framework;

namespace OpenFeasyo.Platform.Network.Controls
{
    public class SkeletonProxy : ISkeleton
    {
        private int[] _points;
        internal SkeletonProxy(int[] points) {
            _points = points;
        }
        public Vector3 GetPositionOf(SkeletonMarkers marker)
        {
            int i = (int)marker;
            return new Vector3(_points[i * 3], _points[i * 3 + 1], _points[i * 3 + 2]);
        }

        // So far we don't need this in proxy class
        public long DeviceTime
        {
            get
            {
                return 0L;
            }
            set
            {
                
            }
        }
    }
}
