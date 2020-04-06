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

namespace GhostlyLib.Drawing
{
    public class DrawingEngine
    {
        #region Private members

        private List<Drawable> _drawables;
        private List<Drawable> _drawablesToAdd;
        private List<Drawable> _drawablesToRemove;

        #endregion Private members

        public DrawingEngine()
        {
            _drawables = new List<Drawable>();
            _drawablesToAdd = new List<Drawable>();
            _drawablesToRemove = new List<Drawable>();
        }

        public void AddDrawable(Drawable d)
        {
            this._drawablesToAdd.Add(d);
        }

        public void RemoveDrawable(Drawable d)
        {
            this._drawablesToRemove.Add(d);
        }

        public void Update(GameTime gameTime)
        {
            _drawables.AddRange(_drawablesToAdd);
            _drawablesToAdd.Clear();

            foreach (Drawable d in _drawables)
            {
                d.Update(gameTime);
            }

            foreach (Drawable d in _drawablesToRemove)
            {
                _drawables.Remove(d);
            }
            _drawablesToRemove.Clear();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Drawable d in _drawables)
            {
                d.Draw(spriteBatch);
            }
        }
    }
}
