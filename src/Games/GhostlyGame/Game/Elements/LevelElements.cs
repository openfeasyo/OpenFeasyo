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
using GhostlyLib.Elements.Enemies;
using GhostlyLib.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GhostlyLib.Elements
{
    public class LevelElements
    {
        #region Private members

        private DrawingEngine _engine;
        private List<Drawable> _elements;

        private List<Drawable> _elementsToAdd = new List<Drawable>();
        private List<Drawable> _elementsToRemove = new List<Drawable>();

        private List<Drawable> _enemiesToAdd = new List<Drawable>();
        private List<Drawable> _enemiesToRemove = new List<Drawable>();

        private List<Drawable> _tilesToAdd = new List<Drawable>();
        private List<Drawable> _tilesToRemove = new List<Drawable>();

        #endregion Private members

        public List<Drawable> Enemies { get; private set; }
        public List<Drawable> Tiles { get; private set; }

        public LevelElements()
        {
            this._engine = new DrawingEngine();
            this._elements = new List<Drawable>();

            this.Enemies = new List<Drawable>();
            this.Tiles = new List<Drawable>();
        }

        public void AddElement(Drawable drawable)
        {
            if (drawable is Enemy)
            {
                this._enemiesToAdd.Add(drawable);
            }
            else if (drawable is Tile)
            {
                this._tilesToAdd.Add(drawable);
            }
            else
            {
                this._elementsToAdd.Add(drawable);
            }

            this._engine.AddDrawable(drawable);
        }

        public void RemoveElement(Drawable drawable)
        {
            if (drawable is Enemy)
            {
                this._enemiesToRemove.Add(drawable);
            }
            else if (drawable is Tile)
            {
                this._tilesToRemove.Add(drawable);
            }
            else
            {
                this._elementsToRemove.Add(drawable);
            }

            this._engine.RemoveDrawable(drawable);
        }

        public void Update(GameTime gameTime)
        {
            Enemies.AddRange(_enemiesToAdd);
            _enemiesToAdd.Clear();

            _elements.AddRange(_elementsToAdd);
            _elementsToAdd.Clear();

            Tiles.AddRange(_tilesToAdd);
            _tilesToAdd.Clear();

            _engine.Update(gameTime);


            foreach (Drawable d in _enemiesToRemove)
            {
                Enemies.Remove(d);
            }
            _enemiesToRemove.Clear();

            foreach (Drawable d in _elementsToRemove)
            {
                _elements.Remove(d);
            }
            _elementsToRemove.Clear();

            foreach (Drawable d in _tilesToRemove)
            {
                Tiles.Remove(d);
            }
            _tilesToRemove.Clear();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _engine.Draw(spriteBatch);
        }
    }
}
