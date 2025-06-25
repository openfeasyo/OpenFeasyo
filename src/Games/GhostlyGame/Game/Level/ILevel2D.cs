using GhostlyLib.Animations;
using GhostlyLib.Elements.Character;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Level
{
    public interface ILevel2D : ILevel
    {
        IGameCharacter Character { get; }
        
        Texture2D Star { get; }
        //Texture2D Exit { get; }
        
        Texture2D Background { get; }
        Texture2D BackgroundClosest { get; }
        Texture2D BackgroundCloser { get; }
        Texture2D BackgroundClose { get; }
        Texture2D BackgroundFar { get; }
        Texture2D BackgroundFurther { get; }
        Texture2D BackgroundFurthest { get; }

        EnemyAnimation BlackEnemyAnimation { get; }
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
        EnemyAnimation GreenEnemyAnimation { get; }
        Texture2D Ground { get; }
        Texture2D Invisible { get; }
        Texture2D LargeHill { get; }
        Texture2D Lava { get; }
        EnemyAnimation RedEnemyAnimation { get; }
        Texture2D SmallHill { get; }
        Texture2D Water { get; }
        EnemyAnimation YellowEnemyAnimation { get; }


        //Textures for space levels
        Texture2D BluePlanet { get; }
        Texture2D YellowPlanet { get; }
        Texture2D OrangePlanet { get; }
        Texture2D PinkPlanet { get; }
        Texture2D RedPlanet { get; }
    }
}
