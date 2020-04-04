using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Drivers;
using System;

namespace OpenFeasyo.Platform.Network.Controls
{
    public class BalanceBoardInputProxy : IBalanceBoardInput
    {
        private DeviceProxy _device;

        public BalanceBoardInputProxy(DeviceProxy device) {
            _device = device;
        }

        public IDevice Device
        {
            get { return _device; }
        }

        public event EventHandler<BalanceChangedEventArgs> BalanceChanged;

        internal void OnNewBalanceBoard(int[] balance)
        {
            if (BalanceChanged != null)
            {
                BalanceChanged(this,
                    new BalanceChangedEventArgs(
                        new BalanceBoardProxy(balance)));
            }
        }
    }
}
