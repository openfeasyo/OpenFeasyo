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
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Configuration;
using Microsoft.Xna.Framework.Input;
using OpenFeasyo.GameTools.Core;
using System.Collections.Generic;

namespace LoopLib.Activities
{
    public class GamePlayActivity : OpenFeasyo.GameTools.UI.Activity, SceneInterface
    {
        private ComponentCollection _gameOverPanel;
        private ComponentCollection _levelDonePanel;
        private ComponentCollection _pausePanel;
        private ComponentCollection _gameplayPanel;

        private Label _scoreLabel;

        private Engine _gameEngine;

        private string _configuration;

        

        public GamePlayActivity(UIEngine engine, int level, string intputConfig) : base(engine) {

            _gameEngine = new Engine(this, engine.Content, LoopGame.Instance, engine.Screen);
           
            //_gameEngine.MaxScore = MaxScore;
            _gameEngine.GameStarted += _gameEngine_GameStarted;
            //() => { OnGameStarted(); };
            _gameEngine.GameFinished += _gameEngine_GameFinished;
            //(score, reason) => { OnGameFinished(score,reason); };
            InitializeSceneInterface();
            _gameEngine.LoadContent();

            #region Game Over Panel
            _gameOverPanel = new ComponentCollection();
            _gameOverPanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);


            TextButton backButton = new TextButton("Back to menu", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            backButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                StartActivity(new MainMenuActivity(engine));
                _engine.MusicPlayer.Play("menu");
            };
            backButton.Position = engine.Screen.ScreenMiddle - backButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

            TextButton playAgainButton = new TextButton("Play again", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            playAgainButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                Components.Remove(_gameOverPanel);
                _gameEngine.StartGame();
            };
            playAgainButton.Position = engine.Screen.ScreenMiddle - playAgainButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

            Label gameOverLabel = new Label("GAME OVER", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            gameOverLabel.Position = engine.Screen.ScreenMiddle - gameOverLabel.Size / 2 - new Vector2(0, engine.Screen.ScreenMiddle.Y * 2 / 3); ;

            _gameOverPanel.Components.Add(gameOverLabel);
            _gameOverPanel.Components.Add(playAgainButton);
            _gameOverPanel.Components.Add(backButton);
            #endregion Game Over Panel

            #region Level Done Panel
            _levelDonePanel = new ComponentCollection();
            _levelDonePanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);

            TextButton nextButton = new TextButton("Next Level", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            nextButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                Components.Remove(_levelDonePanel);
                _gameEngine.StartGame(); 
            };
            nextButton.Position = engine.Screen.ScreenMiddle - nextButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

            backButton = new TextButton("Back to menu", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            backButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                StartActivity(new MainMenuActivity(engine));
                _engine.MusicPlayer.Play("menu");
            };
            backButton.Position = engine.Screen.ScreenMiddle - backButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

            Label levelDoneLabel = new Label("LEVEL COMPLETED", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            levelDoneLabel.Position = engine.Screen.ScreenMiddle - levelDoneLabel.Size / 2 - new Vector2(0, engine.Screen.ScreenMiddle.Y * 2 / 3);

            _scoreLabel = new Label("Score: 00", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), LoopGame.MENU_FONT_COLOR);
            _scoreLabel.Position = engine.Screen.ScreenMiddle - _scoreLabel.Size / 2 + new Vector2(0, -engine.Screen.ScreenMiddle.Y / 2 + 100);


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

            TextButton exitButton = new TextButton("Exit", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            exitButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                //_screen.Exit();
                StartActivity(new MainMenuActivity(engine));
                _engine.MusicPlayer.Play("menu");
            };
            exitButton.Position = engine.Screen.ScreenMiddle - exitButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

            TextButton continueButton = new TextButton("Continue", engine.Content.LoadFont(LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            continueButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                Components.Remove(_pausePanel);
                Components.Add(_gameplayPanel);
                //_screen.ResumeGame();
            };
            continueButton.Position = engine.Screen.ScreenMiddle - continueButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

            _pausePanel.Components.Add(background);
            _pausePanel.Components.Add(exitButton);
            _pausePanel.Components.Add(continueButton);

            #endregion Pause Panel

            #region Gameplay Panel
            _gameplayPanel = new ComponentCollection();
            _gameplayPanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);

            TextButton pauseButton = new TextButton("\uf04c", engine.Content.LoadFont("Fonts/Awesome" + LoopGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            pauseButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                Components.Remove(_gameplayPanel);
                Components.Add(_pausePanel);
                //_screen.PauseGame();
            };
            pauseButton.TextMargin = 20;
            pauseButton.Position = new Vector2(engine.Screen.ScreenWidth - (pauseButton.Size.X), 0);

            _gameplayPanel.Components.Add(pauseButton);

            #endregion Gameplay Panel
            _gameEngine.StartGame();




        }

        private void _gameEngine_GameFinished(int score, GameFinishedEventArgs.EndReason reason)
        {
            LoopGame.Instance.OnGameFinished(score, reason);

            if (reason == GameFinishedEventArgs.EndReason.GameFailed)
            {
                Components.Remove(_gameplayPanel);
                Components.Add(_gameOverPanel);
            }
            else if (reason == GameFinishedEventArgs.EndReason.GoalAccomplished)
            {
                Components.Remove(_gameplayPanel);
                Components.Add(_levelDonePanel);
                _scoreLabel.Text = "Score: " + score;
            }
        }

        private void _gameEngine_GameStarted()
        {
            Components.Add(_gameplayPanel);
            _engine.MusicPlayer.Play("game");
            LoopGame.Instance.Configuration = _configuration;
            LoopGame.Instance.OnGameStarted();
        }

        public override void OnCursorDown(Vector2 pos)
        {
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
        
        public override void OnCreate()
        {
           
        }

        public override void OnDestroy()
        {
            _gameEngine.Unload();
            base.OnDestroy();
        }


        public override void Update(GameTime gameTime)
        {
            _gameEngine.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (Components.Contains(_gameplayPanel))
                {
                    Components.Remove(_gameplayPanel);
                    Components.Add(_pausePanel);
                    
                    //_screen.PauseGame();
                }
                else if (Components.Contains(_pausePanel))
                {
                    Components.Remove(_pausePanel);
                    Components.Add(_gameplayPanel);
                    //_screen.ResumeGame();
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
            base.Draw(gameTime, spritebatch);
            _gameEngine.Draw(gameTime);
        }

        #region Implementation of SceneInterface;

        private ObjectManager _objectManager;

        private void InitializeSceneInterface()
        {
            _objectManager = new ObjectManager(LoopGame.Instance.Components);
        }

        public ObjectManager ObjectManager
        {
            get { return _objectManager; }
        }

        private List<Scene> _scenes = new List<Scene>();

        public void Submit(Scene scene)
        {
            foreach (SceneEntity obj in scene.Objects)
            {
                ObjectManager.Submit(obj);
            }
            _scenes.Add(scene);
            scene.SceneInterface = this;
        }

        public void Remove(Scene scene)
        {
            foreach (SceneEntity obj in scene.Objects)
            {
                ObjectManager.Remove(obj);
            }
            _scenes.Remove(scene);
            scene.SceneInterface = null;
        }

        #endregion Implementation of SceneInterface;
    }
}