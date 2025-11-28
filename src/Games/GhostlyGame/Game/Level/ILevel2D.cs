using GhostlyLib.Animations;
using GhostlyLib.Elements.Character;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Level
{
    public interface ILevel2D : ILevel
    {
        IGameCharacter Character { get; }

        Texture2D Star { get; }
        Texture2D Background { get; }
        Texture2D BackgroundClosest { get; }
        Texture2D BackgroundCloser { get; }
        Texture2D BackgroundClose { get; }
        Texture2D BackgroundFar { get; }
        Texture2D BackgroundFurther { get; }
        Texture2D BackgroundFurthest { get; }
        Texture2D CliffLeft { get; }
        Texture2D CliffRight { get; }
        Texture2D Crate { get; }
        Texture2D DeepLava { get; }
        Texture2D DeepWater { get; }
        Texture2D Dirt { get; }
        Texture2D ExitSign { get; }
        Texture2D Exclamation { get; }
        Texture2D Fence { get; }
        Texture2D Foreground { get; }
        Texture2D Ground { get; }
        Texture2D Invisible { get; }
        Texture2D LargeHill { get; }
        Texture2D Lava { get; }
        Texture2D SmallHill { get; }
        Texture2D Water { get; }

        EnemyAnimation BlackEnemyAnimation { get; }
        EnemyAnimation GreenEnemyAnimation { get; }
        EnemyAnimation RedEnemyAnimation { get; }
        EnemyAnimation YellowEnemyAnimation { get; }
    }
}
