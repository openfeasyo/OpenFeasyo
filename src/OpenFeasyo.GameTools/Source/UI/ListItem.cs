using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OpenFeasyo.GameTools.UI
{
    public abstract class ListItem : Component
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Note { get; }
        public abstract Texture2D Icon { get; }


        public void Draw(GameTime gameTime, SpriteBatch spritebatch, SpriteFont font)
        {
            base.Draw(gameTime, spritebatch);
            spritebatch.DrawString(font,Name,Position + new Vector2(10,10),Color.White);
        }




    }
}
