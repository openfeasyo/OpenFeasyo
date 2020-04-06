/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
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
