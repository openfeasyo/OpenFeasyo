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
using GhostlyLib.Elements.Weapons;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostlyLib.Elements.Character
{
    public class EarthCharacter : GameCharacter
    {
        private const int JUMPSPEED = -15;
        private const int GRAVITY = 1;

        #region Private members

        private List<Projectile> projectiles = new List<Projectile>();
        private List<Projectile> projectilesToAdd = new List<Projectile>();

        private Texture2D _jumpingImage;
        private LevelElements _elements;

        private double fall = 0;

        #endregion Private members

        #region Public properties

        public override Texture2D Sprite
        {
            get
            {
                if (this.VerticalMovement.Equals(VerticalMovement.Jumping) || this.VerticalMovement.Equals(VerticalMovement.LongJumping) || this.VerticalMovement.Equals(VerticalMovement.Falling))
                {
                    return _jumpingImage;
                }
                else
                {
                    return this.Animation.GetImage();
                }
            }
        }

        #endregion Public properties

        public EarthCharacter(GameScreen gameScreen, LevelElements elements) : base(gameScreen)
        {
            this._elements = elements;

            this._jumpingImage = ImagesAndAnimations.Instance.CharacterJumping;
            this.Animation = ImagesAndAnimations.Instance.CharacterAnimation;

            this.SpeedY = 0;

            this.Height = 77;
            this.Width = 56;

            this.CurrentHealth = 3;
            this.X = 100;
            this.Y = 250;

            this.HorizontalMovement = HorizontalMovement.MovingForward;
            this.VerticalMovement = VerticalMovement.Falling;

            this.Animation.SetCurrentFrames(CharacterLiveState.Normal);
            this.IsVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            //while longjumping gravity applies only until 0 gravity
            if (this.VerticalMovement.Equals(VerticalMovement.LongJumping) && this.SpeedY >= 0)
            {
                this.SpeedY = 0;
            }
            else
            {
                if (this.VerticalMovement.Equals(VerticalMovement.Falling))
                {
                    fall += (double)GRAVITY / 2;

                    if (fall >= 1)
                    {
                        this.SpeedY += GRAVITY;
                        fall = 0;
                    }
                }
                else
                {
                    this.SpeedY += GRAVITY;
                }
            }

            this.Y += (int)this.SpeedY;

            if (this.Y < 0)
            {
                this.Y = 0;
            }

            if (HorizontalMovement.Equals(HorizontalMovement.MovingForward))
            {
                this.SpeedX = GameScreen.SPEED;
            }
            else
            {
                this.SpeedX = 0;
            }

            // Move Character or Scroll Background accordingly.
            if (this.SpeedX < 0)
            {
                this.X += (int)(gameTime.ElapsedGameTime.Milliseconds * this.SpeedX);
            }

            //if (this.SpeedX == 0 || this.SpeedX < 0)
            //{
            //    GameScreen.GameBackground.HorizontalSpeed = 0;
            //}

            if (this.X <= 200 && this.SpeedX > 0)
            {
                this.X += (int)(gameTime.ElapsedGameTime.Milliseconds * this.SpeedX);
            }
            //if (this.SpeedX > 0 && this.X > 200)
            //{
            //    GameScreen.GameBackground.HorizontalSpeed = -1 * GameScreen.SPEED;
            //}

            YellowRed = new Rectangle((int)this.X - 32, (int)this.Y - 71, 120, 180);
            TopBody = new Rectangle((int)this.X + 12, (int)this.Y + 1, 35, 35);
            BottomBody = new Rectangle((int)this.X + 23, (int)this.Y + 59, 13, 19);
            //LeftSide = new Rectangle(this.X, this.TopBody.Y + 29, 11, 25);
            RightSide = new Rectangle((int)this.X + 48, this.TopBody.Y + 19, 11, 32);

            Animation.Update(gameTime);

            if (this.TopBody.Y > 720)
            {
                GameScreen.MusicPlayer.PlayEffect("drown");
                this.Die();
            }

            CheckCollisions();
            GameScreen.GameBackground.HorizontalSpeed = HorizontalMovement == HorizontalMovement.Blocked ? 0 : -GameScreen.SPEED;
        }

        private void CheckCollisions()
        {
            IEnumerable<Drawable> tilesAround = this._elements.Tiles.Where(o => ((Tile)o).Rectangle.Intersects(this.YellowRed));

            IEnumerable<Drawable> tilesAhead = this._elements.Tiles.Where(o => ((Tile)o).Rectangle.Intersects(this.RightSide)
                    && (!((Tile)o).TileType.Equals(TileType.Dirt) && !((Tile)o).TileType.Equals(TileType.DeepWater) && !((Tile)o).TileType.Equals(TileType.Water)
                    && !((Tile)o).TileType.Equals(TileType.DeepLava) && !((Tile)o).TileType.Equals(TileType.Lava)));

            if (tilesAhead.Count() > 0)
            {
                this.X = tilesAhead.ElementAt(0).X - this.Width;
                this.Blocked();

                if (((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.Exit) || ((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.ExitSign))
                {
                    GameScreen.LevelDone();
                }
            }
            else
            {
                this.HorizontalMovement = HorizontalMovement.MovingForward;
            }

            IEnumerable<Drawable> tilesAbove = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.TopBody)
                                                                 && !((Tile)o).TileType.Equals(TileType.DeepWater) && !((Tile)o).TileType.Equals(TileType.DeepLava));

            if (tilesAbove.Count() > 0)
            {
                this.SpeedY = 0;
                this.Falling();
                this.Y = tilesAbove.ElementAt(0).Y + ((Tile)tilesAbove.ElementAt(0)).Rectangle.Height + 1;

                IEnumerable<Drawable> tilesExclam = tilesAbove.Where(o => ((Tile)o).TileType.Equals(TileType.Exclamation));

                foreach (Tile ex in tilesExclam)
                {
                    GameScreen.MusicPlayer.PlayEffect("secret");
                    AddOnetimePlusTwoAnimation();
                    this.Score += 2;
                    ex.IsVisible = false;
                    this._elements.RemoveElement(ex);
                }
            }

            IEnumerable<Drawable> tilesBelow = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.BottomBody)
                                                                 && !((Tile)o).TileType.Equals(TileType.Water) && !((Tile)o).TileType.Equals(TileType.Lava)
                                                                 && !((Tile)o).TileType.Equals(TileType.DeepWater) && !((Tile)o).TileType.Equals(TileType.DeepLava));

            if (tilesBelow.Count() > 0)
            {
                this.Standing((Tile)tilesBelow.ElementAt(0));
            }
            else if (this.VerticalMovement.Equals(VerticalMovement.Standing))
            {
                this.Falling();
            }
        }

        public override void Jump()
        {
            if (this.VerticalMovement.Equals(VerticalMovement.Standing))
            {
                GameScreen.MusicPlayer.PlayEffect("jump");
                this.VerticalMovement = VerticalMovement.Jumping;
                this.SpeedY = JUMPSPEED;
                this.SpeedX = GameScreen.SPEED;
            }
        }

        public override void LongJump()
        {
            this.VerticalMovement = VerticalMovement.LongJumping;
        }

        public override void Shoot()
        {
            GameScreen.MusicPlayer.PlayEffect("shoot");
            this._elements.AddElement(new Projectile((int)this.X + 30, (int)this.Y + 19, ImagesAndAnimations.Instance.ProjectileAnimation, this._elements, this.GameScreen));
        }

        public override void Stop()
        {
            this.SpeedX = 0;
        }

        //character collided with some tiles & cannot move forward
        public override void Blocked()
        {
            this.HorizontalMovement = HorizontalMovement.Blocked;

            if (this.VerticalMovement.Equals(VerticalMovement.Jumping) || this.VerticalMovement.Equals(VerticalMovement.LongJumping) || this.VerticalMovement.Equals(VerticalMovement.Falling))
            {
                this.VerticalMovement = VerticalMovement.Falling;
            }
            else
            {
                this.VerticalMovement = VerticalMovement.Standing;
            }
        }

        //legs on ground/platform
        public override void Standing()
        {
            this.SpeedY = 0;
            this.VerticalMovement = VerticalMovement.Standing;
        }

        private void Standing(Tile t)
        {
            Standing();
            this.Y = t.Y - GameScreen.GameCharacter.Height;
        }

        public override void Falling()
        {
            this.VerticalMovement = VerticalMovement.Falling;
        }

        public override void SlidingOnIce()
        {
            //not applicable in earth level
        }
        public override void Swimming()
        {
            //not applicable in earth level
        }

        public override void BreakLongJump()
        {
            this.Falling();
        }

        private void AddOnetimePlusTwoAnimation()
        {
            this.GameScreen.OnetimeAnimations.Add(new OnetimeAnimation((int)this.X + 10, (int)this.Y - 40, 40, 40, ImagesAndAnimations.Instance.PlusTwoFrames, this.GameScreen));
        }
    }
}
