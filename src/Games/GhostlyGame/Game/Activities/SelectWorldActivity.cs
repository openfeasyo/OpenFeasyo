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

namespace GhostlyLib.Activities
{
    public class SelectWorldActivity : OpenFeasyo.GameTools.UI.Activity
    {
        public SelectWorldActivity(UIEngine engine) : base(engine)
        {

            float cell = engine.Screen.ScreenHeight / 9;

            Image backgroundImage = new Image(_engine.Content.LoadTexture("textures/ghostly/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            float leftColumn = engine.Screen.ScreenWidth / 4;
            float rightColumn = engine.Screen.ScreenWidth / 4 + engine.Screen.ScreenWidth / 2;

            Label infoLabel = new Label(LocalizationResourceManager.Instance["SelectTheWorld"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            infoLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X - (infoLabel.Size.X / 2), 70);

            TextButton world1Button = new TextButton(LocalizationResourceManager.Instance["Earth"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world1Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 1, 30)); };
            world1Button.Position = new Vector2(leftColumn, cell * 3) - world1Button.Size / 2;

            TextButton world2Button = new TextButton(LocalizationResourceManager.Instance["Water"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world2Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 31, 60)); };
            world2Button.Position = new Vector2(leftColumn, cell * 4) - world2Button.Size / 2;

            TextButton world3Button = new TextButton(LocalizationResourceManager.Instance["Rock"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world3Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 61, 90)); };
            world3Button.Position = new Vector2(leftColumn, cell * 5) - world3Button.Size / 2;

            TextButton world4Button = new TextButton(LocalizationResourceManager.Instance["Ice"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world4Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 91, 120)); };
            world4Button.Position = new Vector2(leftColumn, cell * 6) - world4Button.Size / 2;

            /*TextButton world5Button = new TextButton("Space", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world5Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 6)); };
            world5Button.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 7) - world5Button.Size / 2;*/

            TextButton world6Button = new TextButton(LocalizationResourceManager.Instance["Land"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world6Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 121, 140)); };
            world6Button.Position = new Vector2(rightColumn, cell * 3) - world6Button.Size / 2;

            TextButton world7Button = new TextButton(LocalizationResourceManager.Instance["Space"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world7Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 141, 160)); };
            world7Button.Position = new Vector2(rightColumn, cell * 4) - world7Button.Size / 2;

            TextButton world8Button = new TextButton(LocalizationResourceManager.Instance["Maze"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world8Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 161, 190)); };
            world8Button.Position = new Vector2(rightColumn, cell * 5) - world8Button.Size / 2;

            TextButton world9Button = new TextButton(LocalizationResourceManager.Instance["SimpleSpace"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            world9Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectLevelActivity(engine, 191, 200)); };
            world9Button.Position = new Vector2(rightColumn, cell * 6) - world9Button.Size / 2;

            Components.Add(infoLabel);
            Components.Add(world1Button);
            Components.Add(world2Button);
            Components.Add(world3Button);
            Components.Add(world4Button);
            /*Components.Add(world5Button);*/
            Components.Add(world6Button);
            Components.Add(world7Button);
            Components.Add(world8Button);
            Components.Add(world9Button);
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