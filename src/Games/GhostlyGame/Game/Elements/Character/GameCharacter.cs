/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Katarina Kostkoa
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using GhostlyLib.Animations;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Timers;

namespace GhostlyLib.Elements.Character
{
    public abstract class GameCharacter : Drawable2D, IGameCharacter
    {
        private double _speedY = 0;
        private System.Timers.Timer _timer;

        protected CharacterAnimation Animation { get; set; }
        //public int CurrentHealth { get; protected set; }
        //public int Height { get; protected set; }
        //public int Width { get; protected set; }
        //public int Score = 0;

        //public double SpeedX { get; protected set; }
        /*public double SpeedY
        {
            get { return _speedY; }
            set { _speedY = value; *//*Debug.WriteLine("character speedY = " + value);*//* }
        }
*/
        //public Rectangle TopBody { get; protected set; }
        //public Rectangle BottomBody { get; protected set; }
        //public Rectangle LeftSide { get; protected set; }
        //public Rectangle RightSide { get; protected set; }
        //public Rectangle MainBody { get; protected set; }
        /*public VerticalMovement VerticalMovement { get; protected set; }
        public HorizontalMovement HorizontalMovement { get; protected set; }
        *///public CharacterLiveState LiveState { get; protected set; }

        public int CurrentHealth { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Score { get; set; }
        public float OriginalRotation { get; set; }
        public float CameraOriginalRotation { get; set; }
        public float TargetRotation { get; set; }
        public float CurrentRotation { get; set; }
        public Direction Direction { get; set; }
        public TurningDirection TurningDirection { get; set; }
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }
        public Rectangle Top { get; set; }
        public Rectangle Bottom { get; set; }
        public Rectangle LeftSide { get; set; }
        public Rectangle RightSide { get; set; }
        public Rectangle MainBody { get; set; }
        public Rectangle Center { get; set; }
        public RotationDirection RotationDirection { get; set; }
        public RotationStatus RotationStatus { get; set; }
        public CharacterLiveState LiveState { get; set; }
        public ActionMovement ActionMovement { get; set; }
        public AutomaticMovement AutomaticMovement { get; set; }
        public Instruction Instruction { get; set; }

        public override Texture2D Sprite { get; } 

        protected GameCharacter(GameScreen gameScreen) : base(gameScreen) { }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (this.Sprite != null && this.IsVisible)
            {
                spriteBatch.Draw(this.Sprite, /*new Rectangle(*/GameScreen.Screen.ToScreen((int)this.X, (int)this.Y, this.Width, this.Height), Color.White);

#if DEBUG
                //Debug.WriteLine("screenrec: " + GameScreen.Screen.ToScreen((int)this.X, (int)this.Y, this.Width, this.Height));
                //draw the bounds for collision detection
                Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                pixel.SetData(new[] { Color.White });
                spriteBatch.Draw(pixel, GameScreen.Screen.ToScreen((int)this.Top.X, (int)this.Top.Y, this.Top.Width, this.Top.Height), Color.Red);
                spriteBatch.Draw(pixel, GameScreen.Screen.ToScreen((int)this.Bottom.X, (int)this.Bottom.Y, this.Bottom.Width, this.Bottom.Height), Color.Blue);
                spriteBatch.Draw(pixel, GameScreen.Screen.ToScreen((int)this.LeftSide.X, (int)this.LeftSide.Y, this.LeftSide.Width, this.LeftSide.Height), Color.Purple);
                spriteBatch.Draw(pixel, GameScreen.Screen.ToScreen((int)this.RightSide.X, (int) this.RightSide.Y, this.RightSide.Width, this.RightSide.Height), Color.Green);
                spriteBatch.Draw(pixel, GameScreen.Screen.ToScreen((int)this.Center.X, (int) this.Center.Y, this.Center.Width, this.Center.Height), Color.Orange);
#endif
            }
        }
        public override void Update(GameTime gameTime) { }

        public void Hit()
        {
            this.CurrentHealth += -1;
            this.Score += -3;

            this.SpeedX = 0;
            this.LiveState = CharacterLiveState.Hit;
            this.Animation.SetCurrentFrames(this.LiveState);

            this._timer = new System.Timers.Timer(1000);
            this._timer.Elapsed += Timer_Elapsed;
            this._timer.Start();

            AddOneTimeAnimation();
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.AutomaticMovement = AutomaticMovement.MovingForward;
            this._timer.Stop();
            this.LiveState = CharacterLiveState.Normal;
            this.Animation.SetCurrentFrames(this.LiveState);
            this.SpeedX = GameScreen.SPEED;
        }

        public void Die()
        {
            this.SpeedX = 0;
            this.GameScreen.GameOver();
        }

        protected void AddOneTimeAnimation()
        {
            this.GameScreen.OnetimeAnimations.Add(new OnetimeAnimation((int)this.X + 10, (int)this.Y - 40, 40, 40, ImagesAndAnimations.Instance.MinusThreeFrames, this.GameScreen));
        }

        public abstract void BreakLongJump();
        public abstract void Jump();
        public abstract void LongJump();
        public abstract void Shoot();
        public abstract void Stop();
        public abstract void Blocked();
        public abstract void Standing();
        public abstract void Falling();
        public abstract void SlidingOnIce();
        public abstract void Swimming();
        public abstract void MoveLeft();
        public abstract void MoveRight();
        public abstract void StopLeftRightMovement();
    }
}