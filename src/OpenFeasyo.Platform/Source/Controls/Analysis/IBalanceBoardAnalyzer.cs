using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Analysis
{
    public interface IBalanceBoardAnalyzer : IAnalyzer
    {
        void OnBalanceChanged(IBalanceBoard newBalance, IGame game);  
    }
}
