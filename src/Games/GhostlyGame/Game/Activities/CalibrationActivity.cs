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

namespace GhostlyLib.Activities
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
        private Label counterLabel;
        private Label instructionLabel;
        private Label infoLabel;
        private double  timer = 0;
        private bool countdown;
        public CalibrationActivity(UIEngine engine, IEmgSensorInput emgInput) : base(engine)
        {
            _emgInput = emgInput;

            Image backgroundImage = new Image(_engine.Content.LoadTexture("textures/ghostly/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);


            counterLabel = new Label("Rest your muscles in", engine.Content.LoadFont("Fonts/Ubuntu" + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            counterLabel.Position = (engine.Screen.ScreenMiddle - counterLabel.Size / 2) - new Vector2(0,counterLabel.Size.Y);
            Components.Add(counterLabel);

            instructionLabel = new Label("5", engine.Content.LoadFont("Fonts/Ubuntu" + GhostlyGame.MENU_BUTTON_FONT_SIZE),GhostlyGame.MENU_FONT_COLOR);
            instructionLabel.Position = (engine.Screen.ScreenMiddle - instructionLabel.Size / 2) + new Vector2(0, instructionLabel.Size.Y);
            Components.Add(instructionLabel);

            infoLabel = new Label("Calibrating ... ", engine.Content.LoadFont("Fonts/Ubuntu" + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            infoLabel.Position = engine.Screen.ScreenMiddle - infoLabel.Size / 2;
            Components.Add(infoLabel);

            _fpsLabel = new Label("Sensor data: - fps", engine.Content.LoadFont("Fonts/Ubuntu12"), GhostlyGame.MENU_FONT_COLOR);
            _fpsLabel.Position = new Vector2(0, engine.Screen.ScreenHeight - _fpsLabel.Size.Y) + new Vector2(10, -10);
            Components.Add(_fpsLabel);

            _emgInput.CalibrationChanged += _emgInput_CalibrationChanged;
            
        }

        private void _emgInput_CalibrationChanged(object sender, CalibrationChangedEventArgs e)
        {
            if (e.CalibrationEvent == CalibrationResults.Finished) {
                // TODO maybe show information about successful calibration first
                _emgInput.MuscleActivationChanged -= _emgInput_MuscleActivationChanged;
                StartActivity(new AdaptCalibrationActivity(_engine,_emgInput));
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(countdown) { 
                if (timer == 0) {
                    timer = gameTime.TotalGameTime.TotalMilliseconds + 5000;
                    counterLabel.Visible = true;
                    instructionLabel.Visible = true;
                    infoLabel.Visible = false;
                }
                else if (timer > gameTime.TotalGameTime.TotalMilliseconds) {
                    // convert the difference to seconds
                    int displayCount = (((int)(timer - gameTime.TotalGameTime.TotalMilliseconds)) / 1000) + 1;
                    instructionLabel.Text = displayCount.ToString();
                } else {
                    countdown = false;
                    counterLabel.Visible = false;
                    instructionLabel.Visible = false;
                    infoLabel.Visible = true;
                    _emgInput.Calibrate();
                }
            }



        }

        public override void OnCreate()
        {
            timer = 0;
            countdown = true;
            _emgInput.MuscleActivationChanged += _emgInput_MuscleActivationChanged;

            base.OnCreate();
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