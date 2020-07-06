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
using Microsoft.Xna.Framework;
using OpenFeasyo.GameTools.Core;
using System;

namespace LoopLib.World
{
    public class Camera : ICamera
    {
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        public Vector3 position;
        public Vector3 up;
        public Vector3 rotatedUp;
        public Vector3 cameraLookAt;
        public float distanceFromCentre;
        public float lookDownAngleDegrees;
        private float lookAheadDistance;

        public Vector3 headMovementScaleFactor;

        public Camera(Matrix view, Matrix projection)
        {
            View = view;
            Projection = projection;
            up = new Vector3(0.0f, 1.0f, 0.0f);
            position = Vector3.Zero;
            cameraLookAt = new Vector3(0.0f, 0.0f, 1.0f);
            distanceFromCentre = 3.0f;
            lookDownAngleDegrees = 10f;
            headMovementScaleFactor = new Vector3(3.0f, 3.0f, 3.0f);
            lookAheadDistance = 10000.0f;
        }

        public void Update(GameTime gameTime, float currentPhase/*, Vector3 headPos*/, float shipAngle)
        {
            Vector2 offset = Tunnel.GetTunnelOffset(currentPhase);
            this.position = Vector3.Transform(this.up * distanceFromCentre, Matrix.CreateRotationZ(shipAngle)) + new Vector3(-offset.X, offset.Y, 0.0f);
            Matrix cameraRotation = Matrix.CreateRotationZ(shipAngle + (float)Math.PI);
            this.rotatedUp = Vector3.Transform(this.up, cameraRotation);
            Vector3 direction = Tunnel.GetTunnelDirection(currentPhase);
            direction.X = -direction.X;

            //Matrix headRotationMatrix = Matrix.CreateFromAxisAngle(this.up, (float)Math.Atan(headPos.X));
            //Vector3 newHeadPos = new Vector3(-1.0f * headPos.X, -1.0f * headPos.Y, 1.0f * headPos.Z);
            //newHeadPos = Vector3.Transform(newHeadPos, headRotationMatrix * cameraRotation);
            //newHeadPos.X *= headMovementScaleFactor.X;
            //newHeadPos.Y *= headMovementScaleFactor.Y;
            //newHeadPos.Z *= headMovementScaleFactor.Z;

            //projects to a point far in the distance
            Vector3 down = new Vector3(0.0f, -(lookAheadDistance * (float)Math.Tan(lookDownAngleDegrees * (float)Math.PI / 180.0f)), 0.0f);
            this.cameraLookAt = position + (direction * lookAheadDistance) + Vector3.Transform(down, cameraRotation);

            //this.cameraLookAt = Vector3.Transform(this.cameraLookAt, cameraRotation * Matrix.CreateRotationX(lookDownAngleDegrees * (float)(Math.PI / 180)));
            View = Matrix.CreateLookAt(this.position /*+ newHeadPos*/, this.cameraLookAt, this.rotatedUp);
        }
    }
}
