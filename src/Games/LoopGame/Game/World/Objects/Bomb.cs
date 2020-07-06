/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using OpenFeasyo.GameTools.Bepu;
using OpenFeasyo.GameTools.Core;
using System;

namespace LoopLib.World.Objects
{
    public class Bomb : TunnelObject
    {
        public const int NUM_EXPLOSION_PARTICLES = 20;
        public const int NUM_SMOKE_PARTICLES = 30;

        public Bomb(ContentRepository repo,
#if WPF
            MonoGameControl.
#else
 Microsoft.Xna.Framework.
#endif
Game game, ICamera camera)
            : base(
            "Bomb", 
            new Sphere(MathConverter.Convert(Vector3.Zero), 2f, 0.2f),
            repo.LoadModel("Models/Bomb"),
            Matrix.CreateScale(0.4f) * Matrix.CreateTranslation(new Vector3(0, -2, 0)) * Matrix.CreateRotationZ((float)Math.PI), 
            game, 
            camera) { 
        
        }

        public override bool OnCollision(SceneEntity entity, ElementManager manager)
        {
            //Globals.TotalScore = Math.Max(0, Globals.TotalScore-5);
            manager.Music.PlayEffect("explode");
            Vector3 pos = (this.World.Translation + entity.World.Translation)/2f;
            
            for (int i = 0; i < NUM_SMOKE_PARTICLES; i++)
            {
                manager.SmokeParticles.AddParticle(pos, Vector3.Zero);
            }
            for (int i = 0; i < NUM_EXPLOSION_PARTICLES; i++)
            {
                manager.ExplosionParticles.AddParticle(pos, Vector3.Zero);
            }
            Globals.Lives--;
            return true;
        }
    }
}
