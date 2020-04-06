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
using OpenFeasyo.Platform.Controls.Constraints;

namespace OpenFeasyo.Platform.Controls.Reports
{
    public class SkeletonCorrectiveAngle : ISkeletonCorrective
    {
        private int severity;
        public int Severity
        {
            get { return severity; }
            set { severity = value; }
        }
        private PlayerJoint angle;
        public PlayerJoint Angle
        {
            get { return angle; }
            set { angle = value; }
        }
        private float currentAngle;
        public float CurrentAngle
        {
            get { return currentAngle; }
            set { currentAngle = value; }
        }
        private float wishedAngle;
        public float WishedAngle
        {
            get { return wishedAngle; }
            set { wishedAngle = value; }
        }
        private float threshold;
        public float Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        private physioPlanes restrictionPlane;
        public physioPlanes RestrictionPlane
        {
            get { return restrictionPlane; }
            set { restrictionPlane = value; }
        }
        private bool planeRespected;
        public bool PlaneRespected
        {
            get { return planeRespected; }
            set { planeRespected = value; }
        }

        public SkeletonCorrectiveAngle()
        {
            severity = 0;
            angle = PlayerJoint.Spine;
            currentAngle = 0;
            wishedAngle = 0;
            threshold = 0;
            restrictionPlane = physioPlanes.NONE;
        }

        public SkeletonCorrectiveAngle(PlayerJoint _angle, float _currentAngle, float _whishedAngle, float _threshold, int _severity, physioPlanes plane, bool planerespect)
        {
            severity = _severity;
            angle = _angle;
            currentAngle = _currentAngle;
            wishedAngle = _whishedAngle;
            threshold = _threshold;
            restrictionPlane = plane;
            planeRespected = planerespect;
        }
    }
}
