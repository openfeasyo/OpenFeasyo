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
using OpenFeasyo.Platform.Network;

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
