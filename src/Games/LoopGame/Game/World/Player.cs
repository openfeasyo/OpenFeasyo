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

namespace LoopLib.World
{

    public class Player: SceneObject
    {
        private float _tiltRotation;
        private Vector3 _position;
        private Matrix _world;
        private Tunnel _tunnel;
        private float _distanceFromCenter;
        private float _zDistance;

        public float Angle { get; set; }



        public Player(ContentRepository repo,
#if WPF
            MonoGameControl.
#else
 Microsoft.Xna.Framework.
#endif
        Game game, Camera camera, Tunnel tunnel)
            : base(
                "Player", 
                new Box(MathConverter.Convert(new Vector3(0, 0, 20)), 4f, 1f, 2.5f), 
                repo.LoadModel("Models/Ship2"),
                Matrix.CreateScale(0.0025f) * Matrix.CreateRotationX((float)Math.PI), 
                game, camera)
        {
                _world = World;
                _tunnel = tunnel;
                Angle = 0f;
                _distanceFromCenter = 7.8f;
                _zDistance = 6f;
                _tiltRotation = 0f;
        }

        public void Update(GameTime gameTime, float change, float tiltAngle)
        {
            _tiltRotation = tiltAngle;

            Angle += change;

            Vector3 direction = Tunnel.GetTunnelDirection(_tunnel.CurrentPhase + _zDistance);
            _position = new Vector3(0.0f, _distanceFromCenter, _zDistance);
            _position = Vector3.Transform(_position, Matrix.CreateRotationZ(Angle));
            Matrix m = _world * Matrix.CreateRotationZ(Angle - _tiltRotation);
            m.Translation = _position +_tunnel.GetTunnelCentrePos(_zDistance);
            World = m;
            _tiltRotation *= 0.9f;
            // update position
            base.Update(gameTime);
        }

    }
}
