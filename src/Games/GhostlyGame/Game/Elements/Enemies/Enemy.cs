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
using GhostlyLib.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Elements.Enemies
{
    public abstract class Enemy : Drawable
    {
        private LevelElements _elements;

        #region Public members

        public int CurrentHealth { get; protected set; }
        public Rectangle Rectangle { get; protected set; }
        public override Texture2D Sprite { get { return Animation.GetImage(); } }
        public int Bonus { get; protected set; }
        public EnemyAnimation Animation { get; protected set; }
        // public override Size Size { get { return new Size(this.Width, this.Height); } }

        #endregion Public members

        public abstract int Height { get; }
        public abstract int Width { get; }

        public abstract EnemyState State { get; }

        public Enemy(int x, int y, LevelElements elements, GameScreen gameScreen) : base(gameScreen)
        {
            this.X = x * 40;
            this.Y = y * 40;

            this._elements = elements;
            this.IsVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            this.X += this.GameScreen.GameBackground.HorizontalSpeed;
            this.Rectangle = new Rectangle((int)X, (int)Y, this.Width, this.Height);

            if (this.Rectangle.Intersects(this.GameScreen.GameCharacter.YellowRed))
            {
                CheckCollision();
            }

            Animation.Update(8);

            if (this.X < this.GameScreen.GameCharacter.X - 400)
            {
                this.IsVisible = false;
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
                spriteBatch.Draw(this.Sprite, /*new Rectangle(*/GameScreen.Screen.ToScreen((int)this.X, (int)this.Y, this.Width, this.Height), Color.White);
            }
        }

        private void CheckCollision()
        {
            if (this.Rectangle.Intersects(GameScreen.GameCharacter.TopBody) || this.Rectangle.Intersects(GameScreen.GameCharacter.BottomBody)
                || this.Rectangle.Intersects(GameScreen.GameCharacter.LeftSide) || this.Rectangle.Intersects(GameScreen.GameCharacter.RightSide))
            {
                if (GameScreen.GameCharacter.CurrentHealth == 1)
                {
                    GameScreen.GameCharacter.Die();
                    GameScreen.MusicPlayer.PlayEffect("death");
                }
                else
                {
                    GameScreen.GameCharacter.Hit();
                    GameScreen.MusicPlayer.PlayEffect("ouch");
                }

                this.Die();
            }
        }

        public void Hit()
        {
            if (this.CurrentHealth > 0)
            {
                this.CurrentHealth -= 1;
                this.Animation.SetCurrentFrames(this.State);
                AddOnetimeHitAnimation();
            }
        }

        public void Die()
        {
            this.CurrentHealth = -1;
            this._elements.RemoveElement(this);
        }

        private void AddOnetimeHitAnimation()
        {
            this.GameScreen.OnetimeAnimations.Add(new OnetimeAnimation((int)this.X + 10, (int)this.Y - 40, 40, 40, ImagesAndAnimations.Instance.PlusOneFrames, this.GameScreen));
        }
    }
}