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
using GhostlyGame.Models;
using GhostlyLib.DynamicDifficulty;
using GhostlyLib.Level;
using GhostlyLib.Screens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Data;
using System.Diagnostics;
using Vub.Etro.IO;
using Vector4 = Vub.Etro.IO.Vector4;

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

        //private Label _scoreLabel;
        private int score = 0;

        private string _configuration;

        private GameScreen _screen;

        public GamePlayActivity(UIEngine engine, int level, string intputConfig) : base(engine)
        {
            if (level >= 161 && level <= 190) //3D maze levels
            {
                _screen = new GameScreen3D(level, engine.MusicPlayer, engine.Screen, engine.Device);
            }
            else    //2D levels
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

            TextButton backButton = new TextButton(LocalizationResourceManager.Instance["BackToMenu"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            backButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
            {
                //TODO for Ghostly+ study
                if (GameSessionInfo.Instance.SelectedPatient != null)
                {
                    StartActivity(new StartGameActivity(engine));
                }
                else
                {
                    StartActivity(new MainMenuActivity(engine));
                }

                _engine.MusicPlayer.Play("menu");
            };
            backButton.Position = engine.Screen.ScreenMiddle - backButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

            TextButton playAgainButton = new TextButton(LocalizationResourceManager.Instance["PlayAgain"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            playAgainButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
            {
                Components.Remove(_gameOverPanel);
                _screen.Initialize();   //to initalize background
                _screen.LoadLevel();
            };
            playAgainButton.Position = engine.Screen.ScreenMiddle - playAgainButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

            Label gameOverLabel = new Label(LocalizationResourceManager.Instance["GameOver"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            gameOverLabel.Position = engine.Screen.ScreenMiddle - gameOverLabel.Size / 2 - new Vector2(0, engine.Screen.ScreenMiddle.Y * 2 / 3); ;

            _gameOverPanel.Components.Add(gameOverLabel);
            _gameOverPanel.Components.Add(playAgainButton);
            _gameOverPanel.Components.Add(backButton);
            #endregion Game Over Panel

            #region Level Done Panel
            LoadLevelDonePanel(engine);

            #endregion Level Done Panel

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
            {
                for (int x = 0; x < 6; x++)
                {
                    if (assesmentValue > 10)
                        continue;

                    TextButton assesmentButton = new TextButton(assesmentValue.ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
                    assesmentButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
                    {
                        if (GameSessionInfo.Instance.Session != null)
                        {
                            GameSessionInfo.Instance.Session.Rpe_post_session = Int16.Parse(assesmentButton.Text.ToString());
                        }
                        Components.Remove(_selfAssesmentPanel);
                        Components.Add(_bfrVopPanel);
                    };
                    assesmentButton.Position = (offset + new Vector2(x * (tileWidth + horizontalSpacing), y * (tileHeight + verticalSpacing)));
                    assesmentButton.Size = new Vector2(tileWidth, tileHeight);
                    assesmentValue++;
                    _selfAssesmentPanel.Components.Add(assesmentButton);
                }
            }

            Label selfAssesmentLabel = new Label(LocalizationResourceManager.Instance["RatedPerceivedExertion"].ToString().ToUpper(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
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

            Label percentageBFRLeftLabel = new Label("50% ", engine.Content.LoadFont(GhostlyGame.MENU_STANDARD_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), Color.White);
            percentageBFRLeftLabel.Position = emgImage1.Position + emgImage1.Size - percentageBFRLeftLabel.Size;
            _bfrVopPanel.Components.Add(percentageBFRLeftLabel);

            Label percentageBfrRightLabel = new Label("50% ", engine.Content.LoadFont(GhostlyGame.MENU_STANDARD_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), Color.White);
            percentageBfrRightLabel.Position = emgImage2.Position + emgImage2.Size - percentageBfrRightLabel.Size;
            _bfrVopPanel.Components.Add(percentageBfrRightLabel);

            DraggableButton bfrLeftVOPButton = new DraggableButton(LocalizationResourceManager.Instance["LeftVOP"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device, 0.5f);
            bfrLeftVOPButton.MinY = emgImage1.Position.Y;
            bfrLeftVOPButton.MaxY = emgImage1.Position.Y + emgImage1.Size.Y;
            bfrLeftVOPButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { };
            bfrLeftVOPButton.Position = new Vector2((int)(engine.Screen.ScreenWidth * 0.3), cell * 2 + emgImage1.Size.Y / 2) - bfrLeftVOPButton.Size / 2;
            bfrLeftVOPButton.PercentageChanged += (float value) => { percentageBFRLeftLabel.Text = (value * 100).ToString("00.") + "%"; };
            _bfrVopPanel.Components.Add(bfrLeftVOPButton);

            DraggableButton bfrRightVOPButton = new DraggableButton(LocalizationResourceManager.Instance["RightVOP"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device, 0.5f);
            bfrRightVOPButton.MinY = emgImage2.Position.Y;
            bfrRightVOPButton.MaxY = emgImage2.Position.Y + emgImage2.Size.Y;
            bfrRightVOPButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { };
            bfrRightVOPButton.Position = new Vector2((int)(engine.Screen.ScreenWidth * 0.7), cell * 2 + emgImage2.Size.Y / 2) - bfrRightVOPButton.Size / 2;
            bfrRightVOPButton.PercentageChanged += (float value) => { percentageBfrRightLabel.Text = (value * 100).ToString("00.") + "%"; };
            _bfrVopPanel.Components.Add(bfrRightVOPButton);

            TextButton okButton = new TextButton(LocalizationResourceManager.Instance["Ok"].ToString().ToUpper(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            okButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
            {
                //TODO save values from sliders
                if (GameSessionInfo.Instance.Session != null)
                {
                    GameSessionInfo.Instance.Session.BFR_target_vop_percentage_ch1 = bfrLeftVOPButton.Percentage;
                    GameSessionInfo.Instance.Session.BFR_target_vop_percentage_ch2 = bfrRightVOPButton.Percentage;

                    // write to c3d
                    UpdateC3D();

                    //Evaluate the level - > check analytics
                    /*if (_screen.Level.GetType() == typeof(MazeLevel3D))
                    {
                        int evaluation = ((MazeLevel3D)_screen.Level).Analytics.Evaluate();

                        //TODO
                        //load the c3d file, process it and calculate performance score

                        _screen.UpdateRequiredContractionDuration(evaluation);
                    }
                    else */
                    if (_screen.Level.GetType() == typeof(SimpleSpaceLevel))
                    {
                        int evaluation = ((SimpleSpaceLevel)_screen.Level).Analytics.Evaluate();
                        _screen.UpdateDifficultyLevel(evaluation);
                    }
                }

                Components.Remove(_bfrVopPanel);
                //TODO the panel needs to be loaded, since it's content changes based on session
                LoadLevelDonePanel(engine);
                Components.Add(_levelDonePanel);

            };
            okButton.Position = new Vector2((int)(engine.Screen.ScreenWidth * 0.5) - (okButton.Size.X / 2), (int)(engine.Screen.ScreenHeight * 0.8));
            _bfrVopPanel.Components.Add(okButton);

            Label bfrVopValuesLabel = new Label(LocalizationResourceManager.Instance["AreBFRVOPValuesCorrect"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            bfrVopValuesLabel.Position = new Vector2((int)((engine.Screen.ScreenWidth - bfrVopValuesLabel.Size.X) / 2), (int)(engine.Screen.ScreenHeight * 0.1));

            _bfrVopPanel.Components.Add(bfrVopValuesLabel);
            #endregion

            #region Gameplay Panel
            _gameplayPanel = new ComponentCollection();
            _gameplayPanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);

            TextButton pauseButton = new TextButton("X", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device); //new TextButton("\uf04c", engine.Content.LoadFont("Fonts/Awesome" + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
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

            #region Pause Panel
            _pausePanel = new ComponentCollection();
            _pausePanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);

            Texture2D texture = new Texture2D(engine.Device, 1, 1);
            texture.SetData(new Color[] { Color.FromNonPremultiplied(0, 0, 0, 170) });
            Image background = new Image(texture);
            background.Size = _pausePanel.Size;

            TextButton exitButton = new TextButton(LocalizationResourceManager.Instance["Exit"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
            exitButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
            {
                _screen.Exit();
                //TODO for Ghostly+ study
                if (GameSessionInfo.Instance.SelectedPatient != null)
                {
                    StartActivity(new StartGameActivity(engine));
                }
                else
                {
                    StartActivity(new MainMenuActivity(engine));
                }

                _engine.MusicPlayer.Play("menu");
            };
            exitButton.Position = engine.Screen.ScreenMiddle - exitButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

            TextButton continueButton = new TextButton(LocalizationResourceManager.Instance["Continue"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
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
        }

        private void LoadLevelDonePanel(UIEngine engine)
        {
            _levelDonePanel = new ComponentCollection();
            _levelDonePanel.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);

            //this is prepared before this level is finished, we check if completed levels + this one equal required number of levels
            if (GameSessionInfo.Instance.LevelsCompleted == GameSessionInfo.Instance.RequiredLevels)
            {
                Label sessionCompleted = new Label(LocalizationResourceManager.Instance["SessionCompleted"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
                sessionCompleted.Position = new Vector2(engine.Screen.ScreenMiddle.X - (sessionCompleted.Size.X / 2), engine.Screen.ScreenMiddle.Y - 35);
                _levelDonePanel.Components.Add(sessionCompleted);

                Label turnOffTheGame = new Label(LocalizationResourceManager.Instance["TurnOffTheGame"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
                turnOffTheGame.Position = new Vector2(engine.Screen.ScreenMiddle.X - (turnOffTheGame.Size.X / 2), engine.Screen.ScreenMiddle.Y + 35);
                _levelDonePanel.Components.Add(turnOffTheGame);
            }
            else
            {
                TextButton nextButton = new TextButton(LocalizationResourceManager.Instance["NextLevel"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
                nextButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
                {
                    Components.Remove(_levelDonePanel);

                    if (_screen.CurrentLevel == 160 || _screen.CurrentLevel == 190)
                    {
                        //levels 161+ are 3D levels, and levels 191+ are again 2D levels
                        //we need to go back to main menu, to select maze game explicitly, to initialize GameScreen3D or GameScreen2D,
                        StartActivity(new MainMenuActivity(engine));
                    }
                    else
                    {
                        //otherwise we remain in the same GameScreen 2D for levels up to 160, and 3D for levels 161 and higher
                        _screen.LoadNextLevel();
                    }
                };
                nextButton.Position = engine.Screen.ScreenMiddle - nextButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

                _levelDonePanel.Components.Add(nextButton);

                TextButton backButton = new TextButton(LocalizationResourceManager.Instance["BackToMenu"].ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
                backButton.Clicked += (object sender, TextButton.ClickedEventArgs e) =>
                {
                    //TODO for Ghostly+ study
                    if (GameSessionInfo.Instance.SelectedPatient != null)
                    {
                        StartActivity(new StartGameActivity(engine));
                    }
                    else
                    {
                        StartActivity(new MainMenuActivity(engine));
                    }

                    _engine.MusicPlayer.Play("menu");
                };
                backButton.Position = engine.Screen.ScreenMiddle - backButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

                Label levelDoneLabel = new Label(LocalizationResourceManager.Instance["LevelCompleted"].ToString().ToUpper(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
                levelDoneLabel.Position = engine.Screen.ScreenMiddle - levelDoneLabel.Size / 2 - new Vector2(0, engine.Screen.ScreenMiddle.Y * 2 / 3);

                Label _scoreLabel = new Label(LocalizationResourceManager.Instance["Score"].ToString() + ": " + score, engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
                _scoreLabel.Position = engine.Screen.ScreenMiddle - _scoreLabel.Size / 2 + new Vector2(0, -engine.Screen.ScreenMiddle.Y / 3);

                _levelDonePanel.Components.Add(backButton);
                _levelDonePanel.Components.Add(levelDoneLabel);
                _levelDonePanel.Components.Add(_scoreLabel);
            }
        }

        private void UpdateC3D()
        {
            if (GameSessionInfo.Instance.Session == null || GameSessionInfo.Instance.SelectedPatient == null)
            {
                Debug.WriteLine("No data to be written to the file, update canceled!");
                return;
            }

            //get the name of the last written c3d
            var _c3dFile = SeriousGames.LastC3DFileCreated;

            if (_c3dFile.IsNullOrEmpty())
            {
                Debug.WriteLine("No C3D file was found.");
                return;
            }

            // read file
            C3dReader reader = new C3dReader();

            if (!reader.Open(_c3dFile))
            {
                throw new ApplicationException("Could not open file " + (_c3dFile) + "!");
            }

            //existing file is loaded with all it's header, parameters, events, and data
            C3dWriter writer = new C3dWriter(reader, false);

            //add prameters
            writer.SetParameter<float>("INFO:BFR_TARGET_VOP_PERCENTAGE_CH1", GameSessionInfo.Instance.Session.BFR_target_vop_percentage_ch1);
            writer.SetParameter<float>("INFO:BFR_TARGET_VOP_PERCENTAGE_CH2", GameSessionInfo.Instance.Session.BFR_target_vop_percentage_ch2);
            //writer.SetParameter<float>("INFO:target_contractions_ch1)", (float)GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh1Ms);
            //writer.SetParameter<float>("INFO:target_contractions_ch2)", (float)GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh2Ms);

            float contractionDuration = (float)DifficultyLevelStateSpace.Instance.getLevelDefinition((int)GameSessionInfo.Instance.SelectedPatient.DifficultyLevel).contractionDuration / 1000;

            writer.SetParameter<float>("INFO:TARGET_CONTRACTIONS_CH1)", (float)contractionDuration);
            writer.SetParameter<float>("INFO:TARGET_CONTRACTIONS_CH2)", (float)contractionDuration);

            writer.SetParameter<Int16>("INFO:RPE_POST_SESSION", GameSessionInfo.Instance.Session.Rpe_post_session);

            int difficultyLevel = (int)GameSessionInfo.Instance.SelectedPatient.DifficultyLevel;

            writer.SetParameter<Int16>("INFO:DIFFICULTY_LEVEL", (Int16)difficultyLevel);
            writer.SetParameter<float>("INFO:DIFFICULTY_LEVEL_MVC", DifficultyLevelStateSpace.Instance.getLevelDefinition(difficultyLevel)._MVCLevel);
            writer.SetParameter<Int16>("INFO:DIFFICULTY_LEVEL_CONTRACTION_DURATION", (Int16)DifficultyLevelStateSpace.Instance.getLevelDefinition(difficultyLevel).contractionDuration);
            writer.SetParameter<Int16>("INFO:DIFFICULTY_LEVEL_REST_DURATION", (Int16)DifficultyLevelStateSpace.Instance.getLevelDefinition(difficultyLevel).restDuration);

            IEmgSensorInput _emgInput = GameSessionInfo.Instance.GetSensorInput();
            writer.SetParameter<float>("INFO:ACTIVATION_THRESHOLD_CH1", _emgInput.ActivationThreshold[0]);
            writer.SetParameter<float>("INFO:ACTIVATION_THRESHOLD_CH2", _emgInput.ActivationThreshold[1]);

            //TODO rethink this!!!!!!!!!!!!!!!!!!!!!!
            var new_c3dFile = SeriousGames.LastC3DFileCreated;
            new_c3dFile = new_c3dFile.Replace(".c3d", ".new.c3d");
            writer.Open(new_c3dFile);
            //float[] analogData = new float[reader.AnalogLabels.Count * reader.AnalogChannels];
            //short[] analogData_int = new short[reader.AnalogLabels.Count * reader.AnalogChannels];

            for (int i = 0; i < reader.FramesCount; i++)
            {
                Vector4[] points = reader.ReadFrame();
                AnalogDataArray adr = reader.AnalogData;

                if (reader.IsFloat)
                {
                    writer.WriteFloatFrame(points);

                    float[] analogData = new float[reader.AnalogLabels.Count];

                    for (int j = 0; j < reader.AnalogChannels; j++)
                    {
                        for (int k = 0; k < reader.AnalogLabels.Count; k++)
                        {
                            analogData[k] = reader.AnalogData.Data[k, j];
                        }

                        writer.WriteFloatAnalogData(analogData);
                    }
                }
                else if (reader.IsInterger)
                {
                    writer.WriteIntFrame(points);

                    short[] analogData = new short[reader.AnalogLabels.Count];

                    for (int j = 0; j < reader.AnalogChannels; j++)
                    {
                        for (int k = 0; k < reader.AnalogLabels.Count; k++)
                        {
                            analogData[k] = (short)reader.AnalogData.Data[k, j];
                        }

                        writer.WriteIntAnalogData(analogData);
                    }
                }
            }

            //close reader
            reader.Close();

            //re-write the file
            writer.Close();

            //re write the old with new file
            System.IO.File.Move(new_c3dFile, _c3dFile, true);

            //upload c3d file to the server
            Task<bool> t = GameSessionInfo.Instance.Uploader.UploadC3DFile(_c3dFile, GameSessionInfo.Instance.SelectedPatient.PatientCode);
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
                //_scoreLabel.Text = LocalizationResourceManager.Instance["Score"].ToString() + ": " + e.Score;
                score = e.Score;
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