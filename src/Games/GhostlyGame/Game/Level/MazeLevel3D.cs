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

        /*public override void ProcessPrimaryAction(bool state)
        {
            //TODO uncomment and test with sensors
            /*if (state)
            {
                //rotating to the right at the moment => interupt turning
                if (this.Character.RotationStatus == RotationStatus.Rotating && this.Character.RotationDirection == RotationDirection.Right)
                {
                    ((GameCharacter3D)this.Character).TurningInterupted();
                }

                ((GameCharacter3D)this.Character).TurnCounterClockwise();
            }
            else
            {
                if (this.Character.RotationStatus == RotationStatus.Rotating && this.Character.RotationDirection == RotationDirection.Left)
                {
                    //muscle not contracted => interupt turning
                    ((GameCharacter3D)this.Character).TurningInterupted();
                }
            }
        }/*

        public override void ProcessSecondaryAction(bool state)
        {
            if (GameScreen.GameCharacter.RotationStatus == RotationStatus.Rotating && !state)
            {
                ((GameCharacter3D)GameScreen.GameCharacter).TurningInterupted();
            }
            else if (state && GameScreen.State.Equals(GameState.Running))
            {
                ((GameCharacter3D)GameScreen.GameCharacter).TurnClockwise();
            }
        }*/
        
        public override void ProcessPrimaryAction(bool state)
        {
            //TODO uncomment and test with sensors
            if (state)
            {
                //rotating to the right at the moment => interupt turning
                if (this.Character.RotationStatus == RotationStatus.Rotating && this.Character.RotationDirection == RotationDirection.Right)
                {
                    ((GameCharacter3D)this.Character).TurningInterupted();
                }
                ((GameCharacter3D)this.Character).TurnCounterClockwise();
            }
            else
            {
                if (this.Character.RotationStatus == RotationStatus.Rotating && this.Character.RotationDirection == RotationDirection.Left)
                {
                    //muscle not contracted => interupt turning
                    ((GameCharacter3D)this.Character).TurningInterupted();
                }
            }
        }

        public override void ProcessSecondaryAction(bool state)
        {
            //TODO uncomment and test with sensors
            if (state)
            {
                //rotating to the left at the moment => interupt turning
                if (this.Character.RotationStatus == RotationStatus.Rotating && this.Character.RotationDirection == RotationDirection.Left)
                {
                    ((GameCharacter3D)this.Character).TurningInterupted();
                }
                ((GameCharacter3D)this.Character).TurnClockwise();
            }
            else
            {
                //muscle not contracted => interupted turning
                ((GameCharacter3D)this.Character).TurningInterupted();
            }
        }

        public override IEnemy GenerateEnemy(int i, int j)
        {
            return new MazeEnemy3D(i, j, this.Elements, this.RedEnemyAnimation, this.GameScreen);
        }
    }
}