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

namespace GhostlyLib.Activities
{
    public class SelectWorldActivity : OpenFeasyo.GameTools.UI.Activity
    {
        public SelectWorldActivity(UIEngine engine) : base(engine) {

            float cell = engine.Screen.ScreenHeight / 9;

            Image backgroundImage = new Image(_engine.Content.LoadTexture("textures/ghostly/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            Label infoLabel = new Label("Select the world", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            infoLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X - (infoLabel.Size.X / 2), 70);


            TextButton world1Button = new TextButton("Earth", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world1Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 1)); };
            world1Button.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 3) - world1Button.Size / 2;

            TextButton world2Button = new TextButton("Water", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world2Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 2)); };
            world2Button.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 4) - world2Button.Size / 2;

            TextButton world3Button = new TextButton("Rock", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world3Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 3)); };
            world3Button.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 5) - world3Button.Size / 2;

            TextButton world4Button = new TextButton("Rock Advanced", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world4Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 4)); };
            world4Button.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 6) - world4Button.Size / 2;

            Components.Add(infoLabel);
            Components.Add(world1Button);
            Components.Add(world2Button);
            Components.Add(world3Button);
            Components.Add(world4Button);
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