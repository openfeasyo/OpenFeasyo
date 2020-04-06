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
using Microsoft.Xna.Framework.Graphics;

namespace OpenFeasyo.GameTools.UI
{
    public interface IComponent
    {
        void Draw(GameTime gameTime, SpriteBatch spritebatch);
        void Update(GameTime gameTime);

        void OnCursorDown(Vector2 pos);
        void OnCursorUp(Vector2 pos);
        bool OnCursorClick(Vector2 pos);
        void OnCursorEnter(Vector2 pos);
        void OnCursorMove(Vector2 oldPos, Vector2 newPos);
        void OnCursorLeave(Vector2 pos);
        
        void OnResizeGesture(float oldSize, float newSize);
        void OnMoveGesture(Vector2 moveVec);

        Vector2 Size { get; set; }

        Vector2 Position { get; set; }

        bool Hidden { get; set; }
        
        bool Visible { get; set; }
    }

    public abstract class Component
    {
        public Component()
        {
            Size = new Vector2(0,0);
            Position = new Vector2(0, 0);
            Hidden = false;
        }

        public virtual void OnCursorDown(Vector2 pos) {}
        public virtual void OnCursorUp(Vector2 pos) {}
        public virtual bool OnCursorClick(Vector2 pos) { return false; }
        public virtual void OnCursorEnter(Vector2 pos) { }
        public virtual void OnCursorMove(Vector2 oldPos, Vector2 newPos) {}
        public virtual void OnCursorLeave(Vector2 pos) { }
        
        public virtual void OnResizeGesture(float oldSize, float newSize) { }
        public virtual void OnMoveGesture(Vector2 moveVec) { }

        public virtual void Draw(GameTime gameTime, SpriteBatch spritebatch) { }
        public virtual void Update(GameTime gameTime) { }

        public Vector2 Size { get; set; }
         
        public Vector2 Position { get; set; }

        public bool Hidden { get; set; }
        public bool Visible {
            get { return !Hidden; }
            set { Hidden = !value; }
        }

        public virtual bool IsIn(Vector2 pos) {
            return pos.X > Position.X &&
                   pos.X < (Position.X + Size.X) &&
                   pos.Y > Position.Y &&
                   pos.Y < (Position.Y + Size.Y);
        }
    }
}
