using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls
{
    public interface IAccelerometer
    {
        float AngleX { get; }

        float AngleY { get; }

        float AngleZ { get; }
    }
}
