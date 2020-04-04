using OpenFeasyo.Platform.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Configuration
{
    public class SegmentAngleConfig : InputConfig
    {
        public override InputType Type
        {
            get
            {
                return InputType.SkeletalAngles;
            }
        }

        public int BoneMarker { get; set; }
        public int Angle { get; set; }
        public int Range { get; set; }

        public override void SendConfig(IObject obj)
        {
            obj.SetAngleBinding(BindingPoint, BoneMarker, Angle, Range);
        }

        #region overriding Equals()

        public override bool Equals(object obj)
        {
            SegmentAngleConfig c = obj as SegmentAngleConfig;
            if ((object)c == null)
            {
                return false;
            }
            return //base.Equals(obj) &&
                c.Angle == Angle &&
                c.BoneMarker == BoneMarker &&
                c.Range == Range;
        }

        public bool Equals(SegmentAngleConfig c)
        {
            return base.Equals((InputConfig)c) &&
                c.Angle == Angle &&
                c.BoneMarker == BoneMarker &&
                c.Range == Range;
        }

        #endregion overriding Equals()
    }
}
