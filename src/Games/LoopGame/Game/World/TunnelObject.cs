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
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.GameTools.Core;
using System;

namespace LoopLib.World
{
    //
    // This class is a derivative work of the project published in the following website:
    // https://www.codeproject.com/Articles/471705/Celerity-Sensory-Overload
    //
    public class TunnelObject : SceneObject
    {
        private bool zInitialized;
        private Matrix _world;
 
        public TunnelObject(String name, Entity entity, Model model, Matrix transform,
#if WPF
            MonoGameControl.
#else
 Microsoft.Xna.Framework.
#endif
Game game, ICamera camera)
            : base(name, entity, model, transform, game, camera)
        {
            zInitialized = false;
            _world = World;
        }

        public void InitializeZPosition(float z)
        {
            if (!zInitialized)
            {
                Matrix m = World;
                m.Translation += new Vector3(0,0, z);
                World = m;
                zInitialized = true;
            }
        }

        public void InitializeTransform(Matrix transform, Vector2 positionOffset) {
            Matrix m = _world;
            m = m * transform;
            m.Translation -= new Vector3(positionOffset.X, -positionOffset.Y, m.Translation.Z*2);
            World = m;
            zInitialized = false;
        }

        public virtual bool OnCollision(SceneEntity entity, ElementManager manager) {
            return true;
        }
    }
}
