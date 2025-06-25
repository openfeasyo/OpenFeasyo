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
        private List<IDrawable> _elements;

        private List<IDrawable> _elementsToAdd = new List<IDrawable>();
        private List<IDrawable> _elementsToRemove = new List<IDrawable>();

        private List<IDrawable> _enemiesToAdd = new List<IDrawable>();
        private List<IDrawable> _enemiesToRemove = new List<IDrawable>();

        private List<IDrawable> _tilesToAdd = new List<IDrawable>();
        private List<IDrawable> _tilesToRemove = new List<IDrawable>();

        #endregion Private members

        public List<IDrawable> Enemies { get; private set; }
        public List<IDrawable> Tiles { get; private set; }

        public LevelElements()
        {
            this._engine = new DrawingEngine();
            this._elements = new List<IDrawable>();

            this.Enemies = new List<IDrawable>();
            this.Tiles = new List<IDrawable>();
        }

        public void AddElement(IDrawable drawable)
        {
            if (drawable is IEnemy)
            {
                this._enemiesToAdd.Add(drawable);
            }
            else if (drawable is ITile)
            {
                this._tilesToAdd.Add(drawable);
            }
            else
            {
                this._elementsToAdd.Add(drawable);
            }

            this._engine.AddDrawable(drawable);
        }

        public void RemoveElement(IDrawable drawable)
        {
            if (drawable is IEnemy)
            {
                this._enemiesToRemove.Add(drawable);
            }
            else if (drawable is ITile)
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

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _engine.Draw(spriteBatch, gameTime);
        }
    }
}
