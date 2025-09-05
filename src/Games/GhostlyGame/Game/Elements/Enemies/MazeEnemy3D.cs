using GhostlyLib.Animations;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Elements.Enemies
{
    public class MazeEnemy3D : EasyEnemy3D
    {
        public override int Height { get { return 38; } }
        public override int Width { get { return 38; } }

        public override BasicEffect Effect { get { return ThreeDEffects.Instance.Enemy; } }

        public MazeEnemy3D(int x, int y, LevelElements elements, EnemyAnimation animation, GameScreen gameScreen) : base(x, y, elements, gameScreen)
        {
            this.Animation = animation;
            this.Animation.SetCurrentFrames(EnemyState.FullHealth);
        }
    }
}