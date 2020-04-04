using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenFeasyo.GameTools
{
    public interface ILevel
    {
        bool Finished { get; }
        int LevelId { get; set; }
        int Score { get; set; }

        void Draw(GameTime gameTime, SpriteBatch spritebatch);
        void Update(GameTime gameTime);
    }
}
