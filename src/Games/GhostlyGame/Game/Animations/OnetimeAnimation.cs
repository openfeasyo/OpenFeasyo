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
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GhostlyLib.Animations
{
    public class OnetimeAnimation
    {
        #region Private members
        private List<AnimFrame> _frames;
        private long _animTime, _totalDuration;

        private GameScreen gameScreen;
        #endregion Private members

        #region Public members
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsVisible { get; set; }
        public Texture2D Image { get { return this.GetImage(); } }
        public int Width { get; set; }
        public int Height { get; set; }
        #endregion Public members

        public OnetimeAnimation(int x, int y, int width, int height, List<AnimFrame> frames, GameScreen gameScreen)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;

            this._frames = frames;
            this.gameScreen = gameScreen;

            this._totalDuration = frames.Sum(f => f.EndTime);

            this.IsVisible = true;

            InitTimes();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void InitTimes()
        {
            _animTime = 0;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Update(long elapsedTime)
        {
            this.X += (int)(this.gameScreen.GameBackground.HorizontalSpeed + 2);
            this.Y += -5;

            if (_frames.Count > 1)
            {
                _animTime += elapsedTime;
                if (_animTime >= _totalDuration)
                {
                    this.IsVisible = false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Texture2D GetImage()
        {
            if (_frames.Count == 0)
            {
                return null;
            }
            else
            {
                return GetFrame(_animTime).Image;
            }
        }

        private AnimFrame GetFrame(long time)
        {
            long tmp = 0;
            AnimFrame af = _frames[0];
            for (int i = 0; i < _frames.Count; i++)
            {
                tmp += _frames[i].EndTime;
                if (time < tmp)
                {
                    return af;
                }
                else
                {
                    af = _frames[i];
                }
            }
            return _frames[0];
        }
    }
}
