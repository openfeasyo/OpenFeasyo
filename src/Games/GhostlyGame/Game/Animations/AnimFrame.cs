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

namespace GhostlyLib.Animations
{
    public class AnimFrame
    {
        #region Public members

        public Texture2D Image { get; private set; }
        public long EndTime { get; private set; }

        #endregion Public members

        public AnimFrame(Texture2D image, long endTime)
        {
            this.Image = image;
            this.EndTime = endTime;
        }
    }
}