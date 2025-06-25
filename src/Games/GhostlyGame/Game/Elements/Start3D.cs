using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GhostlyLib.Elements
{
    public class Star3D : Drawable3D
    {
        #region Private members

        private Rectangle _rectangle;
        private int _width = 25, _height = 25;
        private LevelElements _elements;

        private BasicEffect _effect;

        #endregion Private members

        #region Public members

        public int Value { get; private set; }

        public override BasicEffect Effect { get { return _effect; } }

        #endregion Public members

        public Star3D(int x, int y, LevelElements elements, BasicEffect effect, GameScreen gameScreen) : base(gameScreen)
        {
            this.X = x * 40;
            this.Y = y * 40;
            this.Value = 1;
            this._elements = elements;
            this.IsVisible = true;

            this._rectangle = new Rectangle();

            this._effect = effect;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (this.IsVisible)
            {
                this.Effect.World = _3DCamera.World * Matrix.CreateTranslation(this.Xi, this.Yi, 0); // _world * Matrix.CreateTranslation(x, y, 0);
                this.Effect.View = _3DCamera.View; // _view;
                this.Effect.Projection = _3DCamera.Projection; // _projection;

                foreach (EffectPass pass in this.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    //_model,
                    ((GameScreen3D)GameScreen).GraphicsDevice.DrawUserPrimitives(
                        PrimitiveType.TriangleList,
                        this.Model,
                        0,
                        2
                    );
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            _rectangle = new Rectangle((int)this.X, (int)this.Y, this._width, this._height);

            //if the star is intersepted by the rocket, character gets the points and the star disappears
            if (this.IsVisible && _rectangle.Intersects(this.GameScreen.GameCharacter.Center))
            {
                GameScreen.GameCharacter.Score += this.Value;
                this.IsVisible = false;
                _elements.RemoveElement(this);
            }

            this.IsVisible = true;
        }
    }
}