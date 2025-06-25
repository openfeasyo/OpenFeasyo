using GhostlyLib.Elements.Character;
using GhostlyLib.Elements.Enemies;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Level
{
    public interface ILevel3D : ILevel
    {
        //IGameCharacter Character { get; }
        BasicEffect Crate { get; }
        BasicEffect Star { get; }
        BasicEffect Exit { get; }
        BasicEffect Invisible { get; }

        IEnemy GenerateEnemy(int i, int j);
    }
}
