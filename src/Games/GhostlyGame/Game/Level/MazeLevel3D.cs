using GhostlyLib.Animations;
using GhostlyLib.Elements.Character;
using GhostlyLib.Elements.Enemies;
using GhostlyLib.Elements;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Level
{
    public class MazeLevel3D : Level3D
    {
        #region Private members
        private LevelElements _elements;
        #endregion Private members

        #region Public members

        public override Texture2D Background { get { return ImagesAndAnimations.Instance.Background; } }

        public override LevelElements Elements
        {
            get { return this._elements; }
        }

        public override IGameCharacter Character { get; set; }

        public override EnemyAnimation RedEnemyAnimation { get { return ImagesAndAnimations.Instance.RedEnemyAnimation; } }

        #endregion Public members

        public MazeLevel3D(GameScreen gameScreen, LevelElements elements) : base(gameScreen)
        {
            this._elements = elements;
            this.Character = new MazeCharacter3D(gameScreen, elements, ThreeDEffects.Instance.Character, ThreeDEffects.Instance.CharacterLeftRotating, ThreeDEffects.Instance.CharacterRightRotating);
        }

        public override void ProcessPrimaryAction(bool state)
        {
            //TODO
            //throw new NotImplementedException();
        }

        public override void ProcessSecondaryAction(bool state)
        {
            //TODO
            //throw new NotImplementedException();
        }

        public override IEnemy GenerateEnemy(int i, int j)
        {
            return new MazeEnemy3D(i, j, this.Elements, this.RedEnemyAnimation, this.GameScreen);
        }
    }
}