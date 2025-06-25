using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GhostlyLib.Elements
{
    public class Tile3D : Drawable3D, ITile
    {
        #region Private members

        private int _width = 50, _height = 50;
        private LevelElements _elements;
        private BasicEffect _effect;

        #endregion Private members

        #region Public members

        public Rectangle Rectangle { get; private set; }
        public override BasicEffect Effect { get { return _effect; } }
        public TileType TileType { get; private set; }

        #endregion Public members

        public Tile3D(int x, int y, int width, int height, TileType typeInt, LevelElements elements, BasicEffect effect, GameScreen gameScreen) : base(gameScreen)
        {
            this._width = width;
            this._height = height;
            this.X = x * 40;
            this.Y = y * 40;
            this.TileType = typeInt;
            this._elements = elements;

            this._effect = effect;

            this.IsVisible = true;

            this.Rectangle = new Rectangle((int)this.X, (int)this.Y, this._width, this._height);
        }

        public Tile3D(int x, int y, TileType typeInt, LevelElements elements, BasicEffect effect, GameScreen gameScreen) : this(x, y, 40, 40, typeInt, elements, effect, gameScreen) { }


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
            //tile positions do not change in the scope of this game
        }
    }
}