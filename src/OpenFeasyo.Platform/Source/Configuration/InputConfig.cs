using OpenFeasyo.Platform.Controls.Drivers;
using OpenFeasyo.Platform.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
