using GhostlyLib.Animations;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Elements.Enemies
{
    public abstract class Enemy3D : Drawable3D, IEnemy
    {
        #region Private members
        private LevelElements _elements;
        #endregion Private members

        #region Public members

        public int CurrentHealth { get; protected set; }
        public Rectangle Rectangle { get; protected set; }

        public int Bonus { get; protected set; }
        public EnemyAnimation Animation { get; protected set; }

        public abstract int Height { get; }
        public abstract int Width { get; }
        public abstract EnemyState State { get; }

        public Rectangle Top { get; protected set; }
        public Rectangle Bottom { get; protected set; }
        public Rectangle Left { get; protected set; }
        public Rectangle Right { get; protected set; }

        public Rectangle MainBody { get; protected set; }

        public Direction Direction { get; set; }

        #endregion Public members

        public Enemy3D(int x, int y, LevelElements elements, GameScreen gameScreen) : base(gameScreen)
        {
            this.X = x * 40;
            this.Y = y * 40;

            this._elements = elements;
            this.IsVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            this.Rectangle = new Rectangle((int)X, (int)Y, this.Width, this.Height);

            if (this.Rectangle.Intersects(this.GameScreen.GameCharacter.MainBody))
            {
                CheckCollisionWithCharacter();
            }

            Animation.Update(8);

            this.IsVisible = true;

            MainBody = new Rectangle((int)this.X - 10, (int)this.Y - 10, 58, 58);
            Top = new Rectangle((int)this.X + 5, (int)this.Y - 5, 28, 5);
            Bottom = new Rectangle((int)this.X + 5, (int)this.Y + 36, 28, 5);
            Left = new Rectangle((int)this.X - 5, (int)this.Y + 5, 5, 28);
            Right = new Rectangle((int)this.X + 36, (int)this.Y + 5, 5, 28);

            CheckCollisionsAndMove();
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

        private void CheckCollisionWithCharacter()
        {
            if (!this.IsVisible) return;

            if (this.Rectangle.Intersects(GameScreen.GameCharacter.Center))
            {
                if (GameScreen.GameCharacter.CurrentHealth == 1)
                {
                    GameScreen.GameCharacter.Die();
                    GameScreen.MusicPlayer.PlayEffect("death");
                }
                else
                {
                    GameScreen.GameCharacter.Hit();
                    GameScreen.MusicPlayer.PlayEffect("ouch");
                }

                this.Die();
            }
        }

        private void CheckCollisionsAndMove()
        {
            IEnumerable<IDrawable> tilesAround = this._elements.Tiles.Where(o => ((Tile3D)o).Rectangle.Intersects(this.MainBody));

            IEnumerable<IDrawable> tilesAhead = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.Top));
            IEnumerable<IDrawable> tilesBehind = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.Bottom));
            IEnumerable<IDrawable> tilesOnLeft = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.Left));
            IEnumerable<IDrawable> tilesOnRight = tilesAround.Where(o => ((Tile3D)o).Rectangle.Intersects(this.Right));

            //check if there are no tiles ahead
            if (tilesAhead.Count() == 0 && this.Direction == Direction.North)
            {
                this.Y -= (0.4f * GameScreen.SPEED);
                return;
            }
            if (tilesBehind.Count() == 0 && this.Direction == Direction.South)
            {
                this.Y += (0.4f * GameScreen.SPEED);
                return;
            }
            if (tilesOnLeft.Count() == 0 && this.Direction == Direction.West)
            {
                this.X -= (0.4f * GameScreen.SPEED);
                return;
            }
            if (tilesOnRight.Count() == 0 && this.Direction == Direction.East)
            {
                this.X += (0.4f * GameScreen.SPEED);
                return;
            }

            //enemy cannot move forward anymore, it needs to turn
            //based on where it was headed we check which way is "open" and turn the enemy
            if (Direction.North == this.Direction || Direction.South == this.Direction)
            {
                if (tilesOnLeft.Count() == 0)
                {
                    this.Direction = Direction.West;
                }
                else if (tilesOnRight.Count() == 0)
                {
                    this.Direction = Direction.East;
                }
                else if (tilesAhead.Count() == 0)
                {
                    this.Direction = Direction.North;
                }
                else
                {
                    this.Direction = Direction.South;
                }
            }
            else if (Direction.East == this.Direction || Direction.West == this.Direction)
            {
                if (tilesAhead.Count() == 0)
                {
                    this.Direction = Direction.North;
                }
                else if (tilesBehind.Count() == 0)
                {
                    this.Direction = Direction.South;
                }
                else if (tilesOnLeft.Count() == 0)
                {
                    this.Direction = Direction.West;
                }
                else
                {
                    this.Direction = Direction.East;
                }
            }
        }
        public void Hit()
        {
            if (this.CurrentHealth > 0)
            {
                this.CurrentHealth -= 1;
                this.Animation.SetCurrentFrames(this.State);
                //AddOnetimeHitAnimation();
            }
        }

        public void Die()
        {
            this.CurrentHealth = -1;
            this.IsVisible = false;
            this._elements.RemoveElement(this);
        }

        /*private void AddOnetimeHitAnimation()
        {
            this.GameScreen.OnetimeAnimations.Add(new OnetimeAnimation((int)this.X + 10, (int)this.Y - 40, 40, 40, ImagesAndAnimations.Instance.PlusOneFrames, this.GameScreen));
        }*/
    }
}
