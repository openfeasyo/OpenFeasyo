using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenFeasyo.GameTools.UI
{
    public class Image : Component
    {
        public Texture2D Texture { get; set; }


        public Image(Texture2D texture) {
            Texture = texture;
            Size = new Vector2(Texture.Width, Texture.Height);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            base.Draw(gameTime, spritebatch);
            spritebatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color.White);
        }

    }
}
