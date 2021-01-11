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

namespace LoopLib.Activities
{
    class ConfigureTouchActivity : OpenFeasyo.GameTools.UI.Activity
    {
        private Label _rightTurnLabel;
        private Label _leftTurnLabel;
        public static bool Reversed = false;

        public ConfigureTouchActivity(UIEngine engine) : base(engine)
        {
            Image backgroundImage = new Image(_engine.Content.LoadTexture("Textures/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            Label infoLabel = new Label("Touch the halves of the screen to", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            infoLabel.Position = engine.Screen.ScreenMiddle - infoLabel.Size / 2 - new Vector2(0, engine.Screen.ScreenMiddle.Y / 2);

            _rightTurnLabel = new Label("Right turn", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            _rightTurnLabel.Position = engine.Screen.ScreenMiddle - _rightTurnLabel.Size / 2 + new Vector2(engine.Screen.ScreenMiddle.X/2, 0);

            _leftTurnLabel = new Label("Left turn", engine.Content.LoadFont("Fonts/Ubuntu" + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            _leftTurnLabel.Position = engine.Screen.ScreenMiddle - _leftTurnLabel.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);


            TextButton next = new TextButton("Next", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            next.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                StartActivity(new MainMenuActivity(engine)); 
            };
            next.Position = engine.Screen.ScreenMiddle - next.Size / 2 + new Vector2(0, engine.Screen.ScreenMiddle.Y/2);

            TextButton switchButton = new TextButton(/*"< switch >"*/"switch", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            switchButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                Reversed = !Reversed;
                UpdateLabelsPosition(engine);
            };
            switchButton.Position = engine.Screen.ScreenMiddle - switchButton.Size / 2 + new Vector2(0, 0);

            Components.Add(next);
            Components.Add(switchButton);
            Components.Add(_rightTurnLabel);
            Components.Add(_leftTurnLabel);
            Components.Add(infoLabel);
        }

        private void UpdateLabelsPosition(UIEngine engine) {
            Vector2 quarterDist = new Vector2((engine.Screen.ScreenMiddle.X / 2 ) * (Reversed? -1:1), 0);
            _rightTurnLabel.Position = engine.Screen.ScreenMiddle - _rightTurnLabel.Size / 2 + quarterDist;
            _leftTurnLabel.Position = engine.Screen.ScreenMiddle - _leftTurnLabel.Size / 2 - quarterDist;

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
