using GhostlyLib.Screens;

namespace GhostlyLib.Elements.Enemies
{
    public abstract class EasyEnemy3D : Enemy3D
    {
        public override EnemyState State { get { return EnemyState.FullHealth; } }

        public EasyEnemy3D(int x, int y, LevelElements elements, GameScreen gameScreen) : base(x, y, elements, gameScreen)
        {
            this.CurrentHealth = 1;
            this.Bonus = 1;
        }
    }
}