using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OpenFeasyo.Platform.Controls.Drivers;

namespace OpenFeasyo.Platform.Controls
{
    public class AGamingInput : IGamingInput
    {
        protected AGamingInput(IDevice parrentDevice) {
            _device = parrentDevice;
        }

        private IDevice _device;
        public IDevice Device
        {
            get {
                return _device;
            }
        }

        //public virtual void DrawControlsState(SpriteBatch spriteBatch, Rectangle areaToDraw) { }
    }
}
