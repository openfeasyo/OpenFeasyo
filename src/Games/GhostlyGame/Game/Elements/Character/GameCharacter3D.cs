using GhostlyLib.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Timers;


namespace GhostlyLib.Elements.Character
{
    public abstract class GameCharacter3D : Drawable3D, IGameCharacter
    {
        #region Private members
        private System.Timers.Timer _timer;

        private BasicEffect _standardEffect, _leftActionEffect, _rightActionEffect;
        #endregion Private members

        #region Public members
        public override BasicEffect Effect
        {
            get
            {
                if (RotationDirection == RotationDirection.Left) { return _leftActionEffect; }
                else if (RotationDirection == RotationDirection.Right) { return _rightActionEffect; }
                return _standardEffect;
            }
        }
        public int CurrentHealth { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Score { get; set; }
        public float OriginalRotation { get; set; }
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
        //public Movement Movement { get; set; }
        public RotationDirection RotationDirection { get; set; }
        public RotationStatus RotationStatus { get; set; }
        public CharacterLiveState LiveState { get; set; }
        public ActionMovement ActionMovement { get; set; }
        public AutomaticMovement AutomaticMovement { get; set; }
        public Instruction Instruction { get; set; }

        #endregion Public members

        protected GameCharacter3D(GameScreen gameScreen, BasicEffect standardEffect, BasicEffect leftActionEffect, BasicEffect rightActionEffect) : base(gameScreen)
        {
            this._standardEffect = standardEffect;
            this._leftActionEffect = leftActionEffect;
            this._rightActionEffect = rightActionEffect;

            this._timer = new System.Timers.Timer(1);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (this.IsVisible)
            {
                this.Effect.World = _3DCamera.World * Matrix.CreateRotationZ(this.OriginalRotation + this.CurrentRotation) * Matrix.CreateTranslation(this.Xi, this.Yi, 0); // _world * Matrix.CreateTranslation(x, y, 0);
                this.Effect.View = _3DCamera.View;// _view;
                this.Effect.Projection = _3DCamera.Projection; // _projection;

                foreach (EffectPass pass in this.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    ((GameScreen3D)GameScreen).GraphicsDevice.DrawUserPrimitives(
                        PrimitiveType.TriangleList,
                        this.Model,
                        0,
                        2
                    );
                }
            }
        }

        public void Hit()
        {
            this.CurrentHealth += -1;
            this.Score += -3;

            this.SpeedX = 0;
            this.LiveState = CharacterLiveState.Hit;
            //this.Animation.SetCurrentFrames(this.LiveState);

            this._timer = new System.Timers.Timer(1000);
            this._timer.Elapsed += Timer_Elapsed;
            this._timer.Start();
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.AutomaticMovement = AutomaticMovement.MovingForward;
            this._timer.Stop();
            this.LiveState = CharacterLiveState.Normal;
            //this.Animation.SetCurrentFrames(this.LiveState);
            this.SpeedX = GameScreen.SPEED;
        }

        public void Die()
        {
            this.SpeedX = 0;
            this.GameScreen.GameOver();
        }

        public abstract void Stop();

        public override void Update(GameTime gameTime) { }

        public abstract void TurnCounterClockwise();
        public abstract void TurnClockwise();
        public abstract void TurningInterupted();
    }
}