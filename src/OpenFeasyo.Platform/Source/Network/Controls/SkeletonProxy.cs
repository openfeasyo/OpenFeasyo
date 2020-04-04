using OpenFeasyo.Platform.Controls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
