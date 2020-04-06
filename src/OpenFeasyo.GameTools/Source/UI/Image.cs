/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
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
