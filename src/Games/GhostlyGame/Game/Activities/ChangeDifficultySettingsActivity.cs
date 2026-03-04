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
    public class ChangeDifficultySettingsActivity : OpenFeasyo.GameTools.UI.Activity
    {
        public ChangeDifficultySettingsActivity(UIEngine engine) : base(engine)
        {
            float cell = engine.Screen.ScreenHeight / 10;

            Image backgroundImage = new Image(_engine.Content.LoadTexture("textures/ghostly/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            Label infoLabel = new Label(LocalizationResourceManager.Instance["CurrentDifficulty"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            infoLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X - (infoLabel.Size.X / 2), 70);
            Components.Add(infoLabel);

            Label diffLabel = new Label(GameSessionInfo.Instance.SelectedPatient.DifficultyLevel.ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            diffLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X - (diffLabel.Size.X / 2), 150);
            Components.Add(diffLabel);

            float verticalSpacing = engine.Screen.ScreenHeight * 0.01f;
            float horizontalSpacing = engine.Screen.ScreenWidth * 0.01f;
            float tileWidth = (engine.Screen.ScreenWidth * 0.9f) / 10 - horizontalSpacing;
            float tileHeight = (engine.Screen.ScreenHeight * 0.75f) / 7 - verticalSpacing;
            Vector2 offset = new Vector2(engine.Screen.ScreenWidth * 0.07f, engine.Screen.ScreenHeight * 0.3f);

            int diffLevel = 1;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    LevelSelectionButton diffLevelButton = new LevelSelectionButton(diffLevel.ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
                    diffLevelButton.Level = diffLevel;
                    diffLevelButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
                    {
                        GameSessionInfo.Instance.SelectedPatient.DifficultyLevel = diffLevelButton.Level;
                        StartActivity(new StartGameActivity(engine));
                    };
                    diffLevelButton.Position = (offset + new Vector2(j * (tileWidth + horizontalSpacing), i * (tileHeight + verticalSpacing)));
                    diffLevelButton.Size = new Vector2(tileWidth, tileHeight);
                    diffLevel++;
                    Components.Add(diffLevelButton);
                }
            }
                                                     
            TextButton cancelButton = new TextButton(LocalizationResourceManager.Instance["Cancel"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            cancelButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new StartGameActivity(engine)); };
            cancelButton.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 9) - cancelButton.Size / 2;
            Components.Add(cancelButton);
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