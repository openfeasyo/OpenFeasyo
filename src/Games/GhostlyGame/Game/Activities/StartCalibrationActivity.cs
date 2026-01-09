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
using GhostlyGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OpenFeasyo.GameTools.UI;
using OpenFeasyo.Platform.Controls;
using System;

namespace GhostlyLib.Activities
{
    public class StartCalibrationActivity : OpenFeasyo.GameTools.UI.Activity
    {
        private Label _fpsLabel;

        // FPS measuring related variables
        private DateTime _lastTime = DateTime.Now; // marks the beginning the measurement began
        private int _framesReceived = 0; // an increasing count
        private int _fps = 0;

        private IEmgSensorInput _emgInput;
        public StartCalibrationActivity(UIEngine engine, IEmgSensorInput emgInput) : base(engine)
        {
            _emgInput = emgInput;

            Image backgroundImage = new Image(_engine.Content.LoadTexture("textures/ghostly/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            Label infoLabel1 = new Label(LocalizationResourceManager.Instance["RestYourMuscles"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_STANDARD_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            infoLabel1.Position = engine.Screen.ScreenMiddle - infoLabel1.Size / 2 - new Vector2(0, 150);
            Components.Add(infoLabel1);

            Label infoLabel2 = new Label(LocalizationResourceManager.Instance["ClickOnTheButtonToStartTheCalibration"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_STANDARD_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            infoLabel2.Position = engine.Screen.ScreenMiddle - infoLabel2.Size / 2 - new Vector2(0, 75);
            Components.Add(infoLabel2);

            _fpsLabel = new Label(LocalizationResourceManager.Instance["SensorData"].ToString() + ": - fps", engine.Content.LoadFont(GhostlyGame.MENU_STANDARD_FONT + GhostlyGame.MENU_SMALL_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            _fpsLabel.Position = new Vector2(0,engine.Screen.ScreenHeight - _fpsLabel.Size.Y) + new Vector2(10, -10);
            Components.Add(_fpsLabel);

            TextButton s1 = new TextButton(LocalizationResourceManager.Instance["StartCalibration"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_STANDARD_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            s1.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                StartActivity(new CalibrationActivity(engine, emgInput));
            };
            s1.Position = engine.Screen.ScreenMiddle - s1.Size / 2 + new Vector2(0, 100);
            Components.Add(s1);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _engine.StartActivity(null);
            }
        }
        public override void OnCreate()
        {
            base.OnCreate();
            _emgInput.MuscleActivationChanged += _emgInput_MuscleActivationChanged;
        }

        private void _emgInput_MuscleActivationChanged(object sender, MuscleActivationChangedEventArgs e)
        {
            _framesReceived++;

            if ((DateTime.Now - _lastTime).TotalSeconds >= 1)
            {
                _fps = _framesReceived;
                _framesReceived = 0;
                _lastTime = DateTime.Now;
                _fpsLabel.Text = LocalizationResourceManager.Instance["SensorData"].ToString() + ": " + _fps + " fps";
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _emgInput.MuscleActivationChanged -= _emgInput_MuscleActivationChanged;
        }
    }
}