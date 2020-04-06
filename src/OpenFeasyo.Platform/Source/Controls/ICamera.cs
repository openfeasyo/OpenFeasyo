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

namespace OpenFeasyo.Platform.Controls
{
    public abstract class PerspectiveCamera : ICamera
    {
        public static Matrix SceneProjection { get; set; }

        public float Distance { get; set; }
        public float Angle { get; set; }
        public float Height { get; set; }
        public Vector3 TargetPosition { get; set; }

        public Matrix Projection { get { return SceneProjection; } }
        public Matrix View
        {
            get
            {
                return Matrix.CreateScale(1f) * Matrix.CreateTranslation(0, 0, 0) * GetLookAt();
            }
        }

        protected abstract Matrix GetLookAt();
    }

    public interface ICamera
    {
        float Distance { get; set; }
        float Angle { get; set; }
        float Height { get; set; }
        Vector3 TargetPosition { get; set; }

        Matrix Projection { get; }
        Matrix View { get; }
    }
}
