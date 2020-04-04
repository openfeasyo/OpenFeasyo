using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Reports
{
    public interface IReport
    {
        IReport Copy();

        string Sender { get; }

        bool IsReportingProblem { get; set; }
        string Description { get; set; }
        float Quality { get; set; }
        int GeneralSeverity { get; set; }

    }
}
