using OpenFeasyo.Platform.Controls.Reports;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Constraints
{
    public class SkeletonConstraintAngle : ISkeletonConstraint
    { //in degrees
        private PlayerJoint angle;
        public PlayerJoint Angle
        {
            get { return angle; }
            set { angle = value; }
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
        private int wishness;
        public int Wishness
        {
            get { return wishness; }
            set { wishness = value; }
        }

        private physioPlanes restrictionPlane;
        public physioPlanes RestrictionPlane
        {
            get { return restrictionPlane; }
            set { restrictionPlane = value; }
        }
        
        public SkeletonConstraintAngle()
        {
            angle = PlayerJoint.Spine;
            threshold = 15;
            wishedAngle = 0;
            wishness = 1;
            restrictionPlane = physioPlanes.NONE;
        }

        public SkeletonConstraintAngle(PlayerJoint _angle, float _wishedAngle, float _threshold, int _wishness, physioPlanes plane)
        {
            angle = _angle;
            threshold = _threshold;
            wishedAngle = _wishedAngle;
            wishness = _wishness;
            restrictionPlane = plane;
        }

        public void Check(ISkeleton skeleton, ISkeletonReport report)
        {
            SkeletonCorrectiveAngle correctiveAngle = new SkeletonCorrectiveAngle();

            if (Wishness == 0)
            {
                return;
            }

            Vector3 firstBone;
            Vector3 secondBone;
            float currentAngle;

            #region defineBonesToUse
            //we globally make the vectors point in opposite direction 
            //ex : when we compare the chest and the vertical vector, the two vectors are pointing up
            //ex : when we compare the chest and the head, the head vector points down
            //ex : when we talk about chest, the arms and legs point out of the chest
            //ex : when we compare lower parts with upper parts : the upper parts point to the chest and the lower parts to wirsts and ankles
            //ex : when we compare lower/upper parts : they are pointing out of the chest
            //ex : when we talk about legs, the chest point up and, when we talk about arms, it's pointing down
            //the sternum define the angle between the shoulders vector and the hips vector -> the trunk torsion
            //this is important to correctly compare the angles 
            switch (Angle)
            {
                case PlayerJoint.Spine:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ShoulderCenter),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));//chest
                    secondBone = Vector3.Up;
                    break;
                case PlayerJoint.Sternum:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ShoulderRight),
                        skeleton.GetPositionOf(SkeletonMarkers.ShoulderLeft));//shoulders (pointing right)
                    secondBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.HipRight),
                        skeleton.GetPositionOf(SkeletonMarkers.HipLeft));//hips (pointing right)
                    break;
                case PlayerJoint.Head:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ShoulderCenter),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));//chest
                    secondBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ShoulderCenter),
                        skeleton.GetPositionOf(SkeletonMarkers.Head));//head (pointing down)
                    break;
                case PlayerJoint.ShoulderLeft:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.HipCenter),
                        skeleton.GetPositionOf(SkeletonMarkers.ShoulderCenter));//chest
                    secondBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ElbowLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.ShoulderLeft));//left upper arm
                    break;
                case PlayerJoint.ElbowLeft:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ShoulderLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.ElbowLeft));//left upper arm
                    secondBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.WristLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.ElbowLeft));//left lower arm
                    break;
                case PlayerJoint.HipLeft:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ShoulderCenter),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));//chest
                    secondBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.KneeLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.HipLeft));//left upper leg
                    break;
                case PlayerJoint.KneeLeft:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.HipLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.KneeLeft));//left upper leg
                    secondBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.AnkleLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.KneeLeft));//left lower leg
                    break;
                case PlayerJoint.ShoulderRight:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.HipCenter),
                        skeleton.GetPositionOf(SkeletonMarkers.ShoulderCenter));//chest
                    secondBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ElbowRight),
                        skeleton.GetPositionOf(SkeletonMarkers.ShoulderRight));//right upper arm
                    break;
                case PlayerJoint.ShoulderCenter:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ElbowRight),
                        skeleton.GetPositionOf(SkeletonMarkers.ShoulderRight));//right upper arm
                    secondBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ElbowLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.ShoulderLeft));//left upper arm
                    break;
                case PlayerJoint.ElbowRight:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ShoulderRight),
                        skeleton.GetPositionOf(SkeletonMarkers.ElbowRight));//right upper arm
                    secondBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.WristRight),
                        skeleton.GetPositionOf(SkeletonMarkers.ElbowRight));//right lower arm
                    break;
                case PlayerJoint.HipRight:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.ShoulderCenter),
                        skeleton.GetPositionOf(SkeletonMarkers.HipCenter));//chest
                    secondBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.KneeRight),
                        skeleton.GetPositionOf(SkeletonMarkers.HipRight));//right upper leg
                    break;
                case PlayerJoint.HipCenter:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.KneeLeft),
                        skeleton.GetPositionOf(SkeletonMarkers.HipLeft));//left upper leg
                    secondBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.KneeRight),
                        skeleton.GetPositionOf(SkeletonMarkers.HipRight));//right upper leg
                    break;
                case PlayerJoint.KneeRight:
                    firstBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.HipRight),
                        skeleton.GetPositionOf(SkeletonMarkers.KneeRight));//left upper leg
                    secondBone = Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.AnkleRight),
                        skeleton.GetPositionOf(SkeletonMarkers.KneeRight));//left lower leg
                    break;
                default:
                    firstBone = Vector3.Zero;
                    secondBone = Vector3.Zero;
                    break;
            }
            #endregion

            #region check the angle
            currentAngle = MathHelper.ToDegrees(GetAngleFromVectors(firstBone, secondBone));

            if (Wishness > 0 && Math.Abs(currentAngle - WishedAngle) > Threshold)
            { //we want to reach this position and we currently don't -> wrong
                correctiveAngle = new SkeletonCorrectiveAngle(Angle, currentAngle, WishedAngle, Threshold,
                                                                Wishness, restrictionPlane, true);
            }
            else if (Wishness < 0 && Math.Abs(currentAngle - WishedAngle) < Threshold)
            { //we don't want to reach this position but we currently do -> wrong
                correctiveAngle = new SkeletonCorrectiveAngle(Angle, currentAngle, WishedAngle, Threshold,
                                                                Wishness, restrictionPlane, true);
            }
            #endregion

            #region check if in the right plane
            //the idea is, now, to check if the bones used are, more or less, in the choosen plane
            //we will mesure the angle between the normal to the plane and the normal to the two vectors which forms the angle
            //this angle will give us an information about the respect of this restriction plane

            //create the frontal plane
            Plane plane = new Plane(skeleton.GetPositionOf(SkeletonMarkers.HipRight),
                                        skeleton.GetPositionOf(SkeletonMarkers.HipLeft),
                                        skeleton.GetPositionOf(SkeletonMarkers.ShoulderCenter));

            //define the plane to use
            switch (restrictionPlane) 
            {
                case physioPlanes.NONE:
                    return;
                case physioPlanes.FRONTAL:
                    break;
                case physioPlanes.SAGITTAL:
                    //that's the plane defined by the normal to the frontal plane and the trunk vector
                    //so we take the cross of these two vectors -> that define the normal to the plane 
                    //the distance from the origine along these normal doesn't matter for us
                    plane = new Plane(Vector3.Cross(plane.Normal,
                                                    Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.HipCenter),
                                                                     skeleton.GetPositionOf(SkeletonMarkers.ShoulderCenter))),
                                      0);
                    break;
                case physioPlanes.TRANSVERSE:
                    //the same principle is in application here
                    plane = new Plane(Vector3.Cross(plane.Normal,
                                                    Vector3.Subtract(skeleton.GetPositionOf(SkeletonMarkers.HipLeft),
                                                                     skeleton.GetPositionOf(SkeletonMarkers.HipRight))),
                                      0);
                    break;
                default:
                    break;
            }

            //basically, we are checking the angle with any plane parallel to the restriction plane
            float angleWithPlane = MathHelper.ToDegrees(GetAngleFromVectors(plane.Normal, Vector3.Cross(firstBone, secondBone)));
            float thresholdPlane = 15;

            if ((Math.Abs(angleWithPlane) > thresholdPlane) && (Math.Abs(angleWithPlane - 180) > thresholdPlane))
            { //we don't respect the restriction plane
                correctiveAngle = new SkeletonCorrectiveAngle(Angle, currentAngle, WishedAngle, Threshold,
                                                                Wishness, restrictionPlane, false);
            }
            #endregion

            if (correctiveAngle.Severity != 0) //so the correctiveAngle has been assigned
            {

                if (report.GeneralSeverity.CompareTo(Math.Abs(Wishness)) < 0)
                {
                    report.GeneralSeverity = Math.Abs(Wishness);
                }

                report.CorrectiveCollection.CorrectiveItems.Add(correctiveAngle);
            }
        }


        private static float GetAngleFromVectors(Vector3 v1, Vector3 v2)
        {
            v1.Normalize();
            v2.Normalize();
            float dot = Vector3.Dot(v1, v2);
            float angle = (float)Math.Acos(dot);
            return angle;
        }
    }

    public enum physioPlanes { 
    NONE,
    FRONTAL,
    SAGITTAL,
    TRANSVERSE
    }
}
