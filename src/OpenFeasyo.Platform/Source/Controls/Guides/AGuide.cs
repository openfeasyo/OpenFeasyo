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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using OpenFeasyo.Platform.Controls.Reports;

namespace OpenFeasyo.Platform.Controls.Guides
{
    public abstract class AGuide : IGuide
    {
        #region PublicFields
        protected bool hidden;
        public bool Hidden
        {
            get { return hidden; }
            set { hidden = value; }
        }
        protected Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                customViewPort.X = (int)position.X;
                customViewPort.Y = (int)position.Y;
            }
        }
        protected Vector2 size;
        public Vector2 Size
        {
            get { return size; }
            set
            {
                if (value != null && size != value)
                {
                    size = value;
                    OnResize();
                }
            }
        }
        protected Color background;
        public Color Background
        {
            get { return background; }
            set { background = value; }
        }
        private bool activeZoom;
        public bool ActiveZoom
        {
            get { return activeZoom; }
            set { activeZoom = value; }
        }
        private bool visualFeedback;
        public bool VisualFeedback
        {
            get { return visualFeedback; }
            set { visualFeedback = value; }
        }
        #endregion

        #region protectedFields
        protected GraphicsDevice device;
        protected Viewport customViewPort;
        protected ICamera camera;
        protected IReport report;
        #endregion

        #region Constructors
        public AGuide(GraphicsDevice _device, ContentManager content)
        {
            this.device = _device;
            this.customViewPort = _device.Viewport;
            this.Size = new Vector2(100, 100);
            this.Position = new Vector2(0, 0);
            this.background = Color.Black;
            this.activeZoom = true;
            this.visualFeedback = true;
        }
        #endregion

        #region Methods
        public virtual void Update(GameTime gametime) { }

        public virtual void Draw(GameTime gametime)
        {
            if (!Hidden)
            {
                Texture2D _background = new Texture2D(device, customViewPort.Width, customViewPort.Height);
                Color[] data = new Color[customViewPort.Width * customViewPort.Height];

                for (int i = 0; i < data.Length; ++i) data[i] = background;
                _background.SetData(data);

                SpriteBatch batch = new SpriteBatch(device);
                batch.Begin();
                batch.Draw(_background, new Rectangle(customViewPort.X, customViewPort.Y, customViewPort.Width, customViewPort.Height), background);
                batch.End();
            }
        }

        public virtual void OnReport(IReport _report)
        {
            lock (_report)
            {
                this.report = _report.Copy();
            }

            //this.hidden = !report.IsReportingProblem; //for purpose test we let the guide visible
        }

        protected virtual void OnResize()
        {
            customViewPort.Width = (int)size.X;
            customViewPort.Height = (int)size.Y;
            if (camera != null)
            {
                camera.Distance = Math.Min(size.X, size.Y);
                camera.Angle = 0;
                camera.Height = camera.Distance / 6.25f;
                camera.TargetPosition = new Vector3(camera.Distance / 13, camera.Distance / 6.25f, 0);
                PerspectiveCamera.SceneProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, size.X / size.Y, 1, 10000);
            }
        }
        #endregion
    }
}
