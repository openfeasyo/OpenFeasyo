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
using System;
using System.Collections.Generic;

namespace OpenFeasyo.GameTools.UI
{
    class ImageSwitch<T> : Component where T : struct, IConvertible
    {
        
        private List<Texture2D> _options;
        private Texture2D _currentOption;
        

        private Texture2D _background;
        private Color _color = Color.LightGray;
        

        private bool _isInteractive;
        private bool _isHovered = false;
        

        private T _state;
        public T State {
            get { return _state; }
        }

        public ImageSwitch(List<Texture2D> options, T defaultOpt, GraphicsDevice device, bool interactive = true)
            : base()
        {
            _options = options;
            _currentOption = options[0];
            _isInteractive = interactive;
            _background = new Texture2D(device, 1, 1);
            Color[] data = new Color[1] { _color };
            _background.SetData(data);
            _state = defaultOpt;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            spritebatch.Draw(_background, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), _isHovered && _isInteractive ? Color.Red : Color.White);
            spritebatch.Draw(_currentOption, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color.White );
        }

        public override void OnCursorDown(Vector2 pos)
        {
            
        }
        public override void OnCursorUp(Vector2 pos)
        {
            
        }
        public override bool OnCursorClick(Vector2 pos)
        {
            if (_isInteractive) {
                Next();
            }
            OnClicked(new ClickedEventArgs(this, pos));
            return true;
        }

        public override void OnCursorEnter(Vector2 pos)
        {
            _isHovered = true;
        }

        public override void OnCursorLeave(Vector2 pos)
        {
            _isHovered = false;
        }


        public event EventHandler<ClickedEventArgs> Clicked;

        public void SwitchTo(T state)
        {
            if (typeof(T).IsEnum)
            {
                T[] items = (T[])Enum.GetValues(typeof(T));
                int id = Array.IndexOf(items, state);
                _state = items[id];
                _currentOption = _options[id];
            }
        }

        public void Next() 
        {
            if (typeof(T).IsEnum)
            {
                T[] items = (T[])Enum.GetValues(typeof(T));
                int id = (Array.IndexOf(items, _state) + 1) % items.Length;
                _state = items[id];
                _currentOption = _options[id];
            }
        }

        protected virtual void OnClicked(ClickedEventArgs e)
        {
            if (Clicked != null)
            {
                Clicked(this, e);
            }
        }

        public class ClickedEventArgs : EventArgs
        {
    	    public ClickedEventArgs(Component component, Vector2 position)
    	    {
    		    _component = component;
                _position = position;
    	    }

            private Vector2 _position;
            public Vector2 Position {
                get {
                    return _position;
                }
            }

            private Component _component;
            public Component Component { 
                get {
                    return _component;
                }
            }
        
    	    
        }
    }
}
