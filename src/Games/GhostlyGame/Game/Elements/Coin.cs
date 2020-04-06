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

namespace GhostlyLib.Elements
{
    public class Coin : Drawable
    {
        #region Private members

        private Rectangle _rectangle;
        private int _width = 35, _height = 35;
        private LevelElements _elements;
        private InfiniteAnimation _animation;

        #endregion Private members

        #region Public members

        public int Value { get; private set; }
        public override Texture2D Sprite { get { return this._animation.GetImage(); } }
        //public override Size Size { get { return new Size(this._width, this._height); } }

        #endregion Public members

        public Coin(int x, int y, int value, LevelElements elements, GameScreen gameScreen) : base(gameScreen)
        {
            this.X = x * 40;
            this.Y = y * 40;
            this.Value = value;
            this._elements = elements;
            this.IsVisible = true;

            this._rectangle = new Rectangle();

            switch (value)
            {
                case 1:
                    this._animation = ImagesAndAnimations.Instance.BronzeCoinAnimation;
                    break;
                case 2:
                    this._animation = ImagesAndAnimations.Instance.SilverCoinAnimation;
                    break;
                case 3:
                    this._animation = ImagesAndAnimations.Instance.GoldCoinAnimation;
                    break;
                default:
                    this._animation = ImagesAndAnimations.Instance.BronzeCoinAnimation;
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            this.X += this.GameScreen.GameBackground.HorizontalSpeed;
            _rectangle = new Rectangle((int)this.X, (int)this.Y, this._width, this._height);

            if (this.IsVisible && _rectangle.Intersects(this.GameScreen.GameCharacter.YellowRed))
            {
                CheckVerticalCollision(this.GameScreen.GameCharacter.TopBody, GameScreen.GameCharacter.BottomBody);
                CheckSideCollision(this.GameScreen.GameCharacter.LeftSide, GameScreen.GameCharacter.RightSide);
            }

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

            this._animation.Update(8);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.Sprite != null && this.IsVisible)
            {
                spriteBatch.Draw(this.Sprite, /*new Rectangle(*/GameScreen.Screen.ToScreen((int)this.X, (int)this.Y, this._width, this._height), Color.White);
            }
        }

        public void CheckVerticalCollision(Rectangle topBody, Rectangle bottomBody)
        {
            if (topBody.Intersects(_rectangle) || bottomBody.Intersects(_rectangle))
            {
                AddOnetimeAnimation();
                GameScreen.MusicPlayer.PlayEffect("coin");
                GameScreen.GameCharacter.Score += this.Value;
                this.IsVisible = false;
                _elements.RemoveElement(this);
            }
        }

        public void CheckSideCollision(Rectangle leftHand, Rectangle rightHand)
        {
            if (leftHand.Intersects(_rectangle) || rightHand.Intersects(_rectangle))
            {
                GameScreen.MusicPlayer.PlayEffect("coin");
                AddOnetimeAnimation();
                GameScreen.GameCharacter.Score += this.Value;
                this.IsVisible = false;
                _elements.RemoveElement(this);
            }
        }

        private void AddOnetimeAnimation()
        {
            switch (this.Value)
            {
                case 3:
                    this.GameScreen.OnetimeAnimations.Add(new OnetimeAnimation((int)this.X + 10, (int)this.Y - 40, 40, 40, ImagesAndAnimations.Instance.PlusThreeFrames, this.GameScreen));
                    break;
                case 2:
                    this.GameScreen.OnetimeAnimations.Add(new OnetimeAnimation((int)this.X + 10, (int)this.Y - 40, 40, 40, ImagesAndAnimations.Instance.PlusTwoFrames, this.GameScreen));
                    break;
                case 1:
                    this.GameScreen.OnetimeAnimations.Add(new OnetimeAnimation((int)this.X + 10, (int)this.Y - 40, 40, 40, ImagesAndAnimations.Instance.PlusOneFrames, this.GameScreen));
                    break;
            }
        }
    }
}
