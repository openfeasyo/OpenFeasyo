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
using GhostlyLib.Screens;

namespace GhostlyLib.Elements.Enemies
{
    public abstract class DifficultEnemy : Enemy
    {
        public override EnemyState State
        {
            get
            {
                switch (this.CurrentHealth)
                {
                    case 3:
                        return EnemyState.FullHealth;
                    case 2:
                        return EnemyState.HalfHealth;
                    case 1:
                        return EnemyState.LittleHealth;
                    default:
                        return EnemyState.LittleHealth;
                }
            }
        }

        public DifficultEnemy(int x, int y, LevelElements elements, GameScreen gameScreen) : base(x, y, elements, gameScreen)
        {
            this.CurrentHealth = 3;
            this.Bonus = 5;
        }
    }
}

