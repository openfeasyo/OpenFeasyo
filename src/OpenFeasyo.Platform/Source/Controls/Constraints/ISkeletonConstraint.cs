using OpenFeasyo.Platform.Controls.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Constraints
{
    public interface ISkeletonConstraint
    {
        void Check(ISkeleton skeleton, ISkeletonReport report);
    }
}
