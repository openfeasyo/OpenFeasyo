using GhostlyLib.Animations;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework;
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

        public SpaceCharacter(GameScreen gameScreen, LevelElements elements) : base(gameScreen)
        {
            this._elements = elements;

            this.Animation = ImagesAndAnimations.Instance.SpaceCharacterAnimation;

            this.SpeedX = GameScreen.SPEED;
            this.SpeedY = 0;

            this.Height = 112;
            this.Width = 152;

            this.CurrentHealth = 3;
            this.X = 100;
            this.Y = 300;

            this.HorizontalMovement = HorizontalMovement.MovingForward;
            this.VerticalMovement = VerticalMovement.None;

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

            //once the jumping is done (speed& == 0), gravity should apply
            if (this.SpeedY == 0 && this.Y < 300)
            {
                this.Y += GRAVITY;
            }
            else if (this.SpeedY == 0 && this.Y > 300)
            {
                this.Y -= GRAVITY;
            }           

            if (HorizontalMovement.Equals(HorizontalMovement.MovingForward))
            {
                this.SpeedX = GameScreen.SPEED;
            }
            else
            {
                this.SpeedX = 0;
            }

            MainBody = new Rectangle((int)this.X - 32, (int)this.Y - 71, 250, 190);
            TopBody = new Rectangle((int)this.X + 12, (int)this.Y + 1, 88, 20);     //in case of rocket this is "left side"
            BottomBody = new Rectangle((int)this.X + 12, (int)this.Y + 70, 88, 43); //in case of rocket this is "right side"
            //LeftSide = new Rectangle(this.X, this.TopBody.Y + 29, 11, 25);
            RightSide = new Rectangle((int)this.X + 112, this.TopBody.Y + 30, 45, 55); //in case of rocket this is front of the rocker

            Animation.Update(gameTime);

            CheckCollisions();
            GameScreen.GameBackground.HorizontalSpeed = HorizontalMovement == HorizontalMovement.Blocked ? 0 : -GameScreen.SPEED;
        }

        private void CheckCollisions()
        {
            IEnumerable<Drawable> tilesAround = this._elements.Tiles.Where(o => ((Tile)o).Rectangle.Intersects(this.MainBody));

            IEnumerable<Drawable> tilesAhead = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.RightSide));

            if (tilesAhead.Count() > 0)
            {
                if (((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.Checkpoint))
                {
                    //original X stores original position of the tile, at the start of the level, e.g. 50th tile from the left
                    //also, we save the checkpoint position 5 tiles before the actual checkpoint in the game
                    GameScreen.Checkpoint(((Tile)tilesAhead.ElementAt(0)).OriginalX - 5);
                }
                else
                {
                    //Debug.WriteLine("front collision " + this.X + " " + this.Y);
                    this.X = tilesAhead.ElementAt(0).X - this.Width + 5;
                    this.Blocked();

                    if (((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.Exit) || ((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.FinishLine))
                    {
                        GameScreen.LevelDone();
                    }
                }
            }
            else
            {
                //Debug.WriteLine("moving forward");
                this.HorizontalMovement = HorizontalMovement.MovingForward;
            }

            IEnumerable<Drawable> tilesAbove = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.TopBody));

            if (tilesAbove.Count() > 0)
            {
                if (!((Tile)tilesAbove.ElementAt(0)).TileType.Equals(TileType.Checkpoint))
                {
                    //Debug.WriteLine("left side collision " + this.X + " " + this.Y + " tile: " + tilesAbove.ElementAt(0).X + " " + tilesAbove.ElementAt(0).Y);
                    this.SpeedY = 0;
                    this.Y = tilesAbove.ElementAt(0).Y + ((Tile)tilesAbove.ElementAt(0)).Rectangle.Height + 1;
                }
            }

            IEnumerable<Drawable> tilesBelow = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.BottomBody)); 

            if (tilesBelow.Count() > 0)
            {
                if (!((Tile)tilesBelow.ElementAt(0)).TileType.Equals(TileType.Checkpoint))
                {
                    //Debug.WriteLine("right side collision " + this.X + " " + this.Y + " tile: " + tilesBelow.ElementAt(0).X + " " + tilesBelow.ElementAt(0).Y);
                    this.SpeedY = 0;
                    this.Y = tilesBelow.ElementAt(0).Y - this.Height;
                }
            }

            if (this.Y < 0)
            {
                //Debug.WriteLine("rocket going out of the top of the screen");
                this.Y = 0;
            }
            else if (this.Y > 700 - this.Height)
            {
                //Debug.WriteLine("rocket going out of the bottom of the screen");
                this.Y = 700 - this.Height;
            }
        }

        public override void Blocked()
        {
            this.HorizontalMovement = HorizontalMovement.Blocked;
            this.VerticalMovement = VerticalMovement.None;
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
            this.VerticalMovement = VerticalMovement.Left;
            this.SpeedY = SIDEMOVEMENTSPEED;
            this.SpeedX = GameScreen.SPEED;
            //}
        }

        public override void MoveRight()
        {
            //if (this.VerticalMovement.Equals(VerticalMovement.None))
            //{
            //GameScreen.MusicPlayer.PlayEffect("rocket_moving");
            this.VerticalMovement = VerticalMovement.Right;
            this.SpeedY = -SIDEMOVEMENTSPEED;
            this.SpeedX = GameScreen.SPEED;
            //}
        }

        public override void StopLeftRightMovement()
        {
            this.VerticalMovement = VerticalMovement.None;

            //KATKA: should this be here??
            //this.SpeedY = 0;
        }
    }
}
