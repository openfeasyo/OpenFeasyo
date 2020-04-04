using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Analysis
{
    public interface IAccelerometerAnalyzer : IAnalyzer
    {
        void OnAccelerometerChanged(IAccelerometer newAccelerometer, IGame game);
    }
}
