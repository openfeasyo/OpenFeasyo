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
	public class Label : Component
	{
		private SpriteFont _font;
		private string _text;
		private Color _color;

		public string Text
		{
			get { return _text; }
			set {
				_text = value;
				this.Size = _font.MeasureString(_text);
			}
		}

		public Label(string text, SpriteFont font,Color color) : base()
		{
			_text = text;
			_font = font;
			_color = color;
			this.Size = _font.MeasureString(text);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
		{
			spritebatch.DrawString(_font,_text,Position,_color);
		}


	}
}