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
using Microsoft.Xna.Framework.Input;
using System;

namespace OpenFeasyo.GameTools.Core
{
    /// <summary>
    /// Basic camera class supporting mouse/keyboard/gamepad-based movement.
    /// </summary>
    public class Camera : ICamera
    {
        /// <summary>
        /// Gets or sets the position of the camera.
        /// </summary>
        public Vector3 Position { get; set; }
        float yaw;
        float pitch;
        /// <summary>
        /// Gets or sets the yaw rotation of the camera.
        /// </summary>
        public float Yaw
        {
            get
            {
                return yaw;
            }
            set
            {
                yaw = MathHelper.WrapAngle(value);
            }
        }
        /// <summary>
        /// Gets or sets the pitch rotation of the camera.
        /// </summary>
        public float Pitch
        {
            get
            {
                return pitch;
            }
            set
            {
                pitch = MathHelper.Clamp(value, -MathHelper.PiOver2, MathHelper.PiOver2);
            }
        }

        /// <summary>
        /// Gets or sets the speed at which the camera moves.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets the view matrix of the camera.
        /// </summary>
        public Matrix View { get; private set; }
        /// <summary>
        /// Gets or sets the projection matrix of the camera.
        /// </summary>
        public Matrix Projection { get; set; }

        /// <summary>
        /// Gets the world transformation of the camera.
        /// </summary>
        public Matrix WorldMatrix { get; private set; }
        
        public Vector3 Rotation { get; set; }

        private MouseState referenceMouseState;
        private bool rotating;
        /// <summary>
        /// Constructs a new camera.
        /// </summary>
        /// <param name="game">Game that this camera belongs to.</param>
        /// <param name="position">Initial position of the camera.</param>
        /// <param name="speed">Initial movement speed of the camera.</param>
        public Camera(Vector3 position, Vector3 rotation, float speed)
        {
            Position = position;
            Rotation = rotation;
            Speed = speed;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4f / 3f, .1f, 10000.0f);
            rotating = false;
        }

        /// <summary>
        /// Moves the camera forward using its speed.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        public void MoveForward(float dt)
        {
            Position += WorldMatrix.Forward * (dt * Speed);
        }
        /// <summary>
        /// Moves the camera right using its speed.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        /// 
        public void MoveRight(float dt)
        {
            Position += WorldMatrix.Right * (dt * Speed);
        }
        /// <summary>
        /// Moves the camera up using its speed.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        /// 
        public void MoveUp(float dt)
        {
            Position += new Vector3(0, (dt * Speed), 0);
        }


        /// <summary>
        /// Updates the camera's view matrix.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        public void Update(float dt)
        {            
#if XBOX360
            //Turn based on gamepad input.
            Yaw += Game.GamePadState.ThumbSticks.Right.X * -1.5f * dt;
            Pitch += Game.GamePadState.ThumbSticks.Right.Y * 1.5f * dt;
#else
            //Turn based on mouse input.
            //Yaw += (200 - Game.MouseState.X) * dt * .12f;
            //Pitch += (200 - Game.MouseState.Y) * dt * .12f;

#if !WPF
            //
            // Move to tools and in a GameEngine menu
            //
            //MouseState mouseState = Mouse.GetState();
            //if (mouseState.LeftButton == ButtonState.Pressed)
            //{
            //    if (rotating) { 
            //        Rotation = new Vector3(Rotation.X + ((referenceMouseState.X - mouseState.X) * dt * .12f),
            //                     Rotation.Y + ((referenceMouseState.Y - mouseState.Y) * dt * .12f),
            //                     Rotation.Z);// Rotation.Z + ((200 - Game.MouseState.Y) * dt * .12f));
            //        Mouse.SetPosition(referenceMouseState.X, referenceMouseState.Y);
            //    }
            //    else { 
            //        referenceMouseState = mouseState;
            //    }
            //}
            //rotating = mouseState.LeftButton == ButtonState.Pressed;
#endif            

            

            
#endif
            

            WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Right, Rotation.Y) * Matrix.CreateFromAxisAngle(Vector3.Up, Rotation.X);
                //Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);//Matrix.CreateFromAxisAngle(Vector3.Right, Pitch) * Matrix.CreateFromAxisAngle(Vector3.Up, Yaw);


            float distance = Speed * dt;
#if XBOX360
            //Move based on gamepad input.
                MoveForward(Game.GamePadState.ThumbSticks.Left.Y * distance);
                MoveRight(Game.GamePadState.ThumbSticks.Left.X * distance);
                if (Game.GamePadState.IsButtonDown(Buttons.LeftStick))
                    MoveUp(distance);
                if (Game.GamePadState.IsButtonDown(Buttons.RightStick))
                    MoveUp(-distance);
#else
            KeyboardState keyboardState = Keyboard.GetState();
            //Scoot the camera around depending on what keys are pressed.
            if (keyboardState.IsKeyDown(Keys.E))
                MoveForward(distance);
            if (keyboardState.IsKeyDown(Keys.D))
                MoveForward(-distance);
            if (keyboardState.IsKeyDown(Keys.S))
                MoveRight(-distance);
            if (keyboardState.IsKeyDown(Keys.F))
                MoveRight(distance);
            if (keyboardState.IsKeyDown(Keys.A))
                MoveUp(distance);
            if (keyboardState.IsKeyDown(Keys.Z))
                MoveUp(-distance);

            if (keyboardState.IsKeyDown(Keys.P))
                Console.WriteLine("Position: " + Position.X + "f, " + Position.Y + "f, " + Position.Z + "f");

            if (keyboardState.IsKeyDown(Keys.R))
                Console.WriteLine("Rotation: " + Rotation.X + "f, " + Rotation.Y + "f, " + Rotation.Z + "f");

#endif

            WorldMatrix = WorldMatrix * Matrix.CreateTranslation(Position);
            View = Matrix.Invert(WorldMatrix);
            
        }
    }
}
