/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Katarina Kostkoa
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using GhostlyLib.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Elements
{
    public class Tile : Drawable
    {
        #region Private members

        private int _width = 40, _height = 40;
        private Texture2D _image = null;
        private LevelElements _elements;

        #endregion Private members

        #region Public members

        public Rectangle Rectangle { get; private set; }
        public TileType TileType { get; private set; }
        public override Texture2D Sprite { get { return _image; } }
        //public override Size Size { get { return new Size(_width, _height); } }

        #endregion Public members

        public Tile(int x, int y, TileType typeInt, LevelElements elements, Texture2D image, GameScreen gameScreen) : base(gameScreen)
        {
            this.X = x * 40;
            this.Y = y * 40;
            this.TileType = typeInt;
            this._elements = elements;

            this.IsVisible = true;

            this.Rectangle = new Rectangle((int)this.X, (int)this.Y, this._width, this._height);
            this._image = image;

            switch (this.TileType)
            {
                case TileType.ExitSign:
                    this._height = 80;
                    this._width = 80;
                    break;
                case TileType.HillLarge:
                    this._height = 120;
                    break;
                case TileType.HillSmall:
                    this._height = 80;
                    break;
                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            this.X += this.GameScreen.GameBackground.HorizontalSpeed;
            this.Rectangle = new Rectangle((int)this.X, (int)this.Y, this._width, this._height);

            if (this.X < this.GameScreen.GameCharacter.X - 400)
            {
                this.IsVisible = false;
                _elements.RemoveElement(this);
            }
            else if (this.X > this.GameScreen.GameCharacter.X + 1300)
            {
                this.IsVisible = false;
            }
            else
            {
                this.IsVisible = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.Sprite != null && this.IsVisible)
            {
                spriteBatch.Draw(this.Sprite, /*new Rectangle(*/GameScreen.Screen.ToScreen( (int)this.X, (int)this.Y, this._width+1, this._height+1), Color.White);
            }
        }
    }
}
