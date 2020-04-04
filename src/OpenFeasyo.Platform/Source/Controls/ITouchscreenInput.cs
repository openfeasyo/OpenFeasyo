using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFeasyo.Platform.Controls
{
    public interface ITouchscreenInput : IGamingInput
    {
        event EventHandler<TouchChangedEventArgs> TouchChanged;
    }

    public class TouchChangedEventArgs : EventArgs
    {

        private ITouch _touch;

        public TouchChangedEventArgs(ITouch touch) { _touch = touch; }

        public ITouch Touch { get { return _touch; } }
    }
}
