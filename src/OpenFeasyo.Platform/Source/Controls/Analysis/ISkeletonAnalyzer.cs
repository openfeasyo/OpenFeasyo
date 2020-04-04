using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Analysis
{
    public interface ISkeletonAnalyzer : IAnalyzer
    {
        void OnSkeletonChanged(BoneMarkers marker, ISkeleton newSkeleton, IGame game);  
    }
}
