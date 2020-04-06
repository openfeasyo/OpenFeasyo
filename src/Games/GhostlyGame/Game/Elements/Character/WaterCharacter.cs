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
    public class WaterCharacter : GameCharacter
    {
        private const int SWIMSPEED = -10;
        private const int GRAVITY = 1;

        #region Private members

        private List<Projectile> projectiles = new List<Projectile>();
        private List<Projectile> projectilesToAdd = new List<Projectile>();

        private LevelElements _elements;

        #endregion Private members

        #region Public properties

        public override Texture2D Sprite
        {
            get
            {
                return this.Animation.GetImage();
            }
        }

        #endregion Public properties

        public WaterCharacter(GameScreen gameScreen, LevelElements elements) : base(gameScreen)
        {
            this.Animation = ImagesAndAnimations.Instance.SwimmingCharacterAnimation;
            this._elements = elements;

            this.SpeedX = GameScreen.SPEED;
            this.SpeedY = 0;

            this.Height = 62;
            this.Width = 70;

            this.CurrentHealth = 3;
            this.X = 100;
            this.Y = 250;

            this.VerticalMovement = VerticalMovement.Falling;
            this.HorizontalMovement = HorizontalMovement.MovingForward;
            this.LiveState = CharacterLiveState.Normal;
            this.Animation.SetCurrentFrames(this.LiveState);
            this.IsVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            // Move Character or Scroll Background accordingly.
            if (this.SpeedX < 0)
            {
                this.X += (int)this.SpeedX;
            }

            if (this.SpeedX == 0 || this.SpeedX < 0)
            {
                this.GameScreen.GameBackground.HorizontalSpeed = 0;
            }

            if (this.X <= 200 && this.SpeedX > 0)
            {
                this.X += (int)this.SpeedX;
            }
            if (this.SpeedX > 0 && this.X > 200)
            {
                this.GameScreen.GameBackground.HorizontalSpeed = -1 * GameScreen.SPEED;
            }
            
            if (this.SpeedY < 0)
            {
                this.SpeedY += GRAVITY;
            }
            else
            {
                this.SpeedY += 0.1;
            }
            this.SpeedY = Math.Min(this.SpeedY, 4);

            this.Y += (int)this.SpeedY;
            
            if (this.Y < 0)
            {
                this.Y = 0;
            }

            YellowRed = new Rectangle((int)this.X - 32, (int)this.Y - 51, 180, 120);
            TopBody = new Rectangle((int)this.X, (int)this.Y + 1, 55, 30);
            BottomBody = new Rectangle((int)this.X + 5, (int)this.Y + 39, 45, 16);

            LeftSide = new Rectangle((int)this.X, this.TopBody.Y + 10, 22, 35);
            RightSide = new Rectangle((int)this.X + 48, this.TopBody.Y + 10, 22, 35);

            this.Animation.Update(gameTime);

            if (this.TopBody.Y > 720)
            {
                this.Die();
            }

            CheckCollisions();
        }

        private void CheckCollisions()
        {
            IEnumerable<Drawable> tilesAround = this._elements.Tiles.Where(o => ((Tile)o).Rectangle.Intersects(this.YellowRed));

            IEnumerable<Drawable> tilesAhead = this._elements.Tiles.Where(o => ((Tile)o).Rectangle.Intersects(this.RightSide));

            if (tilesAhead.Count() > 0)
            {
                this.X = tilesAhead.ElementAt(0).X - this.Width;
                this.Blocked();

                if (tilesAhead.Where(t => ((Tile)t).TileType.Equals(TileType.Exit) || ((Tile)t).TileType.Equals(TileType.ExitSign)).Count() > 0)
                {
                    this.GameScreen.LevelDone();
                }
            }
            else
            {
                this.HorizontalMovement = HorizontalMovement.MovingForward;
            }

            IEnumerable<Drawable> tilesAbove = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.TopBody));

            if (tilesAbove.Count() > 0)
            {
                this.SpeedY = 0;
                this.Falling();
                this.Y = tilesAbove.ElementAt(0).Y + ((Tile)tilesAbove.ElementAt(0)).Rectangle.Height + 1;
            }

            IEnumerable<Drawable> tilesBelow = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.BottomBody)
                                                                       && !((Tile)o).TileType.Equals(TileType.InvisibleTile));

            if (tilesBelow.Count() > 0)
            {
                this.Standing((Tile)tilesBelow.ElementAt(0));
            }
            else if (this.VerticalMovement.Equals(VerticalMovement.Standing))
            {
                this.Falling();
            }

            IEnumerable<Drawable> tilesBehinf = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.TopBody));
        }

        public override void Jump()
        {
            //not applicable in water level
        }
        public override void LongJump()
        {
            //not applicable in water level
        }

        public override void Shoot()
        {
            this._elements.AddElement(new Projectile((int)this.X + 30, (int)this.Y + 19, ImagesAndAnimations.Instance.ProjectileAnimation, _elements, this.GameScreen));
        }

        public override void Stop()
        {
            this.SpeedX = 0;
        }

        public override void Blocked() //character collided with some tiles & cannot move forward
        {
            this.HorizontalMovement = HorizontalMovement.Blocked;
            this.SpeedX = 0;
        }

        public override void Standing() //legs on ground/platform
        {
            this.VerticalMovement = VerticalMovement.Standing;
            this.HorizontalMovement = HorizontalMovement.Blocked;

            this.SpeedY = 0;
            this.SpeedX = 0;
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
        
        public override void Swimming()
        {
            this.VerticalMovement = VerticalMovement.Swimming;
            this.SpeedY = SWIMSPEED;
            this.SpeedX = GameScreen.SPEED;
        }

        public override void SlidingOnIce()
        {
            //not applicable in water level
        }
        public override void BreakLongJump()
        {
            //not applicable in water level
        }
    }
}
