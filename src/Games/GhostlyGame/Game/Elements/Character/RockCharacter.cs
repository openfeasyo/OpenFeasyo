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

namespace GhostlyLib.Elements.Character
{
    public class RockCharacter : EarthCharacter
    {
        private LevelElements _elements;

        public RockCharacter(GameScreen gameScreen, LevelElements elements) : base(gameScreen, elements)
        {
            this._elements = elements;
        }

        public override void Shoot()
        {
            _elements.AddElement(new Projectile((int)this.X + 30, (int)this.Y + 19, ImagesAndAnimations.Instance.FireballAnimation, _elements, this.GameScreen));
        }
    }
}
