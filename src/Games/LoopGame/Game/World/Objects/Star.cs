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
    public class Star : TunnelObject
    {
        public Star(ContentRepository repo,
#if WPF
            MonoGameControl.
#else
 Microsoft.Xna.Framework.
#endif
Game game, ICamera camera)
            : base(
            "Star",
            new Sphere(MathConverter.Convert(Vector3.Zero), 2f, 0.2f),
            repo.LoadModel("Models/Star"),
            Matrix.CreateScale(0.04f) * Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Matrix.CreateRotationZ((float)Math.PI),
            game,
            camera)
        {
            //ShowBoundingBox = true;
            //this.Visible = false;
        }

        public override bool OnCollision(SceneEntity entity, ElementManager manager)
        {
            Globals.TotalScore++;
            manager.Music.PlayEffect("star");
            return true;
        }
    }
}
