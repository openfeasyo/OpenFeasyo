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
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using GhostlyLib.Animations;
using OpenFeasyo.GameTools.Effects;

namespace GhostlyLib.Activities
{
    internal class TestCharacter : Image {
        private Texture2D standing;
        private Texture2D jumping;

        private MusicPlayer player;

        private float jumpSpeed = 0;
        private double shootingTimer = 0;

        private Vector2 _position;
        public Vector2 Position {
            get {
                return _position;
            }
            
            set {
                base.Position = value;
                _position = value;
            }
        }
        

        public TestCharacter(MusicPlayer player, Texture2D standing, Texture2D jumping) : base(standing){
            this.standing = standing;
            this.jumping = jumping;
            this.player = player;
        }

        public void Jump()
        {
            if (jumpSpeed == 0) {
                player.PlayEffect("jump");
                jumpSpeed = -500;
                base.Texture = jumping;
            }
        }

        public void Shoot() {
            if(shootingTimer <= 0) {
                shootingTimer = 1;
                player.PlayEffect("shoot");
            }
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(jumpSpeed != 0 ) { 
                base.Position = new Vector2(Position.X, base.Position.Y +jumpSpeed*dt*2);
                jumpSpeed += 700 * dt *2;
            }
            if (base.Position.Y > this.Position.Y) {
                base.Position = this.Position;
                base.Texture = standing;
                jumpSpeed = 0;
            }
            if (shootingTimer > 0) {
                shootingTimer -= dt; 
            }

            base.Update(gameTime);
        }
    }

    public class AdaptCalibrationActivity : OpenFeasyo.GameTools.UI.Activity
    {

        private Label _fpsLabel;
        private IEmgSensorInput _emgInput;

        // FPS measuring related variables
        private DateTime _lastTime = DateTime.Now; // marks the beginning the measurement began
        private int _framesReceived = 0; // an increasing count
        private int _fps = 0;
        
        private Texture2D emgTexture;
        private Image emgImage;
        private static List<double>[] calibrationData = null;
        private static double[] calibrationMean;
        private static double[] calibrationStdDev;
        private TestCharacter character;

        public AdaptCalibrationActivity(UIEngine engine, IEmgSensorInput emgInput) : base(engine)
        {
            _emgInput = emgInput;
            float cell = engine.Screen.ScreenHeight / 16;



            Image backgroundImage = new Image(_engine.Content.LoadTexture("textures/ghostly/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            emgTexture = new Texture2D(engine.Device, 2000, 500);
            Color[] pixelData = new Color[emgTexture.Width * emgTexture.Height];
            emgTexture.GetData<Color>(pixelData);
            for (int i = 0; i < pixelData.Length; i++) {
                pixelData[i] = Color.Transparent;
            }
            emgTexture.SetData<Color>(pixelData);

            emgImage = new Image(emgTexture);
            emgImage.Size = new Vector2((int)(engine.Screen.ScreenWidth*0.6), (int)(engine.Screen.ScreenHeight*0.6));
            emgImage.Position = new Vector2(engine.Screen.ScreenMiddle.X - (emgImage.Size.X/2),cell*2);
            Components.Add(emgImage);

            TextButton jumpingButton = new TextButton("Jumping", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            jumpingButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {  };
            
            jumpingButton.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 2 + emgImage.Size.Y / 4) - jumpingButton.Size / 2;
            Components.Add(jumpingButton);

            TextButton shootingButton = new TextButton("Shooting", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            shootingButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {  };
            shootingButton.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 2 + emgImage.Size.Y/2 + emgImage.Size.Y / 4) - shootingButton.Size / 2;
            Components.Add(shootingButton);

            TextButton nextButton = new TextButton("Next", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            nextButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new MainMenuActivity(_engine)); };
            nextButton.Position = new Vector2(engine.Screen.ScreenMiddle.X + nextButton.Size.X, cell * 14) - nextButton.Size / 2;
            Components.Add(nextButton);

            TextButton recalibrateButton = new TextButton("Recalibrate", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            recalibrateButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new CalibrationActivity(_engine, emgInput)); };
            recalibrateButton.Position = new Vector2(engine.Screen.ScreenMiddle.X- recalibrateButton.Size.X/2, cell * 14) - recalibrateButton.Size / 2;
            Components.Add(recalibrateButton);


            character = new TestCharacter(engine.MusicPlayer,_engine.Content.LoadTexture("Textures\\Ghostly\\character\\walk0001"), ImagesAndAnimations.Instance.CharacterJumping);
            character.Position = new Vector2(engine.Screen.ScreenMiddle.X/5 - (character.Size.X / 2), (int)(engine.Screen.ScreenMiddle.Y *1.2));
            Components.Add(character);

            _fpsLabel = new Label("Sensor data: - fps", engine.Content.LoadFont("Fonts/Ubuntu12"), GhostlyGame.MENU_FONT_COLOR);
            _fpsLabel.Position = new Vector2(0, engine.Screen.ScreenHeight - _fpsLabel.Size.Y) + new Vector2(10, -10);
            Components.Add(_fpsLabel);

            

            emgInput.CalibrationChanged += EmgInput_CalibrationChanged;
        }

        private void EmgInput_CalibrationChanged(object sender, CalibrationChangedEventArgs e)
        {
            if(e.CalibrationEvent == CalibrationResults.Finished) {
                
                calibrationMean = e.ZeroMean;
                calibrationStdDev = e.ZeroStandardDeviation;
                calibrationData = e.CalibrationsData;
                Console.WriteLine("Calibration data " + calibrationData);
                //Color[] pixelData = new Color[emgTexture.Width * emgTexture.Height];
                //emgTexture.GetData<Color>(pixelData);
                //double emgMax = e.ZeroMean[0] * 3 * 2;

                //for (int x = 0; x < Math.Min(pixelData.Length, e.CalibrationsData[0].Count); x++)
                //{
                //    double height = Math.Max(0, Math.Min(e.CalibrationsData[0][x]/emgMax,1))*250;
                //    Console.Write(","+(int)height);
                //    for (int y = 0; y < emgTexture.Height; y++) {

                //        pixelData[y*emgTexture.Width+x] = y<=height ?Color.Blue :Color.Red;
                //    }

                //}
                //emgTexture.SetData<Color>(pixelData);
                //emgImage.Texture = emgTexture;
                //Console.WriteLine("Texture updated");

            }
        }


        private bool emgDrawn = false;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (calibrationData != null && emgDrawn == false) { 
                Color[] pixelData = new Color[emgTexture.Width * emgTexture.Height];
                emgTexture.GetData<Color>(pixelData);
                double emgMax1 = calibrationMean[0] * 3 * 2;
                double emgMax2 = calibrationMean[1] * 3 * 2;

                for (int x = 0; x < Math.Min(pixelData.Length, calibrationData[0].Count); x++)
                {
                    double height1 = 250 - Math.Max(0, Math.Min(calibrationData[0][x] / emgMax1, 1))*250;
                    double height2 = 250 - Math.Max(0, Math.Min(calibrationData[1][x] / emgMax2, 1)) * 250;
                    
                    for (int y = 0; y < emgTexture.Height/2; y++) {
                        pixelData[y*emgTexture.Width+x] = y<=height1 ?Color.Transparent :Color.Red;
                    }

                    for (int y = emgTexture.Height/2; y < emgTexture.Height; y++)
                    {

                        pixelData[y * emgTexture.Width + x] = (y-250) <= height2 ? Color.Transparent : Color.Blue;
                    }

                }
                emgTexture.SetData<Color>(pixelData);
                emgDrawn = true;

            }

            //for (int x = 0; x < Math.Min(pixelData.Length, e.CalibrationsData[0].Count); x++)
            //{
            //    double height = Math.Max(0, Math.Min(e.CalibrationsData[0][x]/emgMax,1))*250;
            //    Console.Write(","+(int)height);
            //    for (int y = 0; y < emgTexture.Height; y++) {

            //        pixelData[y*emgTexture.Width+x] = y<=height ?Color.Blue :Color.Red;
            //    }

        }

        public override void OnCreate()
        {
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
            if (e.EMGSensor != null) {
                if (e.EMGSensor[0].MuscleActivated) {
                    character.Jump();
                }
                else if (e.EMGSensor[1].MuscleActivated) {
                    character.Shoot();
                }

            }

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