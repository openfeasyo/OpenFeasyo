using GhostlyLib.Animations;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Elements.Character
{
    public class SpaceCharacter : GameCharacter
    {
        private const int SIDEMOVEMENTSPEED = -5;
        private const int MOVEMENTSLOWER = 1;
        private const int GRAVITY = 3;

        #region Private members
        private LevelElements _elements;
        #endregion Private members

        #region Public properties

        public override Texture2D Sprite
        {
            get
            {
                return this.Animation.GetImage();
            }
        }

        #endregion Public properties

        public SpaceCharacter(GameScreen gameScreen, LevelElements elements, double initialYPosition) : base(gameScreen)
        {
            this._elements = elements;

            this.Animation = ImagesAndAnimations.Instance.SpaceCharacterAnimation;

            this.SpeedX = GameScreen.SPEED;
            this.SpeedY = 0;

            this.Height = 112;
            this.Width = 152;

            this.CurrentHealth = 3;
            this.X = 100;
            this.Y = initialYPosition;// 300;

            this.AutomaticMovement = AutomaticMovement.MovingForward;
            this.ActionMovement = ActionMovement.None;

            this.Animation.SetCurrentFrames(CharacterLiveState.Normal);
            this.IsVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.SpeedY < 0)
            {
                this.SpeedY += MOVEMENTSLOWER;
            }
            else if (this.SpeedY > 0)
            {
                this.SpeedY -= MOVEMENTSLOWER;
            }

            this.Y += (int)this.SpeedY;

            //once the contraction triggered movement is done (speed == 0), gravity should apply
            if (this.SpeedY == 0 && this.Y < 300)
            {
                this.Y += GRAVITY;
            }
            else if (this.SpeedY == 0 && this.Y > 300)
            {
                this.Y -= GRAVITY;
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
            Top = new Rectangle((int)this.X + 12, (int)this.Y + 1, 88, 20);     //in case of rocket this is "left side"
            Bottom = new Rectangle((int)this.X + 12, (int)this.Y + 70, 88, 43); //in case of rocket this is "right side"
            //LeftSide = new Rectangle(this.X, this.TopBody.Y + 29, 11, 25);
            RightSide = new Rectangle((int)this.X + 112, this.Top.Y + 30, 45, 55); //in case of rocket this is front of the rocker

            Animation.Update(gameTime);

            CheckCollisions();
            GameScreen.GameBackground.HorizontalSpeed = AutomaticMovement == AutomaticMovement.Blocked ? 0 : -GameScreen.SPEED;
        }

        private void CheckCollisions()
        {
            IEnumerable<IDrawable> tilesAround = this._elements.Tiles.Where(o => ((Tile)o).Rectangle.Intersects(this.MainBody));

            IEnumerable<IDrawable> tilesAhead = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.RightSide));

            if (tilesAhead.Count() > 0)
            {
                if (((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.Checkpoint))
                {
                    //original X stores original position of the tile, at the start of the level, e.g. 50th tile from the left
                    //also, we save the checkpoint position 5 tiles before the actual checkpoint in the game
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
            else
            {
                this.AutomaticMovement = AutomaticMovement.MovingForward;
                this.Instruction = Instruction.Release;
            }

            IEnumerable<IDrawable> tilesAbove = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.Top));

            if (tilesAbove.Count() > 0)
            {
                if (!((Tile)tilesAbove.ElementAt(0)).TileType.Equals(TileType.Checkpoint))
                {
                    this.SpeedY = 0;
                    this.Y = ((Drawable)tilesAbove.ElementAt(0)).Y + ((Tile)tilesAbove.ElementAt(0)).Rectangle.Height + 1;
                }
            }

            IEnumerable<IDrawable> tilesBelow = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.Bottom));

            if (tilesBelow.Count() > 0)
            {
                if (!((Tile)tilesBelow.ElementAt(0)).TileType.Equals(TileType.Checkpoint))
                {
                    this.SpeedY = 0;
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
            this.ActionMovement = ActionMovement.None;
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
            this.ActionMovement = ActionMovement.Left;
            this.SpeedY = SIDEMOVEMENTSPEED;
            this.SpeedX = GameScreen.SPEED;
            //}
        }

        public override void MoveRight()
        {
            //if (this.VerticalMovement.Equals(VerticalMovement.None))
            //{
            //GameScreen.MusicPlayer.PlayEffect("rocket_moving");
            this.ActionMovement = ActionMovement.Right;
            this.SpeedY = -SIDEMOVEMENTSPEED;
            this.SpeedX = GameScreen.SPEED;
            //}
        }

        public override void StopLeftRightMovement()
        {
            this.ActionMovement = ActionMovement.None;

            //KATKA: should this be here??
            //this.SpeedY = 0;
        }
    }
}
