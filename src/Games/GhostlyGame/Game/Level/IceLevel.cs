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
using System;

namespace GhostlyLib.Level
{
    public class IceLevel : Level
    {
        private LevelElements _elements;

        #region Public members
        public override Texture2D Background { get { return ImagesAndAnimations.Instance.Background; } }

        public override EnemyAnimation BlackEnemyAnimation { get { return null; } }

        public override Texture2D CliffLeft { get { return ImagesAndAnimations.Instance.IceCliffLeft; } }

        public override Texture2D CliffRight { get { return ImagesAndAnimations.Instance.IceCliffRight; } }

        public override Texture2D Dirt { get { return ImagesAndAnimations.Instance.Dirt; } }

        public override LevelElements Elements { get { return this._elements; } }

        public override Texture2D Foreground { get { return null; } }

        public override EnemyAnimation GreenEnemyAnimation { get { return null; } }

        public override Texture2D Ground { get { return ImagesAndAnimations.Instance.Ice; } }

        public override Texture2D LargeHill { get { return null; } }

        public override EnemyAnimation RedEnemyAnimation { get { return null; } }

        public override Texture2D SmallHill { get { return null; } }

        public override EnemyAnimation YellowEnemyAnimation { get { return null; } }

        public override GameCharacter Character { get; protected set; }

        public override Texture2D BackgroundClosest => throw new NotImplementedException();

        public override Texture2D BackgroundCloser => throw new NotImplementedException();

        public override Texture2D BackgroundClose => throw new NotImplementedException();

        public override Texture2D BackgroundFar => throw new NotImplementedException();

        public override Texture2D BackgroundFurther => throw new NotImplementedException();

        public override Texture2D BackgroundFurthest => throw new NotImplementedException();

        #endregion Public members

        public IceLevel(GameScreen gameScreen, LevelElements elements): base(gameScreen)
        {
            this._elements = elements;
            this.Character = new EarthCharacter(gameScreen, elements);
        }

        //public override void ProcessPrimaryAction(IEmgSignal[] signals, int index)
        public override void ProcessPrimaryAction(bool state)
        {
            throw new NotImplementedException();
            //TODO
            //stop sliding
        }

        //public override void ProcessSecondaryAction(IEmgSignal[] signals, int index)
        public override void ProcessSecondaryAction(bool state)
        {
            throw new NotImplementedException();
            //TODO
            //shoot

            //if (signals[index].MuscleActivated && _prevShootState != signals[index].MuscleActivated)
            //{
            //    GameScreen.GameCharacter.Shoot();
            //}

            //_prevShootState = signals[index].MuscleActivated;
        }

        public override Enemy CreateBlackEnemy(int i, int j)
        {
            return null;
        }
        public override Enemy CreateGreenEnemy(int i, int j)
        {
            return null;
        }
        public override Enemy CreateRedEnemy(int i, int j)
        {
            return null;
        }
        public override Enemy CreateYellowEnemy(int i, int j)
        {
            return null;
        }
    }
}
