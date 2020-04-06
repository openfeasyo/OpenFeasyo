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

namespace GhostlyLib.Level
{
    public class WaterLevel : Level
    {
        #region Private members
        private bool canShoot = true;
        private Timer _timer;

        private LevelElements _elements;
        #endregion Private members

        #region Public members
        public override Texture2D Background { get { return ImagesAndAnimations.Instance.BackgroundWater; } }

        public override Texture2D Foreground { get { return null; } }// ImagesAndAnimations.Instance.ForegroundWater; } }

        public override EnemyAnimation BlackEnemyAnimation { get { return ImagesAndAnimations.Instance.BlackFishAnimation; } }

        public override Texture2D CliffLeft { get { return ImagesAndAnimations.Instance.SandCliffLeft; } }

        public override Texture2D CliffRight { get { return ImagesAndAnimations.Instance.SandCliffRight; } }

        public override Texture2D Dirt { get { return ImagesAndAnimations.Instance.Dirt; } }

        public override LevelElements Elements { get { return this._elements; } }

        public override EnemyAnimation GreenEnemyAnimation { get { return ImagesAndAnimations.Instance.GreenFishAnimation; } }

        public override Texture2D Ground { get { return ImagesAndAnimations.Instance.Sand; } }

        public override Texture2D LargeHill { get { return ImagesAndAnimations.Instance.HillLarge; } }

        public override EnemyAnimation RedEnemyAnimation { get { return ImagesAndAnimations.Instance.RedFishAnimation; } }

        public override Texture2D SmallHill { get { return ImagesAndAnimations.Instance.HillSmall; } }

        public override EnemyAnimation YellowEnemyAnimation { get { return ImagesAndAnimations.Instance.YellowFishAnimation; } }

        public override GameCharacter Character { get; protected set; }

        public override Texture2D BackgroundClosest { get { return ImagesAndAnimations.Instance.BackgroundClosestWater; } }

        public override Texture2D BackgroundCloser { get { return ImagesAndAnimations.Instance.BackgroundCloserWater; } }

        public override Texture2D BackgroundClose { get { return ImagesAndAnimations.Instance.BackgroundCloseWater; } }

        public override Texture2D BackgroundFar => throw new System.NotImplementedException();

        public override Texture2D BackgroundFurther => throw new System.NotImplementedException();

        public override Texture2D BackgroundFurthest => throw new System.NotImplementedException();

        #endregion Public members

        public WaterLevel(GameScreen gameScreen, LevelElements elements): base(gameScreen)
        {
            this._elements = elements;
            this.Character = new WaterCharacter(gameScreen, elements);
        }

        public override void ProcessPrimaryAction(bool state)
        {
            if (state)
            {
                this.Character.Swimming();
            }
            else
            {
                this.Character.Falling();
            }
        }

        public override void ProcessSecondaryAction(bool state)
        {
            if (state && canShoot)
            {
                canShoot = false;

                this.Character.Shoot();

                this._timer = new Timer(200);
                this._timer.Elapsed += Timer_Elapsed;
                this._timer.Start();
            }
        }

        public override Enemy CreateBlackEnemy(int i, int j)
        {
            return new BlackFishEnemy(i, j, this.Elements, this.BlackEnemyAnimation, this.GameScreen);
        }
        public override Enemy CreateGreenEnemy(int i, int j)
        {
            return new GreenFishEnemy(i, j, this.Elements, this.GreenEnemyAnimation, this.GameScreen);
        }
        public override Enemy CreateRedEnemy(int i, int j)
        {
            return new RedFishEnemy(i, j, this.Elements, this.RedEnemyAnimation, this.GameScreen);
        }
        public override Enemy CreateYellowEnemy(int i, int j)
        {
            return new YellowFishEnemy(i, j, this.Elements, this.YellowEnemyAnimation, this.GameScreen);
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.canShoot = true;
            this._timer.Stop();
        }
    }
}
