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
using OpenFeasyo.Platform.Controls.Drivers;
using OpenFeasyo.Platform.Network;

namespace OpenFeasyo.Platform.Configuration
{
    public enum InputType { 
        BalanceBoard,
        SkeletalAngles,
        SkeletalJoint,
        Emg
    }

    public abstract class InputConfig
    {
        public string BindingPoint { get; set; }

        public IDevice Device { get; set; }

        public abstract InputType Type { get; }

        public abstract void SendConfig(IObject obj);

        #region overriding Equals()

        //public override bool Equals(object obj)
        //{
        //    if (obj == null)
        //    {
        //        return false;
        //    }

        //    InputConfig b = obj as InputConfig;
        //    if ((object)b == null)
        //    {
        //        return false;
        //    }

        //    return true;//b.BindingPoint == BindingPoint;
        //}

        //public bool Equals(InputConfig cfg)
        //{
        //    if ((object)cfg == null)
        //    {
        //        return false;
        //    }
        //    return true;//cfg.BindingPoint == BindingPoint;
        //}

        #endregion overriding Equals()
    }
}
