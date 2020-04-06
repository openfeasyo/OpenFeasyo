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
using GhostlyLib.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GhostlyLib.Animations
{
    public class CharacterAnimation
    {
        #region Private members
        private List<AnimFrame> _normalFrames;
        private List<AnimFrame> _hitFrames;
        private List<AnimFrame> _currentFrames;

        private long _currentAnimTime;
        private long _normalTotalDuration, _hitTotalDuration, _currentFramesTotalDuration;
        #endregion Private members

        public CharacterAnimation(List<AnimFrame> normalFrames, List<AnimFrame> hitFrames)
        {
            _normalFrames = normalFrames;
            _hitFrames = hitFrames;

            _normalTotalDuration = normalFrames.Sum(item => item.EndTime);
            _hitTotalDuration = hitFrames.Sum(item => item.EndTime);

            InitTimes();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void InitTimes()
        {
            _currentAnimTime = 0;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetCurrentFrames(CharacterLiveState liveState)//CharacterState enemyState)
        {
            switch (liveState)
            {
                case CharacterLiveState.Normal:
                    _currentFrames = _normalFrames;
                    _currentAnimTime = 0;
                    _currentFramesTotalDuration = _normalTotalDuration;
                    break;
                case CharacterLiveState.Hit:
                    _currentFrames = _hitFrames;
                    _currentAnimTime = 0;
                    _currentFramesTotalDuration = _hitTotalDuration;
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Update(GameTime gameTime)
        {
            if (_currentFrames != null && _currentFrames.Count > 1)
            {
                _currentAnimTime += gameTime.ElapsedGameTime.Milliseconds;
                if (_currentAnimTime >= _currentFramesTotalDuration)
                {
                    _currentAnimTime = _currentAnimTime % _currentFramesTotalDuration;
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Texture2D GetImage()
        {
            if (_currentFrames.Count == 0)
            {
                return null;
            }
            else
            {
                return GetFrame(_currentAnimTime).Image;
            }
        }

        private AnimFrame GetFrame(long time)
        {
            long tmp = 0;
            AnimFrame af = _currentFrames[0];
            for (int i = 0; i < _currentFrames.Count; i++)
            {
                tmp += _currentFrames[i].EndTime;
                if (time < tmp)
                {
                    return af;
                }
                else
                {
                    af = _currentFrames[i];
                }
            }
            return _currentFrames[0];
        }
    }
}