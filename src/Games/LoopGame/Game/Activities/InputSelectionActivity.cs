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
#if ANDROID
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using OpenFeasyo.GameTools.UI;
using System;

namespace LoopLib.Activities
{
    public class InputSelectionActivity : OpenFeasyo.GameTools.UI.Activity
    {
        private bool? _permissionsGranted = null;

        public InputSelectionActivity(UIEngine engine) : base(engine)
        {
            float cell = engine.Screen.ScreenHeight / 16;

            Image backgroundImage = new Image(_engine.Content.LoadTexture("Textures/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            Label questionLabel = new Label("I want to control the game using", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            questionLabel.Position = engine.Screen.ScreenMiddle - questionLabel.Size / 2 - new Vector2(0, 200);

            Label orLabel = new Label("or", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            orLabel.Position = engine.Screen.ScreenMiddle - orLabel.Size / 2 - new Vector2(0, 0);

            TextButton touchButton = new TextButton("Touch", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            touchButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new ConfigureTouchActivity(engine)); };
            touchButton.CursorEntered += (object sender, EventArgs e) => { engine.MusicPlayer.PlayEffect("hover"); };
            touchButton.Position = engine.Screen.ScreenMiddle - touchButton.Size/2 - new Vector2(engine.Screen.ScreenMiddle.X/2, 0);

            TextButton emgButton = new TextButton("EMG", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            emgButton.Clicked += EmgButton_Clicked;
            emgButton.CursorEntered += (object sender, EventArgs e) => { engine.MusicPlayer.PlayEffect("hover"); };
            emgButton.Position = engine.Screen.ScreenMiddle - emgButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

            Image vubetrologoImage = new Image(_engine.Content.LoadTexture("textures/vubetrologo"));
            vubetrologoImage.Size = new Vector2((vubetrologoImage.Size.X / vubetrologoImage.Size.Y) * cell * 2, cell * 2);
            vubetrologoImage.Position = engine.Screen.ScreenMiddle + new Vector2(-vubetrologoImage.Size.X / 2, engine.Screen.ScreenMiddle.Y * 2 / 3 - vubetrologoImage.Size.Y / 2);

            Components.Add(vubetrologoImage);
            Components.Add(questionLabel);
            Components.Add(orLabel);
            Components.Add(touchButton);
            Components.Add(emgButton);

        }

        

        private void EmgButton_Clicked(object sender, TextButton.ClickedEventArgs e)
        {
#if ANDROID
            MainActivity.Instance.RequestPermissions(new[] {
                        Manifest.Permission.AccessCoarseLocation,
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.BluetoothAdmin}, 3, granted => { _permissionsGranted = granted; });
#endif
        }

        public override void Update(GameTime gameTime)
        {
            if (_permissionsGranted.HasValue) {
                if (_permissionsGranted.Value)
                {
                    StartActivity(new SelectSensorActivity(_engine));
                }
                else {
                    // TODO room for improvement
                    _permissionsGranted = null;
                }

            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _engine.StartActivity(null);
            }
            base.Update(gameTime);
        }
        


    }
}
