
using OpenFeasyo.Platform.Controls;
using Microsoft.Xna.Framework;

namespace OpenFeasyo.Platform.Network.Controls
{
    public class BalanceBoardProxy : IBalanceBoard
    {
        private Vector2 _cop;
        private float _weight;

        public BalanceBoardProxy(int [] balance){
            _cop = new Vector2(
                ((float)balance[0]) / 100f,
                ((float)balance[1]) / 100f);
            _weight = ((float)balance[2]) / 100f;
        }

        public float Weight
        {
            get { return _weight; }
        }

        public Vector2 CenterOfPressure
        {
            get { return _cop; }
        }
    }
}
