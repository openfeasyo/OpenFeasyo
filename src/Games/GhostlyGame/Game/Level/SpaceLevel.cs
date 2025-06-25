using GhostlyLib.Animations;
using GhostlyLib.Elements;
using GhostlyLib.Elements.Character;
using GhostlyLib.Elements.Enemies;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Level
{
    public class SpaceLevel : Level
    {
        #region Private members
        //private System.Timers.Timer _timer;
        private LevelElements _elements;
        #endregion Private members

        #region Public members
        public override Texture2D Background { get { return ImagesAndAnimations.Instance.BackgroundSpace; } }

        public override Texture2D BackgroundClosest { get { return ImagesAndAnimations.Instance.BackgroundClosestSpace; } }

        public override Texture2D BackgroundCloser { get { return ImagesAndAnimations.Instance.BackgroundCloserSpace; } }

        public override Texture2D BackgroundClose { get { return ImagesAndAnimations.Instance.BackgroundCloseSpace; } }

        public override Texture2D BackgroundFar { get { return null; } }

        public override Texture2D BackgroundFurther { get { return null; } }

        public override Texture2D BackgroundFurthest { get { return ImagesAndAnimations.Instance.BackgroundFurthestSpace; } }

        public override EnemyAnimation BlackEnemyAnimation { get { return null; } }

        public override Texture2D CliffLeft { get { return null; } }

        public override Texture2D CliffRight { get { return null; } }

        public override LevelElements Elements { get { return this._elements; } }

        public override Texture2D Dirt { get { return null; } }

        public override Texture2D Foreground { get { return null; } }

        public override EnemyAnimation GreenEnemyAnimation { get { return null; } }

        public override Texture2D Ground { get { return null; } }

        public override Texture2D LargeHill { get { return null; } }

        public override EnemyAnimation RedEnemyAnimation { get { return null; } }

        public override Texture2D SmallHill { get { return null; } }

        public override EnemyAnimation YellowEnemyAnimation { get { return null; } }

        public override IGameCharacter Character { get; set; }

        public override Texture2D BluePlanet { get { return ImagesAndAnimations.Instance.BluePlanet; } }
        public override Texture2D YellowPlanet { get { return ImagesAndAnimations.Instance.YellowPlanet; } }
        public override Texture2D OrangePlanet { get { return ImagesAndAnimations.Instance.OrangePlanet; } }
        public override Texture2D PinkPlanet { get { return ImagesAndAnimations.Instance.PinkPlanet; } }
        public override Texture2D RedPlanet { get { return ImagesAndAnimations.Instance.RedPlanet; } }
        public override Texture2D Star { get { return ImagesAndAnimations.Instance.Star; } }
        public override Texture2D Ufo { get { return ImagesAndAnimations.Instance.Ufo; } }
        public override Texture2D Debris { get { return ImagesAndAnimations.Instance.Debris; } }
        public override Texture2D ExitSign { get { return ImagesAndAnimations.Instance.ExitLine; } }
        public override Texture2D SpaceSpiral { get { return ImagesAndAnimations.Instance.SpaceSpiral; } }
        public override Texture2D SpaceMist { get { return ImagesAndAnimations.Instance.SpaceMist; } }

        #endregion Public members

        public SpaceLevel(GameScreen gameScreen, LevelElements elements) : base(gameScreen)
        {
            this._elements = elements;
            this.Character = new SpaceCharacter(gameScreen, elements);
        }

        public override Enemy CreateBlackEnemy(int i, int j, double checkpoint) { return null; }

        public override Enemy CreateGreenEnemy(int i, int j, double checkpoint) { return null; }

        public override Enemy CreateRedEnemy(int i, int j, double checkpoint) { return null; }

        public override Enemy CreateYellowEnemy(int i, int j, double checkpoint) { return null; }

        public override void ProcessPrimaryAction(bool state)
        {
            if (state)  //contracted muscle
            {
                if (GameScreen.GameCharacter.ActionMovement.Equals(ActionMovement.Right))
                {
                    ((GameCharacter)GameScreen.GameCharacter).StopLeftRightMovement();
                }
                else
                {
                    ((GameCharacter)GameScreen.GameCharacter).MoveLeft();
                }
            }
            else
            {
                ((GameCharacter)GameScreen.GameCharacter).StopLeftRightMovement();
            }
        }

        public override void ProcessSecondaryAction(bool state)
        {
            if (state)  //contracted muscle
            {
                if (GameScreen.GameCharacter.ActionMovement.Equals(ActionMovement.Left))
                {
                    ((GameCharacter)GameScreen.GameCharacter).StopLeftRightMovement();
                }
                else
                {
                    ((GameCharacter)GameScreen.GameCharacter).MoveRight();
                }
            }
            else
            {
                ((GameCharacter)GameScreen.GameCharacter).StopLeftRightMovement();
            }
        }
    }
}
