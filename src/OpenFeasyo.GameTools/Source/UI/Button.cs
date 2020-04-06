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
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OpenFeasyo.GameTools.UI
{
    public class TextButton : Component
    {
        private SpriteFont _font;
        private string _text;
        public Color TextColor = Color.White;
        public Color PressedTextColor = Color.FromNonPremultiplied(11, 206, 196, 256);
        public Color Color = Color.FromNonPremultiplied(11, 206, 196, 256);
        public Color PressedColor = Color.White;
        public Color HoveredColor = Color.FromNonPremultiplied(73, 245, 237, 256);

        private Vector2 _fontCentralPoint;
        private Texture2D _background;


        private bool _isPressed = false;
        private bool _isHovered = false;

        public string Text {
            get { return _text; }
            set {
                _text = value;
                UpdateSize();
            }
        }

        public float TextMargin { get; set; }

        public TextButton(string text, SpriteFont font, GraphicsDevice device)
            : base()
        {
            TextMargin = font.MeasureString(text).Y * 0.7f;
            _text = text;
            _font = font;
            UpdateSize();

            _background = new Texture2D(device, 2, 2);
            Color[] data = new Color[2 * 2];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;//FromNonPremultiplied(0,0,0,0);
            _background.SetData(data);
        }

        private void UpdateSize() {
            _fontCentralPoint = _font.MeasureString(_text) / 2;
            Size = new Vector2(Math.Max(_fontCentralPoint.X * 2, 100) + (TextMargin * 2), _fontCentralPoint.Y + (TextMargin * 2));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            spritebatch.Draw(_background, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), _isPressed ? PressedColor : (_isHovered ? HoveredColor : Color));

            Vector2 textPos = Position + (Size / 2 - _fontCentralPoint);
            spritebatch.DrawString(_font, _text, textPos, _isPressed ? PressedTextColor : TextColor);
        }

        public override void OnCursorDown(Vector2 pos)
        {
            _isPressed = true;
        }
        public override void OnCursorUp(Vector2 pos)
        {
            _isPressed = false;
        }
        public override bool OnCursorClick(Vector2 pos)
        {
            OnClicked(new ClickedEventArgs(this, pos));
            return true;
        }

        public override void OnCursorEnter(Vector2 pos)
        {
            OnCursorEntered(new EventArgs());
            _isHovered = true;
        }

        public override void OnCursorLeave(Vector2 pos)
        {
            _isPressed = false;
            _isHovered = false;
        }


        public event EventHandler<EventArgs> CursorEntered;

        protected virtual void OnCursorEntered(EventArgs e)
        {
            if (CursorEntered != null)
            {
                CursorEntered(this, e);
            }
        }

        public event EventHandler<ClickedEventArgs> Clicked;

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

            public float TextMargin { get; set; }

            private Component _component;
            public Component Component { 
                get {
                    return _component;
                }
            }
        
    	    
        }
    }
}
