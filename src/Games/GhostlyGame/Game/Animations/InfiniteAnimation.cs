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
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GhostlyLib.Animations
{
    public class InfiniteAnimation
    {
        #region private members

        private List<AnimFrame> _frames;
        private long _animTime;
        private long _totalDuration;

        #endregion private members

        public InfiniteAnimation(List<AnimFrame> frames)
        {
            _frames = frames;
            _totalDuration = _frames.Sum(item => item.EndTime);

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
            if (_frames.Count > 1)
            {
                _animTime += elapsedTime;
                if (_animTime >= _totalDuration)
                {
                    _animTime = _animTime % _totalDuration;
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