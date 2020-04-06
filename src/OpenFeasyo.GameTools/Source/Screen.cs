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

namespace OpenFeasyo.GameTools
{
    public class Screen
    {
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }

        public float TopLeftX { get; set; }
        public float TopLeftY { get; set; }

        public Vector2 ScreenMiddle { get { return new Vector2(ScreenWidth / 2, ScreenHeight / 2); } }

        public Vector2 TopLeft { get { return new Vector2(TopLeftX, TopLeftY); } }

        public Camera Camera { get; set; }

        

        public Screen(int screenWidth, int screenHeight, float w = 3, float h = 2, float tlx = -1.5f, float tly = -1f ) {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            Width = w; 
            Height = h;
            TopLeftX = tlx;
            TopLeftY = tly;
        }

        public float GetScale(float scale) {
            return Math.Min(ScreenHeight/1500f,ScreenWidth/2000f)*scale;
        }

        public Vector2 To32(Vector2 screenSize) 
        {
            return new Vector2(((screenSize.X / ScreenWidth) * Width) + TopLeftX, ((screenSize.Y / ScreenWidth) * Height) + TopLeftY);
        }

        public Rectangle ToScreen(float x, float y, float w, float h) {
            return new Rectangle(   (int)ToScreenX(x), (int)ToScreenY(y),
                                    (int)ToScreenX(w), (int)ToScreenY(h));
        }

        public float ToScreenX(float x){
            return ((x - TopLeftX)/Width) * ScreenWidth;
        }

        public int ToScreenX(int x)
        {
            return (int)ToScreenX((float)x);
        }

        public float ToScreenY(float y)
        {
            return ((y - TopLeftY) / Height) * ScreenHeight;
        }

        public int ToScreenY(int y)
        {
            return (int)ToScreenY((float)y);
        }

        public Vector2 ToScreen(Vector2 size32)
        {
            return new Vector2(ToScreenX(size32.X), ToScreenY(size32.Y));
        }

        public int FontSize
        {
            get
            {
                int h = ScreenHeight;
                int w = ScreenWidth;

                if (h > 1100 && w > 1300)
                    return 4;
                if (h > 800 && w > 1000)
                    return 3;
                if (h > 700 && w > 799)
                    return 2;
                return 1;
            }
        }

    }
}
