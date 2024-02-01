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


        public TestCharacter(MusicPlayer player, Texture2D standing, Texture2D jumping) : base(standing) {
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
            if (shootingTimer <= 0) {
                shootingTimer = 1;
                player.PlayEffect("shoot");
            }
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (jumpSpeed != 0) {
                base.Position = new Vector2(Position.X, base.Position.Y + jumpSpeed * dt * 2);
                jumpSpeed += 700 * dt * 2;
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

    public class DraggableButton : TextButton {

        public delegate void ValueChanged(float newValue);

        public DraggableButton(string text, SpriteFont font, GraphicsDevice device) : base(text, font, device)
        {
            percentage = 0.75f;
        }

        public float MaxY { get; set; }
        public float MinY { get; set; }

        private float percentage;
        public float Percentage {
            get { return percentage; }
            private set {
                if (percentage != value) { 
                    percentage = value;
                    OnPercentageChanged(value);
                }

            }
        }

        public event ValueChanged PercentageChanged;
        protected virtual void OnPercentageChanged(float value) 
        {
            PercentageChanged?.Invoke(value);
        }

        private Vector2 offset;
        private bool isDown = false;

        public override void OnCursorDown(Vector2 pos)
        {
            base.OnCursorDown(pos);
            offset = pos - (Position + Size / 2);
            isDown = true;
        }
        public override void OnCursorUp(Vector2 pos)
        {
            isDown = false;
            base.OnCursorUp(pos);
        }
        public override void OnCursorLeave(Vector2 pos)
        {
            base.OnCursorLeave(pos);
            isDown = false;
        }
        public override void OnCursorMove(Vector2 oldPos, Vector2 newPos)
        {
            base.OnCursorMove(oldPos, newPos);
            if (isDown) {
                float newY = newPos.Y - offset.Y;
                newY = Math.Min(newY, MaxY);
                newY = Math.Max(newY, MinY);
                Percentage = (1 - ((newY - MinY) / (MaxY - MinY)));
                //Console.WriteLine("Percentage: " + (1-((newY-MinY)/(MaxY-MinY)))*100 );
                Position = new Vector2(Position.X, newY - Size.Y / 2);

            }
        }

    }

    public class EmgImage: Image {

        private Color[] pixelData;

        private float percentage;
        public float Percentage {
            get { return percentage; }
            set {
                percentage = value;
                for (int i = 0; i < pixelData.Length; i++)
                {
                    
                    pixelData[i] = i < ((1-percentage)*100) ? Color.LightBlue: Color.Blue;
                }
                Texture.SetData<Color>(pixelData);
            }

        }

        public EmgImage(GraphicsDevice device, float percentage) : base(new Texture2D(device, 1, 100))  {
            pixelData = new Color[Texture.Width * Texture.Height];
            Texture.GetData<Color>(pixelData);
            Percentage = percentage;
        }
    }

    public class AdaptCalibrationActivity : OpenFeasyo.GameTools.UI.Activity
    {
        private Label percentageJumpingLabel;
        private Label percentageShootingLabel;

        private DraggableButton jumpingButton;
        private DraggableButton shootingButton;

        private Label _fpsLabel;
        private IEmgSensorInput _emgInput;

        // FPS measuring related variables
        private DateTime _lastTime = DateTime.Now; // marks the beginning the measurement began
        private int _framesReceived = 0; // an increasing count
        private int _fps = 0;
        
        private Texture2D emgTexture1;
        private Texture2D emgTexture2;
        private EmgImage emgImage1;
        private EmgImage emgImage2;
        private static List<double>[] calibrationData = null;
        private static double[] calibrationMean;
        private static double[] calibrationStdDev;
        private TestCharacter character;

        public AdaptCalibrationActivity(UIEngine engine, IEmgSensorInput emgInput) : base(engine)
        {
            _emgInput = emgInput;
            float cell = engine.Screen.ScreenHeight / 16;


            Label infoLabel = new Label("Contract muscles maximally 3 times", engine.Content.LoadFont("Fonts/Ubuntu" + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            infoLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X - (infoLabel.Size.X / 2), engine.Screen.ScreenHeight * 0.05f - (GhostlyGame.MENU_BUTTON_FONT_SIZE / 2));
            Components.Add(infoLabel);

            Image backgroundImage = new Image(_engine.Content.LoadTexture("textures/ghostly/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            emgTexture1 = new Texture2D(engine.Device,100, 1);
            emgTexture2 = new Texture2D(engine.Device, 100, 1);
            
            Color[] pixelData = new Color[emgTexture1.Width * emgTexture1.Height];
            emgTexture1.GetData<Color>(pixelData);
            for (int i = 0; i < pixelData.Length; i++) {
                pixelData[i] = Color.Blue;
            }
            emgTexture1.SetData<Color>(pixelData);

            pixelData = new Color[emgTexture2.Width * emgTexture2.Height];
            emgTexture2.GetData<Color>(pixelData);
            for (int i = 0; i < pixelData.Length; i++)
            {
                pixelData[i] = Color.Red;
            }
            emgTexture2.SetData<Color>(pixelData);


            emgImage1 = new EmgImage(engine.Device,0.2f);
            emgImage1.Size = new Vector2((int)(engine.Screen.ScreenWidth*0.1), (int)(engine.Screen.ScreenHeight*0.6));
            emgImage1.Position = new Vector2((int)(engine.Screen.ScreenWidth*0.5) - (emgImage1.Size.X/2),cell*2);
            Components.Add(emgImage1);

            emgImage2 = new EmgImage(engine.Device,0.2f);
            emgImage2.Size = new Vector2((int)(engine.Screen.ScreenWidth * 0.1), (int)(engine.Screen.ScreenHeight * 0.6));
            emgImage2.Position = new Vector2((int)(engine.Screen.ScreenWidth*0.8) - (emgImage2.Size.X / 2), cell * 2);
            Components.Add(emgImage2);

            percentageJumpingLabel = new Label("75%  ", engine.Content.LoadFont("Fonts/Ubuntu" + GhostlyGame.MENU_BUTTON_FONT_SIZE), Color.White);
            percentageJumpingLabel.Position = emgImage1.Position + emgImage1.Size - percentageJumpingLabel.Size;
            Components.Add(percentageJumpingLabel);

            percentageShootingLabel = new Label("75%  ", engine.Content.LoadFont("Fonts/Ubuntu" + GhostlyGame.MENU_BUTTON_FONT_SIZE), Color.White);
            percentageShootingLabel.Position = emgImage2.Position + emgImage2.Size - percentageShootingLabel.Size;
            Components.Add(percentageShootingLabel);


            jumpingButton = new DraggableButton("Jumping", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            jumpingButton.MinY = emgImage1.Position.Y;
            jumpingButton.MaxY = emgImage1.Position.Y + emgImage1.Size.Y;
            jumpingButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {  };
            jumpingButton.Position = new Vector2((int)(engine.Screen.ScreenWidth * 0.5), cell * 2 + emgImage1.Size.Y / 4) - jumpingButton.Size / 2;
            jumpingButton.PercentageChanged += (float value) => { percentageJumpingLabel.Text = (value * 100).ToString("00.") + "%"; };
            Components.Add(jumpingButton);

            shootingButton = new DraggableButton("Shooting", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            shootingButton.MinY = emgImage2.Position.Y;
            shootingButton.MaxY = emgImage2.Position.Y + emgImage2.Size.Y;
            shootingButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {  };
            shootingButton.Position = new Vector2((int)(engine.Screen.ScreenWidth * 0.8) , cell * 2 + emgImage2.Size.Y / 4) - shootingButton.Size / 2;
            shootingButton.PercentageChanged += (float value) => { percentageShootingLabel.Text = (value * 100).ToString("00.") + "%" ; };
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
            emgInput.MuscleActivationChanged += _emgInput_MuscleActivationChanged;
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
                //Color[] pixelData = new Color[emgTexture.Width * emgTexture.Height];
                //emgTexture.GetData<Color>(pixelData);
                //double emgMax1 = calibrationMean[0] * 3 * 2;
                //double emgMax2 = calibrationMean[1] * 3 * 2;

                //for (int x = 0; x < Math.Min(pixelData.Length, calibrationData[0].Count); x++)
               // {
                 //   double height1 = 250 - Math.Max(0, Math.Min(calibrationData[0][x] / emgMax1, 1))*250;
                 //   double height2 = 250 - Math.Max(0, Math.Min(calibrationData[1][x] / emgMax2, 1)) * 250;
                    
                 //   for (int y = 0; y < emgTexture.Height/2; y++) {
                 //       pixelData[y*emgTexture.Width+x] = y<=height1 ?Color.Transparent :Color.Red;
                 //   }

                 //   for (int y = emgTexture.Height/2; y < emgTexture.Height; y++)
                 //   {

                 //       pixelData[y * emgTexture.Width + x] = (y-250) <= height2 ? Color.Transparent : Color.Blue;
                 //   }

//                }
  //              emgTexture.SetData<Color>(pixelData);
    //            emgDrawn = true;

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
            //_emgInput.MuscleActivationChanged += _emgInput_MuscleActivationChanged;

            base.OnCreate();
        }

        public override void OnDestroy()
        {
            //_emgInput.MuscleActivationChanged -= _emgInput_MuscleActivationChanged;
            base.OnDestroy();
        }

        private float max0 = 0;
        private float max1 = 0;

        private void _emgInput_MuscleActivationChanged(object sender, MuscleActivationChangedEventArgs e)
        {
            _framesReceived++;
            if (e.EMGSensor != null) {
                max0 = (float)Math.Max(e.EMGSensor[0].AveragedSample[0], max0);
                max1 = (float)Math.Max(e.EMGSensor[1].AveragedSample[0], max1);
                float valJumping = (float)Math.Max(e.EMGSensor[0].AveragedSample[0], 0);
                float valShooting = (float)Math.Max(e.EMGSensor[1].AveragedSample[0], 0);
                emgImage1.Percentage = valJumping / max0;
                emgImage2.Percentage = valShooting / max1;

                //if (e.EMGSensor[0].MuscleActivated) {
                _emgInput.ActivationThreshold[0] = (max0 * jumpingButton.Percentage);
                _emgInput.ActivationThreshold[1] = (max1 * shootingButton.Percentage);

                if (valJumping > _emgInput.ActivationThreshold[0]) {
                    character.Jump();
                    
                }
                //else if (e.EMGSensor[1].MuscleActivated) {
                else if (valShooting > _emgInput.ActivationThreshold[1]) { 
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