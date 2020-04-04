using OpenFeasyo.Platform.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFeasyo.Platform.Configuration
{
    public class JointPositionConfig : InputConfig
    {
        public override InputType Type
        {
            get
            {
                return InputType.SkeletalJoint;
            }
        }

        public int JointTracked { get; set; }
        public int JointBase { get; set; }
        public float Range { get; set; }
        public int Axis { get; set; }

        public override void SendConfig(IObject obj)
        {
            obj.SetJointPositionBinding(BindingPoint, JointTracked, JointBase, Range, Axis);
        }

        #region overriding Equals()

        public override bool Equals(object obj)
        {
            JointPositionConfig c = obj as JointPositionConfig;
            if ((object)c == null)
            {
                return false;
            }
            return //base.Equals(obj) &&
                c.JointTracked == JointTracked &&
                c.JointBase == JointBase &&
                c.Range == Range &&
                c.Axis == c.Axis;
        }

        public bool Equals(JointPositionConfig c)
        {
            return base.Equals((InputConfig)c) &&
                c.JointTracked == JointTracked &&
                c.JointBase == JointBase &&
                c.Range == Range &&
                c.Axis == c.Axis;
        }

        #endregion overriding Equals()
    }
}
