using GhostlyLib.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Elements
{
    public abstract class Drawable3D : Drawable
    {
        private VertexPositionTexture[] _model;

        //Xi and Yi are used to draw 3D, which should be scaled down by 100
        public float Xi
        {
            get
            {
                return (float)(X / 100);
            }
        }
        public float Yi
        {
            get
            {
                return (float)(Y / 100);
            }
        }

        public VertexPositionTexture[] Model { get { return _model; } }

        public abstract BasicEffect Effect { get; }

        protected Drawable3D(GameScreen gameScreen) : base(gameScreen)
        {
            // Define vertices of a square (two triangles)
            _model = new VertexPositionTexture[6];

            float size = 0.20f;

            _model[0] = new VertexPositionTexture(new Vector3(-size, size, 0), new Vector2(0, 0));
            _model[1] = new VertexPositionTexture(new Vector3(size, -size, 0), new Vector2(1, 1));
            _model[2] = new VertexPositionTexture(new Vector3(-size, -size, 0), new Vector2(0, 1));

            _model[3] = new VertexPositionTexture(new Vector3(-size, size, 0), new Vector2(0, 0));
            _model[4] = new VertexPositionTexture(new Vector3(size, size, 0), new Vector2(1, 0));
            _model[5] = new VertexPositionTexture(new Vector3(size, -size, 0), new Vector2(1, 1));
        }
    }
}
