using OpenFeasyo.Platform.Network;
using System;

namespace OpenFeasyo.Platform.Configuration
{
    public class EmgConfig : InputConfig
    {
        public override InputType Type {
        get {
                return InputType.Emg;
            }
        }

        public override void SendConfig(IObject obj)
        {
            throw new NotImplementedException();
        }
    }
}
