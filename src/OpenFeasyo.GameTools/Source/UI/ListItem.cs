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
