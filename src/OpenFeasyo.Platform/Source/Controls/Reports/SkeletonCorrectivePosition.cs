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
using Microsoft.Xna.Framework;
namespace OpenFeasyo.Platform.Controls.Reports
{
    public class SkeletonCorrecticePosition : ISkeletonCorrective
    {
        private int severity;
        public int Severity
        {
            get { return severity; }
            set { severity = value; }
        }
        private PlayerJoint joint;
        public PlayerJoint Joint
        {
            get { return joint; }
            set { joint = value; }
        }
        private Vector3 currentPosition;
        public Vector3 CurrentPosition
        {
            get { return currentPosition; }
            set { currentPosition = value; }
        }
        private Vector3 wishedPosition;
        public Vector3 WishedPosition
        {
            get { return wishedPosition; }
            set { wishedPosition = value; }
        }
        private Vector3 minPosition;
        public Vector3 MinPosition
        {
            get { return minPosition; }
            set { minPosition = value; }
        }
        private Vector3 maxPosition;
        public Vector3 MaxPosition
        {
            get { return maxPosition; }
            set { maxPosition = value; }
        }

        public SkeletonCorrecticePosition()
        {
            severity = 1;
            joint = PlayerJoint.HipCenter;
            currentPosition = Vector3.Zero;
            wishedPosition = Vector3.Zero;
            minPosition = Vector3.Zero;
            maxPosition = Vector3.Zero;
        }

        public SkeletonCorrecticePosition(PlayerJoint _joint, Vector3 _currentPosition, Vector3 _whishedPosition, Vector3 _minPosition, Vector3 _maxPosition, int _severity)
        {
            severity = _severity;
            joint = _joint;
            currentPosition = _currentPosition;
            wishedPosition = _whishedPosition;
            minPosition = _minPosition;
            maxPosition = _maxPosition;
        }
    }
}
