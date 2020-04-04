using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Reports
{
    public class SkeletonCorrectiveCollection
    {

        private List<ISkeletonCorrective> correctiveItems;
        public List<ISkeletonCorrective> CorrectiveItems
        {
            get { return correctiveItems; }
            set { correctiveItems = value; }
        }

        public SkeletonCorrectiveCollection()
        {
            correctiveItems = new List<ISkeletonCorrective>();
        }
    }
}
