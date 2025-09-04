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
        private ComponentCollection _selfAssesmentPanel;
        private ComponentCollection _bfrVopPanel;

        private Label _scoreLabel;

        private string _configuration;

        private GameScreen _screen;


        public GamePlayActivity(UIEngine engine, int level, string intputConfig) : base(engine)
        {
            if (level >= 161 && level <= 180)
            {
                _screen = new GameScreen3D(level, engine.MusicPlayer, engine.Screen, engine.Device);
            }
            else
            {
                _screen = new GameScreen2D(level, engine.MusicPlayer, engine.Screen);
            }

            _screen.GameStarted += _screen_GameStarted;
            _screen.GameFinished += _screen_GameFinished;
            //_scene.SelfAssesmentFinished += _screen_SelfAssesmentFinished;

            _configuration = intputConfig;

            float cell = engine.Screen.ScreenHeight / 16;

            #region Game Over Panel
            _gameOverPanel = new ComponentCollection();
            _gameOverPanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);


            TextButton backButton = new TextButton("Back to menu", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            backButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
            {
                StartActivity(new MainMenuActivity(engine));
                _engine.MusicPlayer.Play("menu");
            };
            backButton.Position = engine.Screen.ScreenMiddle - backButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

            TextButton playAgainButton = new TextButton("Play again", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            playAgainButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
            {
                Components.Remove(_gameOverPanel);
                _screen.Initialize();   //to initalize background
                _screen.LoadLevel();
            };
            playAgainButton.Position = engine.Screen.ScreenMiddle - playAgainButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

            Label gameOverLabel = new Label("GAME OVER", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            gameOverLabel.Position = engine.Screen.ScreenMiddle - gameOverLabel.Size / 2 - new Vector2(0, engine.Screen.ScreenMiddle.Y * 2 / 3); ;

            _gameOverPanel.Components.Add(gameOverLabel);
            _gameOverPanel.Components.Add(playAgainButton);
            _gameOverPanel.Components.Add(backButton);
            #endregion Game Over Panel

            #region Level Done Panel
            _levelDonePanel = new ComponentCollection();
            _levelDonePanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);

            TextButton nextButton = new TextButton("Next Level", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            nextButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
            {
                Components.Remove(_levelDonePanel);
                _screen.LoadNextLevel();
            };
            nextButton.Position = engine.Screen.ScreenMiddle - nextButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

            backButton = new TextButton("Back to menu", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            backButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
            {
                StartActivity(new MainMenuActivity(engine));
                _engine.MusicPlayer.Play("menu");
            };
            backButton.Position = engine.Screen.ScreenMiddle - backButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

            Label levelDoneLabel = new Label("LEVEL COMPLETED", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            levelDoneLabel.Position = engine.Screen.ScreenMiddle - levelDoneLabel.Size / 2 - new Vector2(0, engine.Screen.ScreenMiddle.Y * 2 / 3);

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
            exitButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
            {
                _screen.Exit();
                StartActivity(new MainMenuActivity(engine));
                _engine.MusicPlayer.Play("menu");
            };
            exitButton.Position = engine.Screen.ScreenMiddle - exitButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

            TextButton continueButton = new TextButton("Continue", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            continueButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
            {
                Components.Remove(_pausePanel);
                Components.Add(_gameplayPanel);
                _screen.ResumeGame();
            };
            continueButton.Position = engine.Screen.ScreenMiddle - continueButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

            _pausePanel.Components.Add(background);
            _pausePanel.Components.Add(exitButton);
            _pausePanel.Components.Add(continueButton);

            #endregion Pause Panel

            #region Self-assesment Rated Perceived Exertion Scale
            _selfAssesmentPanel = new ComponentCollection();
            _selfAssesmentPanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);

            int assesmentValue = 0;

            float verticalSpacing = engine.Screen.ScreenHeight * 0.02f;
            float horizontalSpacing = engine.Screen.ScreenWidth * 0.02f;
            float tileWidth = (engine.Screen.ScreenWidth * 0.9f) / 6 - horizontalSpacing;
            float tileHeight = (engine.Screen.ScreenHeight * 0.75f) / 5 - verticalSpacing;
            Vector2 offset = new Vector2(engine.Screen.ScreenWidth * 0.08f, engine.Screen.ScreenHeight * 0.22f);

            for (int y = 0; y < 2; y++)
                for (int x = 0; x < 5; x++)
                {
                    TextButton assesmentButton = new TextButton(assesmentValue.ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
                    assesmentButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
                    {
                        //TODO save self assesment value
                        //selfAssesment = Int16.Parse(assesmentButton.Text);
                        Components.Remove(_selfAssesmentPanel);
                        Components.Add(_bfrVopPanel);
                    };
                    assesmentButton.Position = (offset + new Vector2(x * (tileWidth + horizontalSpacing), y * (tileHeight + verticalSpacing)));
                    assesmentButton.Size = new Vector2(tileWidth, tileHeight);
                    assesmentValue++;
                    _selfAssesmentPanel.Components.Add(assesmentButton);
                }

            Label selfAssesmentLabel = new Label("RATED PERCEIVED EXERTION", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            selfAssesmentLabel.Position = engine.Screen.ScreenMiddle - selfAssesmentLabel.Size / 2 - new Vector2(0, engine.Screen.ScreenMiddle.Y * 2 / 3);

            _selfAssesmentPanel.Components.Add(selfAssesmentLabel);
            #endregion Self-assesment Rated Perceived Exertion Scale

            #region BFR AOP
            _bfrVopPanel = new ComponentCollection();
            _bfrVopPanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);

            EmgImage emgImage1 = new EmgImage(engine.Device, 0.2f);
            emgImage1.Size = new Vector2((int)(engine.Screen.ScreenWidth * 0.1), (int)(engine.Screen.ScreenHeight * 0.6));
            emgImage1.Position = new Vector2((int)(engine.Screen.ScreenWidth * 0.3) - (emgImage1.Size.X / 2), cell * 2);
            _bfrVopPanel.Components.Add(emgImage1);

            EmgImage emgImage2 = new EmgImage(engine.Device, 0.2f);
            emgImage2.Size = new Vector2((int)(engine.Screen.ScreenWidth * 0.1), (int)(engine.Screen.ScreenHeight * 0.6));
            emgImage2.Position = new Vector2((int)(engine.Screen.ScreenWidth * 0.7) - (emgImage2.Size.X / 2), cell * 2);
            _bfrVopPanel.Components.Add(emgImage2);

            Label percentageBFRLeftLabel = new Label("50% ", engine.Content.LoadFont("Fonts/Ubuntu" + GhostlyGame.MENU_BUTTON_FONT_SIZE), Color.White);
            percentageBFRLeftLabel.Position = emgImage1.Position + emgImage1.Size - percentageBFRLeftLabel.Size;
            _bfrVopPanel.Components.Add(percentageBFRLeftLabel);

            Label percentageBfrRightLabel = new Label("50% ", engine.Content.LoadFont("Fonts/Ubuntu" + GhostlyGame.MENU_BUTTON_FONT_SIZE), Color.White);
            percentageBfrRightLabel.Position = emgImage2.Position + emgImage2.Size - percentageBfrRightLabel.Size;
            _bfrVopPanel.Components.Add(percentageBfrRightLabel);

            DraggableButton bfrLeftVOPButton = new DraggableButton("Left VOP", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device, 0.5f);
            bfrLeftVOPButton.MinY = emgImage1.Position.Y;
            bfrLeftVOPButton.MaxY = emgImage1.Position.Y + emgImage1.Size.Y;
            bfrLeftVOPButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { };
            bfrLeftVOPButton.Position = new Vector2((int)(engine.Screen.ScreenWidth * 0.3), cell * 2 + emgImage1.Size.Y / 2) - bfrLeftVOPButton.Size / 2;
            bfrLeftVOPButton.PercentageChanged += (float value) => { percentageBFRLeftLabel.Text = (value * 100).ToString("00.") + "%"; };
            _bfrVopPanel.Components.Add(bfrLeftVOPButton);

            DraggableButton bfrRightVOPButton = new DraggableButton("Right VOP", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device, 0.5f);
            bfrRightVOPButton.MinY = emgImage2.Position.Y;
            bfrRightVOPButton.MaxY = emgImage2.Position.Y + emgImage2.Size.Y;
            bfrRightVOPButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { };
            bfrRightVOPButton.Position = new Vector2((int)(engine.Screen.ScreenWidth * 0.7), cell * 2 + emgImage2.Size.Y / 2) - bfrRightVOPButton.Size / 2;
            bfrRightVOPButton.PercentageChanged += (float value) => { percentageBfrRightLabel.Text = (value * 100).ToString("00.") + "%"; };
            _bfrVopPanel.Components.Add(bfrRightVOPButton);

            TextButton okButton = new TextButton("OK", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            okButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
            {
                //TODO save values from sliders
                //BFR_L = bfrLeftVOPButton.Percentage;
                //BFR_R = bfrRightVOPButton.Percentage;
                Components.Remove(_bfrVopPanel);
                Components.Add(_levelDonePanel);
            };
            okButton.Position = new Vector2((int)(engine.Screen.ScreenWidth * 0.5) - (okButton.Size.X / 2), (int)(engine.Screen.ScreenHeight * 0.8));
            _bfrVopPanel.Components.Add(okButton);

            Label bfrVopValuesLabel = new Label("Are BFR VOP Values correct?", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            bfrVopValuesLabel.Position = new Vector2((int)((engine.Screen.ScreenWidth - bfrVopValuesLabel.Size.X) / 2), (int)(engine.Screen.ScreenHeight * 0.1));

            _bfrVopPanel.Components.Add(bfrVopValuesLabel);
            #endregion

            #region Gameplay Panel
            _gameplayPanel = new ComponentCollection();
            _gameplayPanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);

            TextButton pauseButton = new TextButton("\uf04c", engine.Content.LoadFont("Fonts/Awesome" + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            pauseButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
            {
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
            GhostlyGame.Instance.OnGameFinished(e.Score, e.Level, e.Reason);

            if (e.Reason == GameFinishedEventArgs.EndReason.GameFailed)
            {
                Components.Remove(_gameplayPanel);
                Components.Add(_gameOverPanel);
            }
            else if (e.Reason == GameFinishedEventArgs.EndReason.GoalAccomplished)
            {
                Components.Remove(_gameplayPanel);
                //Components.Add(_levelDonePanel);
                Components.Add(_selfAssesmentPanel);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
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
                else
                {
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