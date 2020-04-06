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
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using OpenFeasyo.Platform.Controls.Reports;

namespace OpenFeasyo.Platform.Controls.Guides
{

    public abstract class ASkeletonGuide : AGuide
    {
        #region properties
        #endregion

        #region constructors
        public ASkeletonGuide(GraphicsDevice _device, ContentManager content)
            : base(_device, content)
        {
            if (camera != null)
            {
                camera.Distance = Math.Min(size.X, size.Y);
                camera.Angle = 0;
                camera.Height = camera.Distance / 6.25f;
                camera.TargetPosition = new Vector3(camera.Distance / 13, camera.Distance / 6.25f, 0);
            }
        }
        #endregion

        #region methods
        public override void Draw(GameTime gametime)
        {
            base.Draw(gametime);

            if (!Hidden)
            {
                Viewport originalViewPort = device.Viewport;
                device.Viewport = customViewPort;
                device.Viewport = originalViewPort;
            }
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
        }

        public override void OnReport(IReport _report)
        {
            base.OnReport(_report);
        }

        protected override void OnResize()
        {
            base.OnResize();
        }
        #endregion
    }


    //enum of the views available for the SkeletonGuide Component
    public enum CameraView
    {
        Front = 0,
        FrontUp = 1,
        FrontRight = 2,
        FrontLeft = 3,
        FrontRightUp = 4,
        FrontLeftUp = 5,
        Back = 6,
        BackUp = 7,
        BackRight = 8,
        BackLeft = 9,
        BackRightUp = 10,
        BackLeftUp = 11,
        Right = 12,
        Left = 13,
        RightUp = 14,
        LeftUp = 15,
    }
}
