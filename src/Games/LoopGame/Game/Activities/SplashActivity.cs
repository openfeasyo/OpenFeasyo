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
using Microsoft.Xna.Framework.Input;
using OpenFeasyo.GameTools.UI;
using System;

namespace LoopLib.Activities
{
    class SplashActivity : OpenFeasyo.GameTools.UI.Activity
    {
        public SplashActivity(UIEngine engine) : base(engine)
        {

            int headerSize = new int[] { 12, 24, 36, 48, 64 }[Math.Max(engine.Screen.FontSize - 1, 0)];
            int fontSize = new int[] { 12, 24, 36, 48, 64 }[Math.Max(engine.Screen.FontSize - 2, 0)];

            float cell = engine.Screen.ScreenHeight / 16;

            Image backgroundImage = new Image(_engine.Content.LoadTexture("Textures/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            Label infoLabel = new Label("Loop", engine.Content.LoadFont("Fonts/PerfectDark128"), Color.FromNonPremultiplied(11, 206, 196, 256));
            infoLabel.Position = engine.Screen.ScreenMiddle - infoLabel.Size / 2 - new Vector2(0, engine.Screen.ScreenMiddle.Y / 2);

            Label info2Label = new Label("Application is intended for exploratory data acquisition and ", engine.Content.LoadFont("Fonts/Ubuntu" + fontSize), Color.DarkRed/*GhostlyGame.MENU_FONT_COLOR*/);
            info2Label.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 8.5f) - info2Label.Size / 2;

            Label info3Label = new Label("feedback purposes, and is not intended for clinical/diagnostic use.", engine.Content.LoadFont("Fonts/Ubuntu" + fontSize), Color.DarkRed);
            info3Label.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 9.0f) - info3Label.Size / 2;

            Label info4Label = new Label("Application is not to be used for commercial distribution.", engine.Content.LoadFont("Fonts/Ubuntu" + fontSize), Color.DarkRed);
            info4Label.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 10) - info4Label.Size / 2;



            TextButton next = new TextButton("Next", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT+ LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            next.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                StartActivity(new InputSelectionActivity(engine));
            };
            next.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 12) - next.Size / 2;

            Image vubetrologoImage = new Image(_engine.Content.LoadTexture("Textures/vubetrologo"));
            vubetrologoImage.Size = new Vector2((vubetrologoImage.Size.X / vubetrologoImage.Size.Y) * cell * 2, cell * 2);
            vubetrologoImage.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 14) - new Vector2(vubetrologoImage.Size.X / 2, vubetrologoImage.Size.Y / 2);

            Components.Add(next);
            Components.Add(infoLabel);
            Components.Add(info2Label);
            Components.Add(info3Label);
            Components.Add(info4Label);
            Components.Add(vubetrologoImage);
        }

        private bool _musicOn = false; 
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _engine.StartActivity(null);
            }
            if (!_musicOn) {
                _engine.MusicPlayer.Play("menu");
                _musicOn = true;
            }
        }
    }
}
