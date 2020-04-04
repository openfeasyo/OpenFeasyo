using Microsoft.Xna.Framework;

namespace OpenFeasyo.GameTools.Core
{
    public interface ICamera
    {
        Matrix Projection { get; }
        Matrix View { get; }
    }
}
