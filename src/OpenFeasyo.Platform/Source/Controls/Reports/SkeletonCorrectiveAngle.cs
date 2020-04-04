using OpenFeasyo.Platform.Controls.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
