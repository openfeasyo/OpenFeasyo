using GhostlyGame.Models;
using GhostlyLib.DynamicDifficulty;
using GhostlyLib.Level;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Elements.Character
{
    public class MazeCharacter3D : GameCharacter3D
    {
        #region Private members
        private LevelElements _elements;
        private float default_target_duration = 3f;
        private int completedRotations = 0;
        //private bool turnInterupted = false;
        private bool firstPersonRotation = true;
        #endregion Private members

        protected float Target_Duration_Left
        {
            get
            {
                if (GameSessionInfo.Instance.SelectedPatient != null && GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh1Ms != null)
                    return (float)GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh1Ms / 1000;

                return default_target_duration;
            }
        }
        protected float Target_Duration_Right
        {
            get
            {
                if (GameSessionInfo.Instance.SelectedPatient != null && GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh2Ms != null)
                    return (float)GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh2Ms / 1000;

                return default_target_duration;
            }
        }

        protected float Rot_Step_Left
        {
            get { return (float)(((Math.PI / 2) / Target_Duration_Left) / 1000); }
        }

        protected float Rot_Step_Right
        {
            get { return (float)(((Math.PI / 2) / Target_Duration_Right) / 1000); }
        }

        public MazeCharacter3D(GameScreen gameScreen, LevelElements elements,
            BasicEffect standardEffect, BasicEffect leftActionEffect, BasicEffect rightActionEffect, bool firstPersonRotation = true) : base(gameScreen, standardEffect, leftActionEffect, rightActionEffect)
        {
            this._elements = elements;
            //this.Animation = ImagesAndAnimations.Instance.SpaceCharacterAnimation;

            this.SpeedX = GameScreen.SPEED;
            this.SpeedY = 0;

            this.Height = 38;
            this.Width = 38;

            this.CurrentHealth = 3;
            this.X = 10;
            this.Y = 10;

            this.AutomaticMovement = AutomaticMovement.MovingForward;
            this.RotationStatus = RotationStatus.None;
            this.RotationDirection = RotationDirection.None;
            this.TurningDirection = TurningDirection.None;
            this.Instruction = Instruction.None;

            this.IsVisible = true;

            this.firstPersonRotation = firstPersonRotation;
        }

        public override void Update(GameTime gameTime)
        {
            //do nothing first 3 seconds
            if (gameTime.TotalGameTime.TotalMilliseconds < 3000)
                return;

            //zoom in
            if (this.AutomaticMovement == AutomaticMovement.Blocked)
            {
                // Update View matrix
                if (firstPersonRotation)
                {
                    _3DCamera.UpdateView(this.OriginalRotation + this.CurrentRotation, -0.1f, this.Xi, this.Yi);
                }
                else
                {
                    _3DCamera.UpdateView(this.CameraOriginalRotation, -0.1f, this.Xi, this.Yi);
                }


                if (this.RotationStatus == RotationStatus.None)
                {
                    this.Instruction = Instruction.Contract;
                }
            }
            //zoom out
            else
            {
                // Update View matrix
                if (firstPersonRotation)
                {
                    _3DCamera.UpdateView(this.OriginalRotation + this.CurrentRotation, 0.1f, this.Xi, this.Yi);
                }
                else
                {
                    _3DCamera.UpdateView(this.CameraOriginalRotation, 0.1f, this.Xi, this.Yi);
                }
            }


            //if the character is rotating / muscles are contracted / increment current rotation
            if (this.RotationDirection == RotationDirection.Right)
            {
                this.CurrentRotation -= (Rot_Step_Right * gameTime.ElapsedGameTime.Milliseconds);
                this.CurrentRotation %= (MathHelper.Pi * 2);        //normalize

                if (TargetRotationReached())
                {
                    Instruction = Instruction.Release;
                }
                else
                {
                    Instruction = Instruction.Hold;
                }
            }
            else if (this.RotationDirection == RotationDirection.Left)
            {
                this.CurrentRotation += (Rot_Step_Left * gameTime.ElapsedGameTime.Milliseconds);
                this.CurrentRotation %= (MathHelper.Pi * 2);        //normalize 

                if (TargetRotationReached())
                {
                    Instruction = Instruction.Release;
                }
                else
                {
                    Instruction = Instruction.Hold;
                }
            }
            else if (this.RotationDirection == RotationDirection.None)
            {
                //rotation was interupted, the character needs to return to the original rotation
                if (this.RotationStatus == RotationStatus.Interupted)
                {
                    this.CurrentRotation = 0;

                    if (this.AutomaticMovement == AutomaticMovement.Blocked)
                    {
                        this.Instruction = Instruction.Contract;
                    }
                }
                else if (this.RotationStatus == RotationStatus.Completed)
                {
                    //rotation was completed, so the OrientationRotation has to be updated
                    UpdateOrientationAndOriginalRotation();

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

            CheckCollisionsAndMove();
        }

        private void CheckCollisionsAndMove()
        {
            IEnumerable<IDrawable> tilesAround = this._elements.Tiles.Where(o => ((Tile3D)o).Rectangle.Intersects(this.MainBody));

            IEnumerable<IDrawable> tilesAhead = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.Top) && (!((Tile3D)o).TileType.Equals(TileType.LeftRotation)) && (!((Tile3D)o).TileType.Equals(TileType.RightRotation)));
            IEnumerable<IDrawable> tilesBehind = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.Bottom) && (!((Tile3D)o).TileType.Equals(TileType.LeftRotation)) && (!((Tile3D)o).TileType.Equals(TileType.RightRotation)));
            IEnumerable<IDrawable> tilesOnLeft = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.LeftSide) && (!((Tile3D)o).TileType.Equals(TileType.LeftRotation)) && (!((Tile3D)o).TileType.Equals(TileType.RightRotation)));
            IEnumerable<IDrawable> tilesOnRight = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.RightSide) && (!((Tile3D)o).TileType.Equals(TileType.LeftRotation)) && (!((Tile3D)o).TileType.Equals(TileType.RightRotation)));
            IEnumerable<IDrawable> tilesCenter = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.Center)); // && (!((Tile3D)o).TileType.Equals(TileType.LeftRotation)) && (!((Tile3D)o).TileType.Equals(TileType.RightRotation)));

            //reached Exit sign => level done
            if ((tilesAhead.Count() > 0 && tilesAhead.Any(o => ((Tile3D)o).TileType.Equals(TileType.Exit))) ||
                (tilesBehind.Count() > 0 && tilesBehind.Any(o => ((Tile3D)o).TileType.Equals(TileType.Exit))) ||
                (tilesOnLeft.Count() > 0 && tilesOnLeft.Any(o => ((Tile3D)o).TileType.Equals(TileType.Exit))) ||
                (tilesOnRight.Count() > 0 && tilesOnRight.Any(o => ((Tile3D)o).TileType.Equals(TileType.Exit))))
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
                    UpdateTargetRotationAndTurningDirection(tilesCenter, tilesOnLeft, tilesOnRight, tilesBehind);
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
                    UpdateTargetRotationAndTurningDirection(tilesCenter, tilesOnRight, tilesOnLeft, tilesAhead);
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
                    UpdateTargetRotationAndTurningDirection(tilesCenter, tilesBehind, tilesAhead, tilesOnRight);
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
                    UpdateTargetRotationAndTurningDirection(tilesCenter, tilesAhead, tilesBehind, tilesOnLeft);
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

        private bool TargetRotationReached()
        {
            //Debug.WriteLine("currrentRot " + this.CurrentRotation + " targetRot " + this.TargetRotation);
            if (this.TargetRotation < 0) // target is negative, e.g. -90dgrs, -1.57rad
            {
                //if current rotation exceeds the target rotation but does not exceed it by more than 90 dgr
                if (this.CurrentRotation <= this.TargetRotation && this.CurrentRotation > this.TargetRotation - (Math.PI * 2) / 4)        // e.g., -1.8 <= -1.57 (-Pi/2 AKA -90 dgrs)
                {
                    return true;
                }
                else if (this.CurrentRotation >= (-3 * this.TargetRotation))    // e.g., 4.9 >= (-3 * -1.57), when instead of turning -90dgrs, turning +270 dgrs
                {
                    return true;
                }
                return false;

            }
            else if (this.TargetRotation > 0)  // target rotation is positive, e.g. 90dgrs, 1.57rad
            {
                if (this.CurrentRotation >= this.TargetRotation && this.CurrentRotation < this.TargetRotation + (Math.PI * 2) / 4)        // e.g., 1.8 >= 1.57 (PI/2 AKA 90 dgrs)
                {
                    return true;
                }
                else if (this.CurrentRotation <= (-3 * this.TargetRotation))      //e.g., -4.9 <= (-3 * 1.57), when instead of turning 90 dgrs, turning -270 dgrs
                {
                    return true;
                }
                return false;
            }

            return false;
        }

        private void UpdateTargetRotationAndTurningDirection(IEnumerable<IDrawable> tilesCenter, IEnumerable<IDrawable> tilesOnLeft, IEnumerable<IDrawable> tilesOnRight, IEnumerable<IDrawable> tilesBehind)
        {
            //Which way to ratate, based on what was indicated in the maze map, i.e. 'L' or 'R'
            if (tilesCenter.Any(o => ((Tile3D)o).TileType.Equals(TileType.LeftRotation)))
            {
                this.TurningDirection = TurningDirection.Left;

                //Target rotation, rotate by +90, -90, (or +180, -180 degrees when previously went backwards)
                if (tilesOnRight.Count() == 0)
                {
                    this.TargetRotation = +(MathHelper.Pi * 2) / 4;
                }
                else if (tilesBehind.Count() == 0)
                {
                    this.TargetRotation = +(MathHelper.Pi * 2) / 2;
                }
            }
            else if (tilesCenter.Any(o => ((Tile3D)o).TileType.Equals(TileType.RightRotation)))
            {
                this.TurningDirection = TurningDirection.Right;

                //Target rotation, rotate by +90, -90, (or +180, -180 degrees when previously went backwards)
                if (tilesOnLeft.Count() == 0)
                {
                    this.TargetRotation = -((MathHelper.Pi * 2) / 4);
                }
                else if (tilesBehind.Count() == 0)
                {
                    this.TargetRotation = -((MathHelper.Pi * 2) / 2);
                }
            }
        }

        public TileType StandingOn()
        {
            IEnumerable<IDrawable> tilesAround = this._elements.Tiles.Where(o => ((Tile3D)o).Rectangle.Intersects(this.MainBody));
            IEnumerable<IDrawable> tilesCenter = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.Center));

            if (tilesCenter.Count() > 0)
            {
                return ((Tile3D)tilesCenter.First<IDrawable>()).TileType;
            }

            return TileType.Dirt;
        }

        public override void Stop() { }

        public override void TurnCounterClockwise()
        {
            //if the rocket is flying forward, it cannot rotate
            if (this.AutomaticMovement == AutomaticMovement.MovingForward)
                return;

            if (this.RotationDirection == RotationDirection.None)
            {
                this.RotationDirection = RotationDirection.Left;
                this.RotationStatus = RotationStatus.Rotating;

                this.Instruction = Instruction.Hold;

                if (GameScreen.Level.GetType() == typeof(MazeLevel3D))
                {
                    ((Maze3DLevelAnalytics)((MazeLevel3D)GameScreen.Level).Analytics).UpdateTurn(completedRotations, 1, TileTypeToRotationDirection(this.StandingOn()), RotationDirection.Left);
                }
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
                this.RotationStatus = RotationStatus.Rotating;

                this.Instruction = Instruction.Hold;

                if (GameScreen.Level.GetType() == typeof(MazeLevel3D))
                {
                    ((Maze3DLevelAnalytics)((MazeLevel3D)GameScreen.Level).Analytics).UpdateTurn(completedRotations, 1, TileTypeToRotationDirection(this.StandingOn()), RotationDirection.Right);
                }
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
                if (TargetRotationReached())            //rotation completed
                {
                    this.RotationStatus = RotationStatus.Completed;
                    this.completedRotations++;
                }
                else //interupted
                {
                    this.RotationStatus = RotationStatus.Interupted;
                }

                this.RotationDirection = RotationDirection.None;
            }
        }

        private void UpdateOrientationAndOriginalRotation()
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
        }

        private RotationDirection TileTypeToRotationDirection(TileType tiletype)
        {
            if (tiletype == TileType.LeftRotation)
                return RotationDirection.Left;
            else if (tiletype == TileType.RightRotation)
                return RotationDirection.Right;

            return RotationDirection.None;
        }
    }
}