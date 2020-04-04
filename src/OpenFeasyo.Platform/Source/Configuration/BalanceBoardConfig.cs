using Microsoft.Xna.Framework;
using System;

using OpenFeasyo.Platform.Network;

namespace OpenFeasyo.Platform.Configuration
{
    public class BalanceBoardConfig : InputConfig
    {
        public override InputType Type { 
            get { 
                return InputType.BalanceBoard; 
            } 
        }

        public int Direction { get; set; }
        public Vector2 CenterDisplacement { get; set; }
        public float RangeRed { get; set; }
        public float RangeBlue { get; set; }

        public BalanceBoardConfig(){
            CenterDisplacement = Vector2.Zero;
        }

        public override void SendConfig(IObject obj)
        {
            bool vertical = Direction == 0 || Direction == 180;
            Console.WriteLine("  Direction: " + Direction);
            obj.SetBalanceBoardBinding(BindingPoint, Direction, vertical ? CenterDisplacement.Y : CenterDisplacement.X, RangeRed, RangeBlue);
        }

        #region overriding Equals()

        public override bool Equals(object obj)
        {
            BalanceBoardConfig c = obj as BalanceBoardConfig;
            if ((object)c == null)
            {
                return false;
            }
            return //base.Equals(obj) && 
                c.Direction == Direction &&
                c.CenterDisplacement == CenterDisplacement &&
                c.RangeRed == RangeRed &&
                c.RangeBlue == RangeBlue;
        }

        public bool Equals(BalanceBoardConfig c)
        {
            return base.Equals((InputConfig)c) &&
                c.Direction == Direction &&
                c.CenterDisplacement == CenterDisplacement &&
                c.RangeRed == RangeRed &&
                c.RangeBlue == RangeBlue;
        }

        #endregion overriding Equals()
    }
}
