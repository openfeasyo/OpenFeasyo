using GhostlyGame.Models;
using GhostlyLib.Animations;
using GhostlyLib.DynamicDifficulty;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Elements.Character
{
    public class SimpleSpaceCharacter : GameCharacter
    {
        //private const int SIDEMOVEMENTSPEED = -5; //-2 leads to 10s contractions
        //private const int MOVEMENTSLOWER = 1;
        //private const int GRAVITY = 3;

        #region Private members
        private LevelElements _elements;

        private Pull GravityPull = Pull.No;

        private int default_contraction_duration_left = 3; //10;//6;//3;
        private int default_contraction_duration_right = 3; //10;//6;//3;

        private float averageObstacleHeight = 520f;    //obstacles are 560, 520, or 480px tall

        private int completedMovements = 0;
        private ActionMovement desiredMovement;

        #endregion Private members

        #region Protected members
        protected float UpDown_Movement_Step_Left
        {
            get
            {
                if (GameSessionInfo.Instance.SelectedPatient != null && GameSessionInfo.Instance.SelectedPatient.DifficultyLevel != null)
                {
                    int contractionDuration = DifficultyLevelStateSpace.Instance.getLevelDefinition((int)GameSessionInfo.Instance.SelectedPatient.DifficultyLevel).contractionDuration;
                    return (float)(averageObstacleHeight / contractionDuration) / 1000;
                }

                return (float)((averageObstacleHeight / default_contraction_duration_left) / 1000);

            }
        }

        protected float UpDown_Movement_Step_Right
        {
            get
            {
                if (GameSessionInfo.Instance.SelectedPatient != null && GameSessionInfo.Instance.SelectedPatient.DifficultyLevel != null)
                {
                    int contractionDuration = DifficultyLevelStateSpace.Instance.getLevelDefinition((int)GameSessionInfo.Instance.SelectedPatient.DifficultyLevel).contractionDuration;
                    return (float)(averageObstacleHeight / contractionDuration) / 1000;
                }

                return (float)((averageObstacleHeight / default_contraction_duration_right) / 1000);
            }
        }
        #endregion

        #region Public properties

        public override Texture2D Sprite
        {
            get
            {
                return this.Animation.GetImage();
            }
        }

        #endregion Public properties

        public SimpleSpaceCharacter(GameScreen gameScreen, LevelElements elements) : base(gameScreen)
        {
            this._elements = elements;

            this.Animation = ImagesAndAnimations.Instance.SpaceCharacterAnimation;

            this.SpeedX = GameScreen.SPEED;
            this.SpeedY = 0;

            this.Height = 112;
            this.Width = 152;

            this.CurrentHealth = 3;
            this.X = 150;
            this.Y = 300;

            this.AutomaticMovement = AutomaticMovement.MovingForward;
            this.ActionMovement = ActionMovement.None;

            this.Animation.SetCurrentFrames(CharacterLiveState.Normal);
            this.IsVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.ActionMovement == ActionMovement.Left)
            {
                this.SpeedY = -(UpDown_Movement_Step_Left * gameTime.ElapsedGameTime.Milliseconds);
            }
            else if (this.ActionMovement == ActionMovement.Right)
            {
                this.SpeedY = (UpDown_Movement_Step_Right * gameTime.ElapsedGameTime.Milliseconds);
            }
            else
            {
                this.SpeedY = 0;
            }
            this.Y += this.SpeedY;

            //if player should contract but is not, pulling to opposite direction should apply
            if (this.ActionMovement == ActionMovement.None && GravityPull.Equals(Pull.Down))
            {
                this.Y += (UpDown_Movement_Step_Left * gameTime.ElapsedGameTime.Milliseconds);
            }
            else if (this.ActionMovement == ActionMovement.None && GravityPull.Equals(Pull.Up))
            {
                this.Y -= (UpDown_Movement_Step_Right * gameTime.ElapsedGameTime.Milliseconds);
            }

            if (AutomaticMovement.Equals(AutomaticMovement.MovingForward))
            {
                this.SpeedX = GameScreen.SPEED;
            }
            else
            {
                this.SpeedX = 0;
            }

            MainBody = new Rectangle((int)this.X - 32, (int)this.Y - 71, 250, 190);
            Top = new Rectangle((int)this.X + 12, (int)this.Y + 1, 128, 35);        //in case of rocket this is "left side"
            Bottom = new Rectangle((int)this.X + 12, (int)this.Y + 90, 128, 25);    //in case of rocket this is "right side"
            RightSide = new Rectangle((int)this.X + 142, this.Top.Y + 19, 11, 72);  //in case of rocket this is front of the rocker
            Center = new Rectangle((int)this.X + 71, (int)this.Y + 50, 10, 10);     //center of the rocket

            Animation.Update(gameTime);

            CheckCollisions();
            GameScreen.GameBackground.HorizontalSpeed = AutomaticMovement == AutomaticMovement.Blocked ? 0 : -GameScreen.SPEED;
        }

        private void CheckCollisions()
        {
            IEnumerable<IDrawable> tilesAround = this._elements.Tiles.Where(o => ((Tile)o).Rectangle.Intersects(this.MainBody));
            IEnumerable<IDrawable> tilesAhead = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.RightSide));
            IEnumerable<IDrawable> tilesCenter = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.Center));

            //tiles in front of the ship
            if (tilesAhead.Count() > 0)
            {
                //BUT we exited the pull up / pull down section => release muscles
                if (!((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.PullUp) && !((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.PullDown))
                {
                    this.Instruction = Instruction.Release;
                    GravityPull = Pull.No;
                    desiredMovement = ActionMovement.None;
                }
                //they are pulling tiles
                else
                {
                    //they are intersecting with the center of the ship
                    if (tilesCenter.Count() > 0)
                    {
                        //center of the ship is in the pull up section
                        if (((Tile)tilesCenter.ElementAt(0)).TileType.Equals(TileType.PullUp))
                        {
                            GravityPull = Pull.Up;
                            desiredMovement = ActionMovement.Right;

                            if (this.SpeedY == 0)
                            {
                                this.Instruction = Instruction.Contract;
                            }
                            else
                            {
                                this.Instruction = Instruction.Hold;
                            }
                        }
                        //center of the ship is in the pull down section
                        else if (((Tile)tilesCenter.ElementAt(0)).TileType.Equals(TileType.PullDown))
                        {
                            GravityPull = Pull.Down;
                            desiredMovement = ActionMovement.Left;

                            if (this.SpeedY == 0)
                            {
                                this.Instruction = Instruction.Contract;
                            }
                            else
                            {
                                this.Instruction = Instruction.Hold;
                            }
                        }
                    }
                    //not yet intersecting with the center of the ship
                    else
                    {
                        if (GravityPull != Pull.No && this.ActionMovement == ActionMovement.None)
                        {
                            //completed required movement
                            completedMovements++;
                            GravityPull = Pull.No;
                            desiredMovement = ActionMovement.None;
                            this.Instruction = Instruction.Release;
                        }
                    }
                }
            }
            //no tiles ahead
            else
            {
                this.Instruction = Instruction.Release;
                GravityPull = Pull.No;
                desiredMovement = ActionMovement.None;
                //Debug.WriteLine("Nothing ahead -> release");
            }

            //remove up down pulling tiles for further processing
            tilesAhead = tilesAhead.Where(o => !((Tile)o).TileType.Equals(TileType.PullUp)
                && !((Tile)o).TileType.Equals(TileType.PullDown));

            if (tilesAhead.Count() > 0)
            {
                if (((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.Checkpoint))
                {
                    //original X stores original position of the tile, at the start of the level, e.g. 50th tile from the left
                    //we save the checkpoint position 5 tiles before the actual checkpoint in the game
                    GameScreen.SetCheckpoint(((Tile)tilesAhead.ElementAt(0)).OriginalX - 5);
                }
                else
                {
                    this.X = ((Drawable)tilesAhead.ElementAt(0)).X - this.Width + 5;
                    this.Blocked();

                    if (((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.Exit) || ((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.FinishLine))
                    {
                        GameScreen.LevelDone();
                    }
                }
            }
            else //no tiles ahead
            {
                this.AutomaticMovement = AutomaticMovement.MovingForward;
            }

            IEnumerable<IDrawable> tilesAbove = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.Top)
                && !((Tile)o).TileType.Equals(TileType.PullUp)
                && !((Tile)o).TileType.Equals(TileType.PullDown));

            if (tilesAbove.Count() > 0)
            {
                if (!((Tile)tilesAbove.ElementAt(0)).TileType.Equals(TileType.Checkpoint))
                {
                    this.Y = ((Drawable)tilesAbove.ElementAt(0)).Y + ((Tile)tilesAbove.ElementAt(0)).Rectangle.Height + 1;
                }
            }

            IEnumerable<IDrawable> tilesBelow = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.Bottom)
                && !((Tile)o).TileType.Equals(TileType.PullUp)
                && !((Tile)o).TileType.Equals(TileType.PullDown));

            if (tilesBelow.Count() > 0)
            {
                if (!((Tile)tilesBelow.ElementAt(0)).TileType.Equals(TileType.Checkpoint))
                {
                    this.Y = ((Drawable)tilesBelow.ElementAt(0)).Y - this.Height;
                }
            }

            if (this.Y < 0)
            {
                this.Y = 0;
            }
            else if (this.Y > 700 - this.Height)
            {
                this.Y = 700 - this.Height;
            }
        }

        public override void Blocked()
        {
            this.AutomaticMovement = AutomaticMovement.Blocked;

            if (this.SpeedY == 0)
            {
                this.Instruction = Instruction.Contract;
            }
            else
            {
                this.Instruction = Instruction.Hold;
            }
        }

        public override void BreakLongJump()
        {
            throw new NotImplementedException();
        }

        public override void Falling()
        {
            throw new NotImplementedException();
        }

        public override void Jump()
        {
            throw new NotImplementedException();
        }

        public override void LongJump()
        {
            throw new NotImplementedException();
        }

        public override void Shoot()
        {
            throw new NotImplementedException();
        }

        public override void SlidingOnIce()
        {
            throw new NotImplementedException();
        }

        public override void Standing()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            this.SpeedX = 0;
        }

        public override void Swimming()
        {
            throw new NotImplementedException();
        }

        public override void MoveLeft()
        {
            //if (this.VerticalMovement.Equals(VerticalMovement.None))
            //{
            //GameScreen.MusicPlayer.PlayEffect("rocket_moving");
            if (this.ActionMovement != ActionMovement.Left)
            {
                this.ActionMovement = ActionMovement.Left;
                this.SpeedX = GameScreen.SPEED;

                //((SimpleSpaceLevelAnalytics)((SimpleSpaceLevel)GameScreen.Level).Analytics).UpdateMovement(completedMovements, 1, desiredMovement, this.ActionMovement, DateTime.Now.Ticks);
            }//}
        }

        public override void MoveRight()
        {
            //if (this.VerticalMovement.Equals(VerticalMovement.None))
            //{
            //GameScreen.MusicPlayer.PlayEffect("rocket_moving");
            if (this.ActionMovement != ActionMovement.Right)
            {
                this.ActionMovement = ActionMovement.Right;
                this.SpeedX = GameScreen.SPEED;

                //((SimpleSpaceLevelAnalytics)((SimpleSpaceLevel)GameScreen.Level).Analytics).UpdateMovement(completedMovements, 1, desiredMovement, this.ActionMovement, DateTime.Now.Ticks);
            }
            //}
        }

        public override void StopLeftRightMovement()
        {
            this.ActionMovement = ActionMovement.None;
        }
    }
}
