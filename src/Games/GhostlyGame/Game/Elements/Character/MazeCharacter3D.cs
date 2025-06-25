using GhostlyLib.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Elements.Character
{
    public class MazeCharacter3D : GameCharacter3D
    {
        #region Private members
        private LevelElements _elements;
        //private Rectangle _rectangle;
        private float target_duration = 3.5f;
        #endregion Private members

        public MazeCharacter3D(GameScreen gameScreen, LevelElements elements, BasicEffect effect) : base(gameScreen, effect)
        {
            this._elements = elements;
            //this.Animation = ImagesAndAnimations.Instance.SpaceCharacterAnimation;

            this.SpeedX = GameScreen.SPEED;
            this.SpeedY = 0;

            this.Height = 38; // 37; // 112;
            this.Width = 38; //50; // 152;

            this.CurrentHealth = 3;
            this.X = 10; // 100;
            this.Y = 10; // 250;

            this.AutomaticMovement = AutomaticMovement.MovingForward;
            this.RotationStatus = RotationStatus.None;
            this.RotationDirection = RotationDirection.None;
            this.TurningDirection = TurningDirection.None;

            this.IsVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            // Update View matrix
            //_3DCamera.UpdateView(this.OriginalRotation + this.CurrentRotation, 0, this.Xi, this.Yi);

            if (this.AutomaticMovement == AutomaticMovement.Blocked)
            {
                // Update View matrix
                _3DCamera.UpdateView(this.OriginalRotation + this.CurrentRotation, -0.1f, this.Xi, this.Yi);
            }
            else
            {
                // Update View matrix
                _3DCamera.UpdateView(this.OriginalRotation + this.CurrentRotation, 0.1f, this.Xi, this.Yi);
            }

            //do nothing first 3 seconds
            if (gameTime.TotalGameTime.TotalMilliseconds < 3000)
                return;

            //if the character is rotating / muscles are contracted / increment target rotation
            if (this.RotationDirection == RotationDirection.Right)
            {
                float rot_step = (float)(((Math.PI / 2) / target_duration) / 1000);
                //this.CurrentRotation -= (0.0008f * gameTime.ElapsedGameTime.Milliseconds);
                this.CurrentRotation -= (rot_step * gameTime.ElapsedGameTime.Milliseconds);
            }
            else if (this.RotationDirection == RotationDirection.Left)
            {
                float rot_step = (float)(((Math.PI / 2) / target_duration) / 1000);
                //this.CurrentRotation += (0.0008f * gameTime.ElapsedGameTime.Milliseconds);
                this.CurrentRotation += (rot_step * gameTime.ElapsedGameTime.Milliseconds);
            }
            else if (this.RotationDirection == RotationDirection.None)
            {
                //rotation was interupted, the character needs to return to the original rotation
                if (this.RotationStatus == RotationStatus.Interupted)
                {
                    this.CurrentRotation = 0;
                }
                else if (this.RotationStatus == RotationStatus.Completed)
                {
                    //rotation was completed, so the OrientationRotation has to be updated
                    updateOrientationAndOriginalRotation();

                    //reinitialize rotation variables - target, current, and status
                    this.TargetRotation = 0;
                    this.CurrentRotation = 0;
                    this.RotationStatus = RotationStatus.None;
                    this.TurningDirection = TurningDirection.None;
                }
            }

            MainBody = new Rectangle((int)this.X - 10, (int)this.Y - 10, 58, 58);
            Top = new Rectangle((int)this.X + 5, (int)this.Y - 5, 28, 5);           // front of the rocket - red
            Bottom = new Rectangle((int)this.X + 5, (int)this.Y + 36, 28, 5);       // back of the rocket - blue
            LeftSide = new Rectangle((int)this.X - 5, (int)this.Y + 5, 5, 28);      // left side of the rocket - purple
            RightSide = new Rectangle((int)this.X + 36, (int)this.Y + 5, 5, 28);    // right side of the rocket - green
            Center = new Rectangle((int)this.X + 15, (int)this.Y + 15, 10, 10);     // center of the rocket - orange
            //Animation.Update(gameTime);

            CheckCollisionsAndMove();
            //CheckCollisionsWithEnemies();
        }

        private void CheckCollisionsAndMove()
        {
            IEnumerable<IDrawable> tilesAround = this._elements.Tiles.Where(o => ((Tile3D)o).Rectangle.Intersects(this.MainBody));

            IEnumerable<IDrawable> tilesAhead = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.Top));
            IEnumerable<IDrawable> tilesBehind = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.Bottom));
            IEnumerable<IDrawable> tilesOnLeft = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.LeftSide));
            IEnumerable<IDrawable> tilesOnRight = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.RightSide));
            IEnumerable<IDrawable> tilesCenter = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.Center));

            //reached Exit sign => level done
            if ((tilesAhead.Count() > 0 && tilesAhead.Any(o => ((Tile3D)o).TileType.Equals(TileType.Exit))) ||
                (tilesBehind.Count() > 0 && tilesBehind.Any(o => ((Tile3D)o).TileType.Equals(TileType.Exit))) ||
                (tilesOnLeft.Count() > 0 && tilesOnLeft.Any(o => ((Tile3D)o).TileType.Equals(TileType.Exit))) ||
                (tilesOnRight.Count() > 0 && tilesOnRight.Any(o => ((Tile3D)o).TileType.Equals(TileType.Exit))))
            /*if (tilesCenter.Count() > 0 && tilesCenter.Any(o => ((Tile)o).TileType.Equals(TileType.Exit)))*/
            {
                GameScreen.LevelDone();
            }

            if (this.Direction == Direction.North) //front of the rocket is turned up / "north"
            {
                if (tilesAhead.Count() == 0) // nothing lies ahead, can move forward
                {
                    this.AutomaticMovement = AutomaticMovement.MovingForward;
                    this.Y -= GameScreen.SPEED;
                    this.TurningDirection = TurningDirection.None;
                }
                else //is blocked. Which way can it go?
                {
                    this.AutomaticMovement = AutomaticMovement.Blocked;

                    if (tilesOnLeft.Count() == 0) // can go west
                    {
                        this.TurningDirection = TurningDirection.Left;
                    }
                    else if (tilesOnRight.Count() == 0)
                    {
                        this.TurningDirection = TurningDirection.Right;
                    }
                }
            }
            else if (this.Direction == Direction.South) // front of the rocket is turned down / "south"
            {
                if (tilesBehind.Count() == 0)   // nothing lies ahead, can move forward
                {
                    this.AutomaticMovement = AutomaticMovement.MovingForward;
                    this.Y += GameScreen.SPEED;
                    this.TurningDirection = TurningDirection.None;
                }
                else //is blocked, which way can it go?
                {
                    this.AutomaticMovement = AutomaticMovement.Blocked;

                    if (tilesOnLeft.Count() == 0)
                    {
                        this.TurningDirection = TurningDirection.Right;
                    }
                    else if (tilesOnRight.Count() == 0)
                    {
                        this.TurningDirection = TurningDirection.Left;
                    }
                }
            }
            else if (this.Direction == Direction.West)  // front of the rocket is turned left / "east"
            {
                if (tilesOnLeft.Count() == 0)
                {
                    this.AutomaticMovement = AutomaticMovement.MovingForward;
                    this.X -= GameScreen.SPEED;
                    this.TurningDirection = TurningDirection.None;
                }
                else // is blocked, which way can it go?
                {
                    this.AutomaticMovement = AutomaticMovement.Blocked;

                    if (tilesAhead.Count() == 0)
                    {
                        this.TurningDirection = TurningDirection.Right;
                    }
                    else if (tilesBehind.Count() == 0)
                    {
                        this.TurningDirection = TurningDirection.Left;
                    }
                }
            }
            else if (this.Direction == Direction.East) //front of the rocket is turned right / "west"
            {
                if (tilesOnRight.Count() == 0)
                {
                    this.AutomaticMovement = AutomaticMovement.MovingForward;
                    this.X += GameScreen.SPEED;
                    this.TurningDirection = TurningDirection.None;
                }
                else // is blocked, which way can it go?
                {
                    this.AutomaticMovement = AutomaticMovement.Blocked;

                    if (tilesAhead.Count() == 0)
                    {
                        this.TurningDirection = TurningDirection.Left;
                    }
                    else if (tilesBehind.Count() == 0)
                    {
                        this.TurningDirection = TurningDirection.Right; ;
                    }
                }
            }

            //checks that rocket doesn't go beyonf the top and bottom of the screen
            if (this.Y < 0)
            {
                this.Y = 0;
            }
            else if (this.Y > 700 - this.Height)
            {
                this.Y = 700 - this.Height;
            }
        }

        /*private void CheckCollisionsWithEnemies()
        {
            foreach (Enemy enemy in this._elements.Enemies)
            {
                if (this.IsVisible && enemy.IsVisible && this._rectangle.Intersects(enemy.Rectangle))
                {
                    if (enemy.CurrentHealth > 0)
                    {
                        enemy.Hit();
                        //GameScreen.MusicPlayer.PlayEffect(enemy.CurrentHealth == 0 ? "kill" : "hit");
                    }
                    if (enemy.CurrentHealth == 0)
                    {
                        this.GameScreen.GameCharacter.Score += enemy.Bonus;
                        enemy.Die();
                    }
                }
            }
        }*/

        public override void TurnCounterClockwise()
        {
            //if the rocket is flying forward, it cannot rotate
            if (this.AutomaticMovement == AutomaticMovement.MovingForward)
                return;

            if (this.RotationDirection == RotationDirection.None)
            {
                this.RotationDirection = RotationDirection.Left;
                this.TargetRotation = +((MathHelper.Pi * 2) / 4);
                this.RotationStatus = RotationStatus.Rotating;
            }
            else if (this.RotationDirection == RotationDirection.Right)
            {
                //TODO
                //if it is turning in other direction at the moment, the rotations should be interupted
                this.RotationStatus = RotationStatus.Interupted;
            }
        }

        public override void TurnClockwise()
        {
            //if the rocket is flying forward, it cannot rotate
            if (this.AutomaticMovement == AutomaticMovement.MovingForward)
                return;

            if (this.RotationDirection == RotationDirection.None)
            {
                this.RotationDirection = RotationDirection.Right;
                this.TargetRotation = -(MathHelper.Pi * 2) / 4;
                this.RotationStatus = RotationStatus.Rotating;
            }
            else if (this.RotationDirection == RotationDirection.Left)
            {
                //TODO
                //if it is turning in other dirrection at the moment, the rotations should be interupted
                this.RotationStatus = RotationStatus.Interupted;
            }
        }

        public override void TurningInterupted()
        {
            if (this.RotationDirection != RotationDirection.None)   //RotationDirection is left or right
            {
                //if completed
                if ((this.TargetRotation < 0 && this.CurrentRotation <= this.TargetRotation) ||
                        (this.TargetRotation > 0 && this.CurrentRotation >= this.TargetRotation))
                {
                    this.RotationStatus = RotationStatus.Completed;
                }
                else //interupted
                {
                    this.RotationStatus = RotationStatus.Interupted;
                }

                this.RotationDirection = RotationDirection.None;
            }
        }

        private void updateOrientationAndOriginalRotation()
        {
            var rot = (int)(this.CurrentRotation / ((MathHelper.Pi * 2) / 4));     // rot = number of quarter rotations done, carries + and -
            //normalzie, remove full circle rotations
            rot %= 4;

            // add all completed quarter rotations
            this.OriginalRotation = this.OriginalRotation + (rot * (MathHelper.Pi * 2) / 4);
            //normalize, remove all full circles
            this.OriginalRotation %= (MathHelper.Pi * 2);

            List<Direction> cardinalDirections = new List<Direction> { Direction.North, Direction.East, Direction.South, Direction.West };

            int currentIndex = cardinalDirections.IndexOf(this.Direction);
            int newIndex = (currentIndex + rot) % cardinalDirections.Count;

            //rotating counterClockWise throught the cardinal directions
            if (newIndex < 0) newIndex += 4;

            this.Direction = cardinalDirections[newIndex];
            //Debug.WriteLine("original rotation: " + OriginalRotation + " , current rotation: " + CurrentRotation + ", direction: " + Direction);
        }
    }
}