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
using OpenFeasyo.GameTools.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LoopLib.Activities
{
    public class MainMenuActivity : OpenFeasyo.GameTools.UI.Activity
    {
        public MainMenuActivity(UIEngine engine) : base(engine) {

            float cell = engine.Screen.ScreenHeight / 8;
            Image backgroundImage = new Image(_engine.Content.LoadTexture("Textures/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            Label infoLabel = new Label("Loop", engine.Content.LoadFont("Fonts/PerfectDark128"), Color.FromNonPremultiplied(11, 206, 196, 256));
            infoLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell*2 ) - infoLabel.Size/2;

            
            TextButton startGameButton = new TextButton("Start Game", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            //TextButton startGameButton = new TextButton("\uf04b", engine.Content.LoadFont("Fonts/Awesome48"), engine.Device);
            startGameButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new GamePlayActivity(engine,1,
                "<?xml version=\"1.0\" encoding=\"utf - 8\"?><Configuration>" +
                            "<devices><device name=\"TrignoEmg\">" +
                                "<analyzers><analyzer file=\"C3DSerializer.dll\" /></analyzers>" +
                            "</device></devices>" +
                            "<bindings>" +
                                "<binding point=\"Left\" zeroAngle=\"0\" sensitivity=\"1\" device=\"TrignoEmg\"><emgSensor device=\"TrignoEmg\" channel=\"0\"></emgSensor></binding>" +
                                "<binding point=\"Right\" zeroAngle=\"0\" sensitivity=\"1\" device=\"TrignoEmg\"><emgSensor device=\"TrignoEmg\" channel=\"1\"></emgSensor></binding>" +
                            "</bindings></Configuration>"
                )); };
            startGameButton.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 4) - startGameButton.Size/2;

            TextButton calibrateButton = new TextButton("Calibrate", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            calibrateButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new StartCalibrationActivity(engine, null)); };
            calibrateButton.Size = startGameButton.Size;
            calibrateButton.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 5) - calibrateButton.Size / 2;

            TextButton aboutButton = new TextButton("Credits", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            //TextButton aboutButton = new TextButton("\uf05a", engine.Content.LoadFont("Fonts/Awesome48"), engine.Device);
            aboutButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new AboutActivity(engine)); };
            aboutButton.Size = startGameButton.Size;
            aboutButton.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 5) - aboutButton.Size / 2;

            TextButton exitButton = new TextButton("Exit", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            //TextButton exitButton = new TextButton("\uf52b", engine.Content.LoadFont("Fonts/Awesome48"), engine.Device);
            exitButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { engine.StartActivity(null); };
            exitButton.Size = startGameButton.Size;
            exitButton.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 6) - exitButton.Size / 2;

            Image vubetrologoImage = new Image(_engine.Content.LoadTexture("textures/vubetrologo"));
            vubetrologoImage.Size = new Vector2((vubetrologoImage.Size.X / vubetrologoImage.Size.Y) * cell, cell);
            vubetrologoImage.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 7) - new Vector2(vubetrologoImage.Size.X / 2, vubetrologoImage.Size.Y / 2);


            Components.Add(vubetrologoImage);
            Components.Add(aboutButton);
            Components.Add(startGameButton);
            Components.Add(infoLabel);
            Components.Add(exitButton);
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _engine.StartActivity(new InputSelectionActivity(_engine));
            }
        }
    }
}