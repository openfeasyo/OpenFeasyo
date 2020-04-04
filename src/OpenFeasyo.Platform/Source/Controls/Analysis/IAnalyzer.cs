using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Analysis
{
    public interface IAnalyzer
    {
        void OnCreate(Dictionary<string, string> parameters, IGame game);

        void OnDestroy();
    }
}
