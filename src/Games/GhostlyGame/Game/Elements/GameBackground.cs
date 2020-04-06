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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.GameTools;
using System.Collections.Generic;

namespace GhostlyLib.Elements
{
    public class GameBackground
    {
        private float x;
        private float[] positions;

        public float HorizontalSpeed { get; set; }
        private float[] speeds;

        private List<Texture2D> ParallaxLayers { get; set; }
        public Texture2D ContinuousLayer { get; set; }

        private Screen _screen;

        public GameBackground(int x, float horizontalSpeed, Screen screen)
        {
            _screen = screen;
            ParallaxLayers = new List<Texture2D>();
            this.x = x;
            this.HorizontalSpeed = horizontalSpeed;
        }

        public void SetParallaxLayers(List<Texture2D> parallaxLayers) {
            ParallaxLayers = parallaxLayers;
            positions = new float[parallaxLayers.Count];
            speeds = new float[parallaxLayers.Count];
            for (int i = 0; i < parallaxLayers.Count; i++) {
                positions[i] = 0;
                speeds[i] = (HorizontalSpeed / (parallaxLayers.Count *3))*(i+1);
            }
        }

        public void Update(GameTime gameTime)
        {
            //this.X += GameScreen.SPEED; //(int) (gameTime.ElapsedGameTime.Milliseconds /3);
            x += (-Screens.GameScreen.SPEED*0.2f + HorizontalSpeed/6);// gameTime.ElapsedGameTime.Milliseconds;
            
            if (this.x <= -ContinuousLayer.Width)
            {
                this.x += ContinuousLayer.Width;
            }
            if(HorizontalSpeed != 0) { 
                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] -= speeds[i];
                    float width = ParallaxLayers[i].Width * (_screen.ScreenHeight / (float)ParallaxLayers[i].Height);
                    if (positions[i] <= -width) {
                        positions[i] += width;
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            //TODO do it for all paralax layers

            for (int i = 0; i < positions.Length; i++)
            {
                Draw(spriteBatch, ParallaxLayers[i], (int)(positions[i]));
            }
            Draw(spriteBatch, ContinuousLayer, (int)x);

            //spriteBatch.Draw(ContinuousLayer, new Rectangle((int)x, 0, ContinuousLayer.Width, _screen.ScreenHeight), Color.White);
            //if (x + ContinuousLayer.Width < _screen.ScreenWidth) {
            //    spriteBatch.Draw(ContinuousLayer, new Rectangle((int)x + ContinuousLayer.Width, 0, ContinuousLayer.Width, _screen.ScreenHeight), Color.White);
            //}
        }

        private void Draw(SpriteBatch spriteBatch, Texture2D texture, int pos) {
            int width =(int) (texture.Width * (_screen.ScreenHeight / (float)texture.Height));
            spriteBatch.Draw(texture, new Rectangle((int)pos, 0, width, _screen.ScreenHeight), Color.White);
            if (pos + width < _screen.ScreenWidth)
            {
                spriteBatch.Draw(texture, new Rectangle((int)(pos + width), 0, width, _screen.ScreenHeight), Color.White);
                spriteBatch.Draw(texture, new Rectangle((int)(pos + width*2), 0, width, _screen.ScreenHeight), Color.White);
            }
        }
    }
}
