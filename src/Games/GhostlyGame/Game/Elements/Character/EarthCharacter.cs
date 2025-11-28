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
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Elements.Character
{
    public class EarthCharacter : GameCharacter
    {
        private const int JUMPSPEED = -15;
        private const int GRAVITY = 1;

        #region Private members

        //private List<Projectile> projectiles = new List<Projectile>();
        //private List<Projectile> projectilesToAdd = new List<Projectile>();

        private Texture2D _jumpingImage;
        private LevelElements _elements;

        private double fall = 0;

        #endregion Private members

        #region Public properties

        public override Texture2D Sprite
        {
            get
            {
                if (this.ActionMovement.Equals(ActionMovement.Jumping) || this.ActionMovement.Equals(ActionMovement.LongJumping) || this.ActionMovement.Equals(ActionMovement.Falling))
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

            this.AutomaticMovement = AutomaticMovement.MovingForward;
            this.ActionMovement = ActionMovement.Falling;

            this.Animation.SetCurrentFrames(CharacterLiveState.Normal);
            this.IsVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            //while longjumping gravity applies only until 0 gravity
            if (this.ActionMovement.Equals(ActionMovement.LongJumping) && this.SpeedY >= 0)
            {
                this.SpeedY = 0;
            }
            else
            {
                if (this.ActionMovement.Equals(ActionMovement.Falling))
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

            if (AutomaticMovement.Equals(AutomaticMovement.MovingForward))
            {
                this.SpeedX = GameScreen.SPEED;
            }
            else
            {
                this.SpeedX = 0;
            }

            // Move Character or Scroll Background accordingly.
            //KATKa: is this ever used???
            //if (this.SpeedX < 0)
            //{
            //    this.X += (int)(gameTime.ElapsedGameTime.Milliseconds * this.SpeedX);
            //}

            //KATKA: is this ever used???
            //if (this.X <= 200 && this.SpeedX > 0)
            //{
            //    this.X += (int)(gameTime.ElapsedGameTime.Milliseconds * this.SpeedX);
            //}
            
            MainBody = new Rectangle((int)this.X - 32, (int)this.Y - 71, 120, 180);
            Top = new Rectangle((int)this.X + 12, (int)this.Y + 1, 35, 35);
            Bottom = new Rectangle((int)this.X + 23, (int)this.Y + 59, 13, 19);
            //LeftSide = new Rectangle(this.X, this.TopBody.Y + 29, 11, 25);
            RightSide = new Rectangle((int)this.X + 48, this.Top.Y + 19, 11, 32);

            Animation.Update(gameTime);

            if (this.Top.Y > 720)
            {
                GameScreen.MusicPlayer.PlayEffect("drown");
                this.Die();
            }

            CheckCollisions();
            GameScreen.GameBackground.HorizontalSpeed = AutomaticMovement == AutomaticMovement.Blocked ? 0 : -GameScreen.SPEED;
        }

        private void CheckCollisions()
        {
            IEnumerable<IDrawable> tilesAround = this._elements.Tiles.Where(o => ((Tile)o).Rectangle.Intersects(this.MainBody));

            IEnumerable<IDrawable> tilesAhead = this._elements.Tiles.Where(o => ((Tile)o).Rectangle.Intersects(this.RightSide)
                    && (!((Tile)o).TileType.Equals(TileType.Dirt)
                    && !((Tile)o).TileType.Equals(TileType.DeepWater) && !((Tile)o).TileType.Equals(TileType.Water)
                    && !((Tile)o).TileType.Equals(TileType.DeepLava) && !((Tile)o).TileType.Equals(TileType.Lava)));

            if (tilesAhead.Count() > 0)
            {
                if (((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.Checkpoint))
                {
                    //original X stores original position of the tile, at the start of the level, e.g. 50th tile from the left
                    //also, we save the checkpoint position 5 tiles before the actual checkpoint in the game
                    GameScreen.SetCheckpoint(((Tile)tilesAhead.ElementAt(0)).OriginalX - 5);   
                }
                else
                {
                    this.X = ((Drawable)tilesAhead.ElementAt(0)).X - this.Width;
                    this.Blocked();

                    if (((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.Exit) || ((Tile)tilesAhead.ElementAt(0)).TileType.Equals(TileType.ExitSign))
                    {
                        GameScreen.LevelDone();
                    }
                }
            }
            else
            {
                this.AutomaticMovement = AutomaticMovement.MovingForward;
            }

            IEnumerable<IDrawable> tilesAbove = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.Top)
                && !((Tile)o).TileType.Equals(TileType.DeepWater) 
                && !((Tile)o).TileType.Equals(TileType.DeepLava) 
                && !((Tile)o).TileType.Equals(TileType.Checkpoint));

            if (tilesAbove.Count() > 0)
            {
                this.SpeedY = 0;
                this.Falling();
                this.Y = ((Drawable)tilesAbove.ElementAt(0)).Y + ((Tile)tilesAbove.ElementAt(0)).Rectangle.Height + 1;

                IEnumerable<IDrawable> tilesExclam = tilesAbove.Where(o => ((Tile)o).TileType.Equals(TileType.Exclamation));

                foreach (Tile ex in tilesExclam)
                {
                    GameScreen.MusicPlayer.PlayEffect("secret");
                    AddOnetimePlusTwoAnimation();
                    this.Score += 2;
                    ex.IsVisible = false;
                    this._elements.RemoveElement(ex);
                }
            }

            IEnumerable<IDrawable> tilesBelow = tilesAround.Where(o => ((Tile)o).Rectangle.Intersects(this.Bottom)
                    && !((Tile)o).TileType.Equals(TileType.Water) && !((Tile)o).TileType.Equals(TileType.Lava)
                    && !((Tile)o).TileType.Equals(TileType.DeepWater) && !((Tile)o).TileType.Equals(TileType.DeepLava)
                    && !((Tile)o).TileType.Equals(TileType.Checkpoint));

            if (tilesBelow.Count() > 0)
            {
                this.Standing((Tile)tilesBelow.ElementAt(0));
            }
            else if (this.ActionMovement.Equals(ActionMovement.Standing))
            {
                this.Falling();
            }
        }

        public override void Jump()
        {
            if (this.ActionMovement.Equals(ActionMovement.Standing))
            {
                GameScreen.MusicPlayer.PlayEffect("jump");
                this.ActionMovement = ActionMovement.Jumping;
                this.SpeedY = JUMPSPEED;
                this.SpeedX = GameScreen.SPEED;
            }
        }

        public override void LongJump()
        {
            this.ActionMovement = ActionMovement.LongJumping;
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
            this.AutomaticMovement = AutomaticMovement.Blocked;

            if (this.ActionMovement.Equals(ActionMovement.Jumping) || this.ActionMovement.Equals(ActionMovement.LongJumping) || this.ActionMovement.Equals(ActionMovement.Falling))
            {
                this.ActionMovement = ActionMovement.Falling;
            }
            else
            {
                this.ActionMovement = ActionMovement.Standing;
            }
        }

        //legs on ground/platform
        public override void Standing()
        {
            this.SpeedY = 0;
            this.ActionMovement = ActionMovement.Standing;
        }

        private void Standing(Tile t)
        {
            Standing();
            this.Y = t.Y - GameScreen.GameCharacter.Height;
        }

        public override void Falling()
        {
            this.ActionMovement = ActionMovement.Falling;
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

        public override void MoveLeft()
        {
            //not applicable in earth level
        }

        public override void MoveRight()
        {
            //not applicable in earth level
        }

        public override void StopLeftRightMovement()
        {
            // not applicable in earth level
        }
    }
}
