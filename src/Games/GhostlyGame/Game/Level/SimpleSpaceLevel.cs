using GhostlyGame.Models;
using GhostlyLib.Animations;
using GhostlyLib.DynamicDifficulty;
using GhostlyLib.Elements;
using GhostlyLib.Elements.Character;
using GhostlyLib.Elements.Enemies;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace GhostlyLib.Level
{
    public class SimpleSpaceLevel : SpaceLevel
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
        /*public Texture2D BluePlanet { get { return ImagesAndAnimations.Instance.BluePlanet; } }
        public Texture2D YellowPlanet { get { return ImagesAndAnimations.Instance.YellowPlanet; } }
        public Texture2D OrangePlanet { get { return ImagesAndAnimations.Instance.OrangePlanet; } }
        public Texture2D PinkPlanet { get { return ImagesAndAnimations.Instance.PinkPlanet; } }
        public Texture2D RedPlanet { get { return ImagesAndAnimations.Instance.RedPlanet; } }
        public Texture2D Star { get { return ImagesAndAnimations.Instance.Star; } }
        public Texture2D Ufo { get { return ImagesAndAnimations.Instance.Ufo; } }
        public Texture2D Debris { get { return ImagesAndAnimations.Instance.Debris; } }
        public Texture2D DebrisRocks { get { return ImagesAndAnimations.Instance.DebrisRocks; } }*/
        public override Texture2D ExitSign { get { return ImagesAndAnimations.Instance.ExitLine; } }
       /* public Texture2D SpaceSpiral { get { return ImagesAndAnimations.Instance.SpaceSpiral; } }
        public Texture2D SpaceMist { get { return ImagesAndAnimations.Instance.SpaceMist; } }*/

        #endregion Public members

        public SimpleSpaceLevel(GameScreen gameScreen, LevelElements elements) : base(gameScreen, elements)
        {
            this._elements = elements;
            this.Character = new SimpleSpaceCharacter(gameScreen, elements);
            this.Analytics = new SimpleSpaceLevelAnalytics();

            Debug.WriteLine("Difficulty Level: " + GameSessionInfo.Instance.SelectedPatient.DifficultyLevel);
        }

        public override Enemy CreateBlackEnemy(int i, int j, double checkpoint) { return null; }

        public override Enemy CreateGreenEnemy(int i, int j, double checkpoint) { return null; }

        public override Enemy CreateRedEnemy(int i, int j, double checkpoint) { return null; }

        public override Enemy CreateYellowEnemy(int i, int j, double checkpoint) { return null; }

        public override void ProcessPrimaryAction(bool state)
        {
            /*if (state)  //contracted muscle
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
            }*/
        }

        public override void ProcessSecondaryAction(bool state)
        {
           /* if (state)  //contracted muscle
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
            }*/
        }
    }
}
