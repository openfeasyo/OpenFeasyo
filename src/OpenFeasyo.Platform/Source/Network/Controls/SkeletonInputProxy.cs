using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Network.Controls
{
    public class SkeletonInputProxy: ISkeletonInput
    {
        private DeviceProxy _device;

        internal SkeletonInputProxy(DeviceProxy device)
        {
            _device = device;
        }

        public IDevice Device
        {
            get { return _device; }
        }

        public event EventHandler<SkeletonChangedEventArgs> SkeletonChanged;

        internal void OnNewSkeleton(int [] skeleton) {
            if (SkeletonChanged != null) 
            {
                SkeletonChanged(this, 
                    new SkeletonChangedEventArgs(
                        new SkeletonProxy(skeleton)));
            }
        }
    }
}
