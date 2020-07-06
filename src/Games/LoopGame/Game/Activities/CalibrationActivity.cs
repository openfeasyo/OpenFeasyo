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
using OpenFeasyo.GameTools.UI;
using Microsoft.Xna.Framework;
using OpenFeasyo.Platform.Controls;

namespace LoopLib.Activities
{
    public class CalibrationActivity : OpenFeasyo.GameTools.UI.Activity
    {

        private Label _fpsLabel;

        private const int CALIBRATION_TIME_MILLISECONDS = 10000;
        private double _elapsedTime = -1;
        private IEmgSensorInput _emgInput;

        // FPS measuring related variables
        private DateTime _lastTime = DateTime.Now; // marks the beginning the measurement began
        private int _framesReceived = 0; // an increasing count
        private int _fps = 0;


        public CalibrationActivity(UIEngine engine, IEmgSensorInput emgInput) : base(engine)
        {
            _emgInput = emgInput;

            Image backgroundImage = new Image(_engine.Content.LoadTexture("Textures/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            Label l = new Label("Calibrating ... ", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            l.Position = engine.Screen.ScreenMiddle - l.Size / 2;
            Components.Add(l);

            _fpsLabel = new Label("Sensor data: - fps", engine.Content.LoadFont("Fonts/Ubuntu12"), LoopGame.MENU_FONT_COLOR);
            _fpsLabel.Position = new Vector2(0, engine.Screen.ScreenHeight - _fpsLabel.Size.Y) + new Vector2(10, -10);
            Components.Add(_fpsLabel);

            _emgInput.CalibrationChanged += _emgInput_CalibrationChanged;
            
        }

        private void _emgInput_CalibrationChanged(object sender, CalibrationChangedEventArgs e)
        {
            if (e.CalibrationEvent == CalibrationResults.Finished) {
                // TODO maybe show information about successful calibration first
                StartActivity(new MainMenuActivity(_engine));
            }
        }

        public override void OnCreate()
        {
            _emgInput.Calibrate();
            base.OnCreate();

            _emgInput.MuscleActivationChanged += _emgInput_MuscleActivationChanged;
        }

        public override void OnDestroy()
        {
            _emgInput.MuscleActivationChanged -= _emgInput_MuscleActivationChanged;
            base.OnDestroy();
        }

        private void _emgInput_MuscleActivationChanged(object sender, MuscleActivationChangedEventArgs e)
        {
            _framesReceived++;

            if ((DateTime.Now - _lastTime).TotalSeconds >= 1)
            {
                _fps = _framesReceived;
                _framesReceived = 0;
                _lastTime = DateTime.Now;
                _fpsLabel.Text = "Sensor data: " + _fps + " fps";
            }
        }
    }
}