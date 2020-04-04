using OpenFeasyo.Platform.Controls.Reports;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Constraints
{
    public class SkeletonConstraintPosition : ISkeletonConstraint
    { //in meter
        private PlayerJoint joint;

        public PlayerJoint Joint
        {
            get { return joint; }
            set { joint = value; }
        }
        private Vector3 minPos;
        public Vector3 MinPos
        {
            get { return minPos; }
            set { minPos = value; }
        }
        private Vector3 maxPos;
        public Vector3 MaxPos
        {
            get { return maxPos; }
            set { maxPos = value; }
        }
        private int wishness;
        public int Wishness
        {
            get { return wishness; }
            set { wishness = value; }
        }

        public SkeletonConstraintPosition()
        {
            joint = PlayerJoint.HipCenter;
            minPos = Vector3.Zero;
            maxPos = Vector3.Zero;
            wishness = 0;
        }

        public SkeletonConstraintPosition(PlayerJoint _joint, Vector3 _minPos, Vector3 _maxPos, int _wishness)
        {
            joint = _joint;
            minPos = _minPos;
            maxPos = _maxPos;
            wishness = _wishness;
        }

        public void Check(ISkeleton skeleton, ISkeletonReport report)
        {
            if (Wishness == 0)
            {
                return;
            }

            //the positions we recieve from the Kinect are absolute positions ...
            //we have to use relative position if we want to apply a constraint position efficiently
            //-> use the hipcenter as (0,0,0) and convert the rest in relative positions
            //warning : the values are in meter !
            Vector3 currentRelativePosition;

            #region getCurrentRelativePosition
            switch (Joint)
            {
                case PlayerJoint.HipCenter:
                    currentRelativePosition = Vector3.Zero;
                    break;
                case PlayerJoint.AnkleLeft:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.AnkleLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.AnkleRight:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.AnkleRight),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.ElbowLeft:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ElbowLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.ElbowRight:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ElbowRight),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.FootLeft:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.FootLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.FootRight:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.FootRight),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.HandLeft:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.HandLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.HandRight:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.HandRight),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.Head:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.Head),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.HipLeft:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.HipLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.HipRight:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.HipRight),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.KneeLeft:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.KneeLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.KneeRight:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.KneeRight),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.Sternum:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.Sternum),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.ShoulderCenter:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ShoulderCenter),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.ShoulderLeft:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ShoulderLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.ShoulderRight:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ShoulderRight),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.Spine:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.Spine),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.WristLeft:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.WristLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                case PlayerJoint.WristRight:
                    currentRelativePosition = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.WristRight),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));
                    break;
                default:
                    currentRelativePosition = Vector3.Zero;
                    break;
            }
            #endregion

            //distance recieved in millimeters????
            currentRelativePosition /= 1000;

            //if the values are the same we do not test this direction
            bool xProblemAnalyze = MinPos.X != MaxPos.X;
            bool yProblemAnalyze = MinPos.Y != MaxPos.Y;
            bool zProblemAnalyze = MinPos.Z != MaxPos.Z;

            if (Wishness > 0)
            { //we want to reach this position

                if ((xProblemAnalyze &&
                    (currentRelativePosition.X < MinPos.X || currentRelativePosition.X > MaxPos.X)) ||
                    (yProblemAnalyze &&
                    (currentRelativePosition.Y < MinPos.Y || currentRelativePosition.Y > MaxPos.Y)) ||
                    (zProblemAnalyze &&
                    (currentRelativePosition.Z < MinPos.Z || currentRelativePosition.Z > MaxPos.Z)))
                {//well there's one problem but we don't really care which one here

                    if (report.GeneralSeverity.CompareTo(Math.Abs(Wishness)) < 0)
                        report.GeneralSeverity = Math.Abs(Wishness);

                    report.CorrectiveCollection.CorrectiveItems.Add(new SkeletonCorrecticePosition(
                        Joint, currentRelativePosition,
                        Vector3.Divide(Vector3.Add(MinPos, MaxPos), 2),
                        MinPos, MaxPos, Wishness));
                }

            }
            else if (Wishness < 0)
            { //we don't want to reach this position

                bool xProblemDetected = false;
                bool yProblemDetected = false;
                bool zProblemDetected = false;

                if (xProblemAnalyze &&
                    currentRelativePosition.X > MinPos.X &&
                    currentRelativePosition.X < MaxPos.X)
                {//problem and we want to check it
                    xProblemDetected = true;
                }

                if (yProblemAnalyze &&
                    currentRelativePosition.Y > MinPos.Y &&
                    currentRelativePosition.Y < MaxPos.Y)
                {
                    yProblemDetected = true;
                }

                if (zProblemAnalyze &&
                    currentRelativePosition.Z > MinPos.Z &&
                    currentRelativePosition.Z < MaxPos.Z)
                {
                    zProblemAnalyze = true;
                }

                if ((xProblemDetected || !xProblemAnalyze) &&
                   (yProblemDetected || !yProblemAnalyze) &&
                   (zProblemDetected || !zProblemAnalyze) &&
                   (xProblemAnalyze || yProblemAnalyze || zProblemAnalyze))
                {//here we can be sure that, if we want to detect a problem on one or mutiple direction, it will be detected
                    //we are in a position which is not good !!
                    if (report.GeneralSeverity.CompareTo(Math.Abs(Wishness)) < 0)
                        report.GeneralSeverity = Math.Abs(Wishness);

                    report.CorrectiveCollection.CorrectiveItems.Add(new SkeletonCorrecticePosition(
                        Joint, currentRelativePosition,
                        Vector3.Divide(Vector3.Add(MinPos, MaxPos), 2),
                        MinPos, MaxPos, Wishness));
                }
            }
        }

    }
}
