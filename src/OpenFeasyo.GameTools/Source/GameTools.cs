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
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Reflection;

namespace OpenFeasyo.GameTools
{

    public class KeyTracker {
        private Keys _key;
        private bool state;
        
        public KeyTracker(Keys k, EventHandler handler) {
            _key = k;
            state = false;
            KeyReleased += handler;
        }

        

        
        public void Update(KeyboardState state) {
            if (state.IsKeyDown(_key) && this.state == false) {
                this.state = true;
            }
            if (this.state == true && state.IsKeyUp(_key)) {
                this.state = false;
                OnKeyReleased();
            }
        }

        public event EventHandler KeyReleased;
        private void OnKeyReleased() {
            if (KeyReleased != null) {
                KeyReleased(_key, new EventArgs());
            }
        }
    }

    public static class GameTools
    {
        private static KeyTracker pauseKey = new KeyTracker(
            Keys.Space, (o, i) => { IsPaused = !IsPaused; });

        private static KeyTracker soundsKey = new KeyTracker(
            Keys.M, (o, i) => { Mute = !Mute; });

        public static Texture2D loadTextureFromResource(GraphicsDevice device, Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                return Texture2D.FromStream(device, stream);
            }
        }

        public static void Update(GameTime gametine, MouseState mouse, KeyboardState keyboard) {
            Mouse = mouse;
            Keyboard = keyboard;
            
            pauseKey.Update(keyboard);
            soundsKey.Update(keyboard);
        }

        
        public static MouseState Mouse { get; set; }
         
        public static KeyboardState Keyboard { get; set; }

        public static bool IsPaused { get; set; }
        
        public static bool Mute { get; set; }
    }
}
