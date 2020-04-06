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
using System;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace OpenFeasyo.Platform.Controls
{
    #region SkeletonMarkers enum

    public enum SkeletonMarkers // | Kinect | Kinect2 | OpenNI | IISU | Vicon | Vicon C3D label
    {                           // +---+---+---+---+---+
        HipCenter = 0,          // | X | X | - | X | X | SACR
        Spine = 1,              // | X | X | X | X | X | TH7
        ShoulderCenter = 2,     // | X | X | X | X | X | TH2
        Head = 3,               // | X | X | X | X | X | HEAD
        ShoulderLeft = 4,       // | X | X | X | X | X | LSHO 
        ElbowLeft = 5,          // | X | X | X | X | X | LELB 
        WristLeft = 6,          // | X | X | - | - | X | LSTR 
        HandLeft = 7,           // | X | X | X | X | X | LFIN 
        ShoulderRight = 8,      // | X | X | X | X | X | RSHO 
        ElbowRight = 9,         // | X | X | X | X | X | RELB 
        WristRight = 10,        // | X | X | - | - | X | RSTR 
        HandRight = 11,         // | X | X | X | X | X | RFIN 
        HipLeft = 12,           // | X | X | X | X | X | LGT
        KneeLeft = 13,          // | X | X | X | X | X | LKNE 
        AnkleLeft = 14,         // | X | X | - | - | X | LANK 
        FootLeft = 15,          // | X | X | X | X | X | LHLX
        HipRight = 16,          // | X | X | X | X | X | RGT
        KneeRight = 17,         // | X | X | X | X | X | RKNE 
        AnkleRight = 18,        // | X | X | - | - | X | RANK 
        FootRight = 19,         // | X | X | X | X | X | RHLX
        SpineShoulder = 20,     // | - | X | ? | ? | ? | ???
        HandTipLeft = 21,       // | - | X | ? | ? | ? | ???
        ThumbLeft = 22,         // | - | X | ? | ? | ? | ??? 
        HandTipRight = 23,      // | - | X | ? | ? | ? | ???
        ThumbRight = 24,        // | - | X | ? | ? | ? | ???
        Sternum = 25,           // | - | - | - | X | X | STRN
        Count = 26,             // +---+---+---+---+---+
    }

    public class BonePartsValue : System.Attribute
    {
        private SkeletonMarkers _marker1;
        private SkeletonMarkers _marker2;

        public BonePartsValue(SkeletonMarkers firstMarker, SkeletonMarkers secondMarker)
        {
            _marker1 = firstMarker;
            _marker2 = secondMarker;
        }

        public SkeletonMarkers FirsrMarker
        {
            get { return _marker1; }
        }

        public SkeletonMarkers SecondMarker
        {
            get { return _marker2; }
        }

        public static BonePartsValue GetMarkersFrom(Enum value){
            FieldInfo field = value.GetType().GetField(value.ToString());
            return Attribute.GetCustomAttribute(field, typeof(BonePartsValue)) as BonePartsValue;
        }
    }



    public enum BoneMarkers
    {
        [BonePartsValue(SkeletonMarkers.Head,SkeletonMarkers.ShoulderCenter)]           Head = SkeletonMarkers.Count,
        [BonePartsValue(SkeletonMarkers.ShoulderCenter,SkeletonMarkers.ShoulderLeft)]   ShoulderLeft,
        [BonePartsValue(SkeletonMarkers.ShoulderCenter,SkeletonMarkers.ShoulderRight)]  ShoulderRight,
        [BonePartsValue(SkeletonMarkers.ShoulderLeft,SkeletonMarkers.ElbowLeft)]        UpperArmLeft,
        [BonePartsValue(SkeletonMarkers.ShoulderRight,SkeletonMarkers.ElbowRight)]      UpperArmRight,
        [BonePartsValue(SkeletonMarkers.ElbowLeft,SkeletonMarkers.WristLeft)]           LowerArmLeft,
        [BonePartsValue(SkeletonMarkers.ElbowRight,SkeletonMarkers.WristRight)]         LowerArmRight,
        [BonePartsValue(SkeletonMarkers.WristLeft,SkeletonMarkers.HandLeft)]            PalmLeft,
        [BonePartsValue(SkeletonMarkers.WristRight,SkeletonMarkers.HandRight)]          PalmRight,
        [BonePartsValue(SkeletonMarkers.ShoulderCenter,SkeletonMarkers.Spine)]          Spine,
        [BonePartsValue(SkeletonMarkers.Spine,SkeletonMarkers.HipCenter)]               Hip,
        [BonePartsValue(SkeletonMarkers.HipCenter,SkeletonMarkers.HipLeft)]             HipLeft,
        [BonePartsValue(SkeletonMarkers.HipCenter,SkeletonMarkers.HipRight)]            HipRight,
        [BonePartsValue(SkeletonMarkers.HipLeft,SkeletonMarkers.KneeLeft)]              UpperLegLeft,
        [BonePartsValue(SkeletonMarkers.HipRight,SkeletonMarkers.KneeRight)]            UpperLegRight,
        [BonePartsValue(SkeletonMarkers.KneeLeft,SkeletonMarkers.AnkleLeft)]            LowerLegLeft,
        [BonePartsValue(SkeletonMarkers.KneeRight,SkeletonMarkers.AnkleRight)]          LowerLegRight,
        [BonePartsValue(SkeletonMarkers.AnkleLeft,SkeletonMarkers.FootLeft)]            FootLeft,
        [BonePartsValue(SkeletonMarkers.AnkleRight,SkeletonMarkers.FootRight)]          FootRight
    }

    #endregion

    public interface ISkeleton
    {
        Vector3 GetPositionOf(SkeletonMarkers marker);
        long DeviceTime { get; set; }
    }

    public interface IRichSkeleton : ISkeleton
    {
        Matrix GetOrientationOf(SkeletonMarkers marker);
        bool GetQualityOf(SkeletonMarkers marker);
    }
}
