/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2025 - Katarina Kostkova
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using GhostlyGame;
using GhostlyGame.Models;

namespace GhostlyLib.Activities
{
    public class LimitedSelectLevelActivity : OpenFeasyo.GameTools.UI.Activity
    {
        public LimitedSelectLevelActivity(UIEngine engine) : base(engine)
        {
            float cell = engine.Screen.ScreenHeight / 10;

            Image backgroundImage = new Image(_engine.Content.LoadTexture("textures/ghostly/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            Label infoLabel = new Label(LocalizationResourceManager.Instance["StartTheLevel"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            infoLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X - (infoLabel.Size.X / 2), 70);
            Components.Add(infoLabel);

            if (GameSessionInfo.Instance.LevelsCompleted < GameSessionInfo.Instance.RequiredLevels)
            {
                float verticalSpacing = engine.Screen.ScreenHeight * 0.01f;
                float horizontalSpacing = engine.Screen.ScreenWidth * 0.01f;
                float tileWidth = (engine.Screen.ScreenWidth * 0.9f) / 6 - horizontalSpacing;
                float tileHeight = (engine.Screen.ScreenHeight * 0.75f) / 5 - verticalSpacing;

                Vector2 offset = new Vector2(engine.Screen.ScreenWidth * 0.06f, engine.Screen.ScreenHeight * 0.20f);

                LevelSelectionButton startLevelButton = new LevelSelectionButton(LocalizationResourceManager.Instance["StartToPlay"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
                startLevelButton.Level = (int) GameSessionInfo.Instance.LevelToPlay();
                startLevelButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
                {
                    StartActivity(new GamePlayActivity(engine, ((LevelSelectionButton)sender).Level,
                        "<?xml version=\"1.0\" encoding=\"utf - 8\"?><Configuration>" +
                        "<devices><device name=\"Trigno Avanti\">" +
                        "<analyzers><analyzer file=\"C3DSerializer.dll\" /></analyzers>" +
                        "</device></devices>" +
                        "<bindings>" +
                        "<binding point=\"Jump/Swim\" zeroAngle=\"0\" sensitivity=\"1\" device=\"Trigno Avanti\"><emgSensor device=\"Trigno Avanti\" channel=\"0\"></emgSensor></binding>" +
                        "<binding point=\"Shoot\" zeroAngle=\"0\" sensitivity=\"1\" device=\"Trigno Avanti\"><emgSensor device=\"Trigno Avanti\" channel=\"1\"></emgSensor></binding>" +
                        "</bindings></Configuration>"
                        ));
                };
                startLevelButton.Position = (offset + new Vector2(engine.Screen.ScreenMiddle.X - startLevelButton.Size.X / 2, engine.Screen.ScreenMiddle.Y - startLevelButton.Size.Y / 2));
                startLevelButton.Size = new Vector2(tileWidth, tileHeight);
                Components.Add(startLevelButton);
            }
            else
            {
                Label sessionCompleted = new Label(LocalizationResourceManager.Instance["SessionCompleted"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
                infoLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X - (sessionCompleted.Size.X / 2), cell * 4);
                Components.Add(infoLabel);

                TextButton allWorldsButton = new TextButton(LocalizationResourceManager.Instance["AllWorlds"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
                allWorldsButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new SelectWorldActivity(engine)); };
                allWorldsButton.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 5) - allWorldsButton.Size / 2;
                Components.Add(allWorldsButton);
            }
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