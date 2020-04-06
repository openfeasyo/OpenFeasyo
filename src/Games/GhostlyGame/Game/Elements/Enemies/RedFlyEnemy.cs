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

namespace GhostlyLib.Elements.Enemies
{
    public class RedFlyEnemy : MediumEnemy
    {
        public override int Height { get { return 40; } }
        public override int Width { get { return 63; } }

        public RedFlyEnemy(int x, int y, LevelElements elements, EnemyAnimation animation, GameScreen gameScreen) : base(x, y, elements, gameScreen)
        {
            this.Animation = animation;
            this.Animation.SetCurrentFrames(EnemyState.FullHealth);
        }
    }
}
