using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Elements
{
    public abstract class Drawable2D : Drawable
    {
        public abstract Texture2D Sprite { get; }

        protected Drawable2D(GameScreen gameScreen) : base(gameScreen)
        {
        }
    }
}