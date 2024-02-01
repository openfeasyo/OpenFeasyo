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
using GhostlyLib.Elements;
using GhostlyLib.Elements.Character;
using GhostlyLib.Elements.Enemies;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;
using System.Timers;
using System;

namespace GhostlyLib.Level
{
    public class IceLevel : Level
    {
        #region Private members
        private bool canShoot = true;
        private Timer _timer;

        private LevelElements _elements;
        #endregion Private members

        #region Public members
        public override Texture2D Background { get { return ImagesAndAnimations.Instance.Background; } }

        public override EnemyAnimation BlackEnemyAnimation { get { return ImagesAndAnimations.Instance.BlackFlyAnimation; } }

        public override Texture2D CliffLeft { get { return ImagesAndAnimations.Instance.IceCliffLeft; } }

        public override Texture2D CliffRight { get { return ImagesAndAnimations.Instance.IceCliffRight; } }

        public override Texture2D Dirt { get { return ImagesAndAnimations.Instance.Dirt; } }

        public override LevelElements Elements { get { return this._elements; } }

        public override Texture2D Foreground { get { return null; } }

        public override EnemyAnimation GreenEnemyAnimation { get { return ImagesAndAnimations.Instance.GreenFlyAnimation; } }

        public override Texture2D Ground { get { return ImagesAndAnimations.Instance.Ice; } }

        public override Texture2D LargeHill { get { return ImagesAndAnimations.Instance.HillLarge; } }

        public override EnemyAnimation RedEnemyAnimation { get { return ImagesAndAnimations.Instance.RedFlyAnimation; } }

        public override Texture2D SmallHill { get { return ImagesAndAnimations.Instance.HillSmall; } }

        public override EnemyAnimation YellowEnemyAnimation { get { return ImagesAndAnimations.Instance.YellowFlyAnimation; } }

        public override GameCharacter Character { get; protected set; }

        public override Texture2D BackgroundClosest { get { return ImagesAndAnimations.Instance.BackgroundClosestRock; } }

        public override Texture2D BackgroundCloser { get { return ImagesAndAnimations.Instance.BackgroundCloserRock; } }

        public override Texture2D BackgroundClose { get { return ImagesAndAnimations.Instance.BackgroundCloseRock; } }

        public override Texture2D BackgroundFar { get { return ImagesAndAnimations.Instance.BackgroundFar; } }

        public override Texture2D BackgroundFurther { get { return ImagesAndAnimations.Instance.BackgroundFurther; } }

        public override Texture2D BackgroundFurthest { get { return ImagesAndAnimations.Instance.BackgroundFurthest; } }

        #endregion Public members

        public IceLevel(GameScreen gameScreen, LevelElements elements): base(gameScreen)
        {
            this._elements = elements;
            this.Character = new EarthCharacter(gameScreen, elements);
        }

        public override void ProcessPrimaryAction(bool state)
        {
            if (state)
            {
                if (GameScreen.GameCharacter.VerticalMovement.Equals(VerticalMovement.LongJumping) || GameScreen.GameCharacter.VerticalMovement.Equals(VerticalMovement.Jumping))
                {
                    GameScreen.GameCharacter.LongJump();
                }
                else
                {
                    GameScreen.GameCharacter.Jump();
                }
            }
            else
            {
                if (GameScreen.GameCharacter.VerticalMovement.Equals(VerticalMovement.LongJumping))
                {
                    GameScreen.GameCharacter.BreakLongJump();
                }
                else if (GameScreen.GameCharacter.VerticalMovement.Equals(VerticalMovement.Jumping))
                {
                    GameScreen.GameCharacter.Falling();
                }
            }
        }

        public override void ProcessSecondaryAction(bool state)
        {
            if (state && canShoot)
            {
                canShoot = false;

                GameScreen.GameCharacter.Shoot();

                this._timer = new Timer(200);
                this._timer.Elapsed += Timer_Elapsed;
                this._timer.Start();
            }
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.canShoot = true;
            this._timer.Stop();
        }

        public override Enemy CreateBlackEnemy(int i, int j)
        {
            return new BlackFlyEnemy(i, j, this.Elements, this.BlackEnemyAnimation, this.GameScreen);
        }
        public override Enemy CreateGreenEnemy(int i, int j)
        {
            return new GreenFlyEnemy(i, j, this.Elements, this.GreenEnemyAnimation, this.GameScreen);
        }
        public override Enemy CreateRedEnemy(int i, int j)
        {
            return new RedFlyEnemy(i, j, this.Elements, this.RedEnemyAnimation, this.GameScreen);
        }
        public override Enemy CreateYellowEnemy(int i, int j)
        {
            return new YellowFlyEnemy(i, j, this.Elements, this.YellowEnemyAnimation, this.GameScreen);
        }
    }
}
