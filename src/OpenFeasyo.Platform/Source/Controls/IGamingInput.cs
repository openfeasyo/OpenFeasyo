using OpenFeasyo.Platform.Controls.Drivers;
using System;
using System.Collections.Generic;

namespace OpenFeasyo.Platform.Controls
{
    ///<summary>
    /// General interface for all kinds of inputs for rehabilitation games.
    ///</summary>
    public interface IGamingInput
    {
        IDevice Device
        {
            get;
        }

//        void DrawControlsState(SpriteBatch spriteBatch, Rectangle areaToDraw);

    }
}
