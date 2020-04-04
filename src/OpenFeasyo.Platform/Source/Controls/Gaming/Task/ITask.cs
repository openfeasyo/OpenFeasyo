//extern alias mono;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Gaming.Task
{
    public interface ITask
    {
//        GameConfiguration GameConfiguration { get; set; }
        Vector3 Position { get; set; }
        Vector3 Rotation { get; set; }

        void Load(ContentManager content, GraphicsDevice device);

        void Unload();

        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);

        void Draw2D(GameTime gameTime, SpriteBatch spriteBatch);

        bool ConfigurationChanged { get; }

        void SaveChangedConfiguration();

    }
}
