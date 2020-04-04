using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls
{
    public interface IBalanceBoard
    {
        float Weight { get; }

        Vector2 CenterOfPressure { get; }
    }
}
