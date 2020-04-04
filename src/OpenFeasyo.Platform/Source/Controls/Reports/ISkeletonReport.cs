using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Reports
{
    public interface ISkeletonReport : IReport
    {
        ISkeleton GoodSkeleton { get; set; }
        ISkeleton CurrentSkeleton { get; set; }

        SkeletonCorrectiveCollection CorrectiveCollection { get; set; }
    }


    public enum PlayerJoint
    {
        HipCenter = 0,
        Spine = 1,
        ShoulderCenter = 3,
        Head = 4,
        Sternum = 5,
        ShoulderRight = 6,
        ElbowRight = 7,
        WristRight = 8,
        HandRight = 9,
        ShoulderLeft = 11,
        ElbowLeft = 12,
        WristLeft = 13,
        HandLeft = 14,
        HipLeft = 16,
        KneeLeft = 17,
        AnkleLeft = 18,
        FootLeft = 19,
        HipRight = 21,
        KneeRight = 22,
        AnkleRight = 23,
        FootRight = 24,
    }
}
