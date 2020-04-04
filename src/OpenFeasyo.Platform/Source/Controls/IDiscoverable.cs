using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFeasyo.Platform.Controls
{
    public interface IDiscoverable
    {
        void ScanAsync();

        event EventHandler<ScanResultsEventArgs> ScanFinished;

        void ConnectAsync(string device);

        event EventHandler<ConnectionEventArgs> ConnectionEstablished;

        event EventHandler<ConnectionEventArgs> ConnectionFailed;

    }

    public class ScanResultsEventArgs : EventArgs
    {
        private ICollection<string> _devices;
        public ICollection<string> Devices { get { return _devices; } }

        public ScanResultsEventArgs(ICollection<string> devices) {
            _devices = devices;
        }
    }

    public class ConnectionEventArgs : EventArgs
    {
        private string _device;
        public string Devices { get { return _device; } }

        public ConnectionEventArgs(string device)
        {
            _device = device;
        }
    }

}
