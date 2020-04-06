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
using GhostlyLib.Animations;
using GhostlyLib.Elements.Enemies;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Elements.Weapons
{
    public abstract class Weapon : Drawable
    {
        #region Private members

        private int _speedX, _height, _width;
        private Rectangle _rectangle;
        private InfiniteAnimation _animation;
        private LevelElements _elements;

        #endregion Private members

        #region Public mmebers

        public override Texture2D Sprite
        {
            get { return _animation.GetImage(); }
        }

        //public override Size Size { get { return new Size(this._width, this._height); }}
        #endregion Public mmebers

        public Weapon(int startX, int startY, int height, int width, InfiniteAnimation animation, LevelElements elements, GameScreen gameScreen) : base(gameScreen)
        {

            this.X = startX;
            this.Y = startY;
            this._animation = animation;
            this._elements = elements;
            this._height = height;
            this._width = width;

            this._speedX = 10;
            this._rectangle = new Rectangle(0, 0, 0, 0);

            this.IsVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            this.X += this._speedX;
            this._rectangle = new Rectangle((int)X, (int)Y, this._width, this._height);

            if (X > GameScreen.GameCharacter.X + 1000)
            {
                _elements.RemoveElement(this);
            }
            if (X < GameScreen.GameCharacter.X + 1000)
            {
                CheckCollision();
            }

            this._animation.Update(8);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.Sprite != null && this.IsVisible)
            {
                spriteBatch.Draw(this.Sprite, /*new Rectangle(*/GameScreen.Screen.ToScreen((int)this.X, (int)this.Y, this._width, this._height), Color.White);
            }
        }

        private void CheckCollision()
        {
            foreach (Enemy enemy in _elements.Enemies)
            {
                if (this.IsVisible && enemy.IsVisible && this._rectangle.Intersects(enemy.Rectangle))
                {
                    this.IsVisible = false;
                    _elements.RemoveElement(this);

                    if (enemy.CurrentHealth > 0)
                    {
                        this.GameScreen.GameCharacter.Score += 1;
                        enemy.Hit();

                        GameScreen.MusicPlayer.PlayEffect(enemy.CurrentHealth == 0  ? "kill": "hit");
                    }
                    if (enemy.CurrentHealth == 0)
                    {
                        this.GameScreen.GameCharacter.Score += enemy.Bonus;
                        enemy.Die();
                        
                    }
                }
            }

            foreach (Tile tile in _elements.Tiles)
            {
                if (this.IsVisible && tile.IsVisible && tile.TileType != TileType.InvisibleTile && tile.TileType != TileType.Exit && (this._rectangle.Intersects(tile.Rectangle)))
                {
                    this.IsVisible = false;
                    _elements.RemoveElement(this);
                }
            }
        }
    }
}
