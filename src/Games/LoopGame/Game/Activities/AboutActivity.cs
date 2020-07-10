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
    public class AboutActivity : OpenFeasyo.GameTools.UI.Activity
    {
        public AboutActivity(UIEngine engine) : base(engine)
        {
            int headerSize = new int[] { 12, 24, 36, 48, 64 }[Math.Max(engine.Screen.FontSize - 1, 0)];
            int fontSize = new int[] { 12, 24, 36, 48, 64 }[Math.Max( engine.Screen.FontSize - 2,0)];

            float cell = engine.Screen.ScreenHeight / 20;
            Image backgroundImage = new Image(_engine.Content.LoadTexture("Textures/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            Label delucaLabel = new Label("Application development was supported by the De Luca Foundation and Delsys, Inc.", engine.Content.LoadFont("Fonts/Ubuntu" + fontSize), LoopGame.MENU_FONT_COLOR);
            delucaLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 3) - delucaLabel.Size / 2;

            Label musicHeaderLabel = new Label("Music in this game", engine.Content.LoadFont("Fonts/Ubuntu" + headerSize), LoopGame.MENU_FONT_COLOR);
            musicHeaderLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 5) - musicHeaderLabel.Size / 2;

            Label music1Label = new Label("Music from https://filmmusic.io", engine.Content.LoadFont("Fonts/Ubuntu" + fontSize), LoopGame.MENU_FONT_COLOR);
            music1Label.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 6) - music1Label.Size / 2;

            Label music2Label = new Label("\"Wholesome\" and \"Arcadia\" by Kevin MacLeod(https://incompetech.com)", engine.Content.LoadFont("Fonts/Ubuntu" + fontSize), LoopGame.MENU_FONT_COLOR);
            music2Label.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 7) - music2Label.Size / 2;

            Label music3Label = new Label("License: CC BY (http://creativecommons.org/licenses/by/4.0/)", engine.Content.LoadFont("Fonts/Ubuntu" + fontSize), LoopGame.MENU_FONT_COLOR);
            music3Label.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 8) - music3Label.Size / 2;

            Label authorsHeaderLabel = new Label("Authors", engine.Content.LoadFont("Fonts/Ubuntu" + headerSize), LoopGame.MENU_FONT_COLOR);
            authorsHeaderLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 10) - authorsHeaderLabel.Size / 2 ;

            Label authors1Label = new Label("This game was developed at Vrije Universiteit Brussel (ETRO dept.) by", engine.Content.LoadFont("Fonts/Ubuntu" + fontSize), LoopGame.MENU_FONT_COLOR);
            authors1Label.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 11) - authors1Label.Size / 2;

            Label authors2Label = new Label("Katarina Kostkova and Lubos Omelina in the team of prof. Bart Jansen", engine.Content.LoadFont("Fonts/Ubuntu" + fontSize), LoopGame.MENU_FONT_COLOR);
            authors2Label.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 12) - authors2Label.Size / 2;

            Label authors3Label = new Label("Visit http://www.etrovub.be for more info.", engine.Content.LoadFont("Fonts/Ubuntu" + fontSize), Color.Black);
            authors3Label.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 13) - authors3Label.Size / 2 ;


            TextButton backButton = new TextButton("Back", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            backButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { engine.StartActivity(new MainMenuActivity(engine)); };
            backButton.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 15) - backButton.Size / 2;

            Image vubetrologoImage = new Image(_engine.Content.LoadTexture("Textures/vubetrologo"));
            vubetrologoImage.Size = new Vector2((vubetrologoImage.Size.X / vubetrologoImage.Size.Y) * cell * 2, cell * 2);
            vubetrologoImage.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 18) - new Vector2(vubetrologoImage.Size.X / 2, vubetrologoImage.Size.Y / 2);

            

            
            Components.Add(delucaLabel);

            Components.Add(musicHeaderLabel);
            Components.Add(music1Label);
            Components.Add(music2Label);
            Components.Add(music3Label);

            Components.Add(authorsHeaderLabel);
            Components.Add(authors1Label);
            Components.Add(authors2Label);
            Components.Add(authors3Label);

            Components.Add(vubetrologoImage);
            Components.Add(backButton);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _engine.StartActivity(new MainMenuActivity(_engine));
            }
        }
    }
}
