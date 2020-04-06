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
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.Platform.Controls;
using Microsoft.Xna.Framework.Input;

namespace GhostlyLib.Activities
{
    public class GamePlayActivity : OpenFeasyo.GameTools.UI.Activity
    {
        private ComponentCollection _gameOverPanel;
        private ComponentCollection _levelDonePanel;
        private ComponentCollection _pausePanel;
        private ComponentCollection _gameplayPanel;

        private Label _scoreLabel;

        private string _configuration;

        private GameScreen _screen;


        public GamePlayActivity(UIEngine engine, int level, string intputConfig) : base(engine) {

            _screen = new GameScreen(level, engine.MusicPlayer, engine.Screen);
            _screen.GameStarted += _screen_GameStarted;
            _screen.GameFinished += _screen_GameFinished;

            _configuration = intputConfig;

            #region Game Over Panel
            _gameOverPanel = new ComponentCollection();
            _gameOverPanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            

            TextButton backButton = new TextButton("Back to menu", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            backButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                StartActivity(new MainMenuActivity(engine));
                _engine.MusicPlayer.Play("menu");
            };
            backButton.Position = engine.Screen.ScreenMiddle - backButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

            TextButton playAgainButton = new TextButton("Play again", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            playAgainButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                Components.Remove(_gameOverPanel);
                _screen.LoadLevel();
            };
            playAgainButton.Position = engine.Screen.ScreenMiddle - playAgainButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

            Label gameOverLabel = new Label("GAME OVER", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            gameOverLabel.Position = engine.Screen.ScreenMiddle - gameOverLabel.Size / 2 - new Vector2(0, engine.Screen.ScreenMiddle.Y * 2 / 3 ); ;

            _gameOverPanel.Components.Add(gameOverLabel);
            _gameOverPanel.Components.Add(playAgainButton);
            _gameOverPanel.Components.Add(backButton);
            #endregion Game Over Panel

            #region Level Done Panel
            _levelDonePanel = new ComponentCollection();
            _levelDonePanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);

            TextButton nextButton = new TextButton("Next Level", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            nextButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                Components.Remove(_levelDonePanel);
                _screen.LoadNextLevel();
            };
            nextButton.Position = engine.Screen.ScreenMiddle - nextButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

            backButton = new TextButton("Back to menu", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            backButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                StartActivity(new MainMenuActivity(engine));
                _engine.MusicPlayer.Play("menu");
            };
            backButton.Position = engine.Screen.ScreenMiddle - backButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

            Label levelDoneLabel = new Label("LEVEL COMPLETED", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            levelDoneLabel.Position = engine.Screen.ScreenMiddle - levelDoneLabel.Size / 2 - new Vector2(0,engine.Screen.ScreenMiddle.Y * 2 / 3);

            _scoreLabel = new Label("Score: 00", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            _scoreLabel.Position = engine.Screen.ScreenMiddle - _scoreLabel.Size / 2 + new Vector2(0, -engine.Screen.ScreenMiddle.Y / 3);


            _levelDonePanel.Components.Add(nextButton);
            _levelDonePanel.Components.Add(backButton);
            _levelDonePanel.Components.Add(levelDoneLabel);
            _levelDonePanel.Components.Add(_scoreLabel);
            #endregion Level Done Panel

            #region Pause Panel
            _pausePanel = new ComponentCollection();
            _pausePanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);

            Texture2D texture = new Texture2D(engine.Device, 1, 1);
            texture.SetData(new Color[] { Color.FromNonPremultiplied(0, 0, 0, 170) });
            Image background = new Image(texture);
            background.Size = _pausePanel.Size;
            
            TextButton exitButton = new TextButton("Exit", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            exitButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                _screen.Exit();
                StartActivity(new MainMenuActivity(engine));
                _engine.MusicPlayer.Play("menu");
            };
            exitButton.Position = engine.Screen.ScreenMiddle - exitButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

            TextButton continueButton = new TextButton("Continue", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            continueButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                Components.Remove(_pausePanel);
                Components.Add(_gameplayPanel);
                _screen.ResumeGame();
            };
            continueButton.Position = engine.Screen.ScreenMiddle - continueButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

            _pausePanel.Components.Add(background);
            _pausePanel.Components.Add(exitButton);
            _pausePanel.Components.Add(continueButton);
            
            #endregion Pause Panel

            #region Gameplay Panel
            _gameplayPanel = new ComponentCollection();
            _gameplayPanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            
            TextButton pauseButton = new TextButton("\uf04c", engine.Content.LoadFont("Fonts/Awesome" + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            pauseButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                Components.Remove(_gameplayPanel);
                Components.Add(_pausePanel);
                _screen.PauseGame();
            };
            pauseButton.TextMargin = 20;
            pauseButton.Position = new Vector2(engine.Screen.ScreenWidth - (pauseButton.Size.X), 0);

            _gameplayPanel.Components.Add(pauseButton);

            #endregion Gameplay Panel




        }

        public override void OnCursorDown(Vector2 pos)
        {
            base.OnCursorDown(pos);
            if (GhostlyActionHandlers.CurrentLevel != null)
            {
                bool right = pos.X > _engine.Device.Viewport.Width / 2;
                right = (ConfigureTouchActivity.Reversed ? !right : right);
                if (right)
                {
                    GhostlyActionHandlers.CurrentLevel.ProcessPrimaryAction(true);
                }
                else
                {
                    GhostlyActionHandlers.CurrentLevel.ProcessSecondaryAction(true);
                }
            }
        }

        //public override bool OnCursorClick(Vector2 pos)
        //{
        //    if (base.OnCursorClick(pos))
        //    {
        //        return true;
        //    }
        //    else {
        //        if (GhostlyActionHandlers.CurrentLevel != null) { 
        //            if (pos.X > _engine.Device.Viewport.Width / 2)
        //            {
        //                GhostlyActionHandlers.CurrentLevel.ProcessPrimaryAction(true);
        //            } else {
        //                GhostlyActionHandlers.CurrentLevel.ProcessSecondaryAction(true);
        //            }
        //        }
        //        return false;
        //    }

        //}

        private void _screen_GameFinished(object sender, GameFinishedEventArgs e)
        {
            GhostlyGame.Instance.OnGameFinished(e.Score, e.Reason);

            if (e.Reason == GameFinishedEventArgs.EndReason.GameFailed)
            {
                Components.Remove(_gameplayPanel);
                Components.Add(_gameOverPanel);
            }
            else if (e.Reason == GameFinishedEventArgs.EndReason.GoalAccomplished) {
                Components.Remove(_gameplayPanel);
                Components.Add(_levelDonePanel);
                _scoreLabel.Text = "Score: " + e.Score;
            }

        }

        private void _screen_GameStarted(object sender, GameStartedEventArgs e)
        {
            Components.Add(_gameplayPanel);
            _engine.MusicPlayer.Play("game");
            GhostlyGame.Instance.Configuration = _configuration;
            GhostlyGame.Instance.OnGameStarted(e);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            _screen.Initialize();
            _screen.LoadContent(_engine.Content);
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _screen.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                if (Components.Contains(_gameplayPanel))
                {
                    Components.Remove(_gameplayPanel);
                    Components.Add(_pausePanel);
                    _screen.PauseGame();
                }
                else if (Components.Contains(_pausePanel))
                {
                    Components.Remove(_pausePanel);
                    Components.Add(_gameplayPanel);
                    _screen.ResumeGame();
                }
                else {
                    StartActivity(new MainMenuActivity(_engine));
                    _engine.MusicPlayer.Play("menu");
                }

            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            
            _screen.Draw(spritebatch, gameTime);
            base.Draw(gameTime, spritebatch);
        }

        public override void OnDestroy()
        {
            _screen.UnloadContent();
            base.OnDestroy();
        }
    }
}