/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Katarina Kostkova
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using GhostlyGame.Models;
using GhostlyLib.Animations;
using GhostlyLib.Elements;
using GhostlyLib.Elements.Character;
using GhostlyLib.Level;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.GameTools.Core;
using OpenFeasyo.GameTools.Effects;
using OpenFeasyo.Platform.Controls;
using System.Diagnostics;

namespace GhostlyLib.Screens
{
    public abstract class GameScreen : Screens.Screen
    {
        public const int CONST_SPEED = 3;
        public abstract float SPEED { get; }// { get { return 3f; } }

        #region Private members
        //private IDevice _emgDevice;
        //private IEmgSensorInput _emgInput;
        private SpriteFont[] _font = new SpriteFont[4];
        #endregion Private members

        protected double Checkpoint = 0;

        public GameState? State { get; protected set; }
        public int CurrentLevel = 1;

        public ILevel Level;

        public LevelElements Elements;

        public State EmgState = new State(false, false);

        public SpriteFont[] Font { get { return _font; } }


        #region Public properties
        public IGameCharacter GameCharacter;
        public GameBackground GameBackground { get; set; }
        public MusicPlayer MusicPlayer { get; private set; }

        public List<OnetimeAnimation> OnetimeAnimations = new List<OnetimeAnimation>();

        public OpenFeasyo.GameTools.Screen Screen { get; private set; }


        public int Completion
        {
            get
            {
                return (int)(((double)GameCharacter.Score / Level.MaxScore) * 100);
            }
        }

        #endregion Public properties

        public GameScreen(int level, MusicPlayer player, OpenFeasyo.GameTools.Screen screen)
        {
            this.CurrentLevel = level;
            this.MusicPlayer = player;
            this.Screen = screen;

            //_gameState = GameState.DeviceConnected;
            State = GameState.DeviceTrained;
        }

        /*public void Initialize()
        {
            GameBackground = new GameBackground(0, SPEED, Screen);
        }*/

        public abstract void Initialize();

        #region Loaders

        public void LoadContent(ContentRepository content)
        {
            _font[0] = content.LoadFont("Fonts/Vitamin12");
            _font[1] = content.LoadFont("Fonts/Vitamin24");
            _font[2] = content.LoadFont("Fonts/Vitamin36");
            _font[3] = content.LoadFont("Fonts/Vitamin48");

            try
            {
                LoadLevel();
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void LoadNextLevel()
        {
            //TODO Ghostly+ study...only levels 191 - 200 can be played
            if (GameSessionInfo.Instance.SelectedPatient != null)
            {
                this.CurrentLevel = GameSessionInfo.Instance.LevelToPlay();
            }
            else
            {
                this.CurrentLevel += 1;
                this.CurrentLevel = Math.Min(this.CurrentLevel, 224);
            }
            LoadLevel();
        }

        public abstract void LoadLevel();
        #endregion Loaders

        public void UnloadContent() { }

        public void Exit()
        {
            if (State.Equals(GameState.Running))
            {
                State = GameState.Paused;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (State.Equals(GameState.Running) || State.Equals(GameState.Paused))
            {
                DrawGameplay(spriteBatch, gameTime);
            }
        }

        protected abstract void DrawGameplay(SpriteBatch spriteBatch, GameTime gameTime);

        public void PauseGame()
        {
            if (this.State.Equals(GameState.Running))
            {
                this.State = GameState.Paused;
            }
        }

        public void ResumeGame()
        {
            if (this.State.Equals(GameState.Paused))
            {
                this.State = GameState.Running;
            }
        }

        public void GameOver()
        {
            ((GameCharacter)this.GameCharacter).Stop();
            this.State = GameState.GameOver;
            OnGameFinished(GameCharacter.Score, CurrentLevel, GameFinishedEventArgs.EndReason.GameFailed);
        }

        public void LevelDone()
        {
            MusicPlayer.PlayEffect("win");
            this.Checkpoint = 0;
            ((IGameCharacter)this.GameCharacter).Stop();
            this.State = GameState.LevelDone;
            OnGameFinished(GameCharacter.Score, CurrentLevel, GameFinishedEventArgs.EndReason.GoalAccomplished);
            GameSessionInfo.Instance.LevelsCompleted++;
        }

        public abstract void SetCheckpoint(double checkpoint);

        public event EventHandler<GameStartedEventArgs> GameStarted;
        protected void OnGameStarted(int level)
        {
            if (GameStarted != null)
            {
                GameStarted(this, new GameStartedEventArgs(GhostlyGame.Instance.Definition.Name, level));
            }
        }

        public event EventHandler<GameFinishedEventArgs> GameFinished;

        public void OnGameFinished(int score, int level, GameFinishedEventArgs.EndReason reason)
        {
            if (GameFinished != null)
            {
                GameFinished(this, new GameFinishedEventArgs("", score, level, reason));
            }
        }

        public abstract void Update(GameTime gameTime);


        /*public void UpdateRequiredContractionDuration(int evaluation)
        {
            if (evaluation < 0)
            {
                //decrease
                GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh1Ms = Math.Max((float)GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh1Ms - 1000, 3000);
                GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh2Ms = Math.Max((float)GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh2Ms - 1000, 3000);
            }
            else if (evaluation > 0)
            {
                //increase
                GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh1Ms = Math.Min((float)GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh1Ms + 1000, 10000);
                GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh2Ms = Math.Min((float)GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh2Ms + 1000, 10000);
            }
            else { }//no change   
        }*/

        public void UpdateDifficultyLevel(int evaluation)
        {
            if (evaluation < 0)
            {
                //decrease
                if (GameSessionInfo.Instance.SelectedPatient.DifficultyLevel != null)
                {
                    GameSessionInfo.Instance.SelectedPatient.DifficultyLevel = Math.Max((int)GameSessionInfo.Instance.SelectedPatient.DifficultyLevel - 1, 1);
                }
                else
                {
                    GameSessionInfo.Instance.SelectedPatient.DifficultyLevel = 1;
                }
            }
            else if (evaluation > 0)
            {
                //increase
                if (GameSessionInfo.Instance.SelectedPatient.DifficultyLevel != null)
                {
                    GameSessionInfo.Instance.SelectedPatient.DifficultyLevel = Math.Min((int)GameSessionInfo.Instance.SelectedPatient.DifficultyLevel + 1, 27);
                }
                else
                {
                    GameSessionInfo.Instance.SelectedPatient.DifficultyLevel = 2;
                }

            }
            else { }    //no change
        }
    }

    public class State
    {
        internal bool Primary { get; }
        internal bool Secondary { get; }
        public State(bool primary, bool secondary)
        {
            this.Primary = primary;
            this.Secondary = secondary;
        }
    }
}