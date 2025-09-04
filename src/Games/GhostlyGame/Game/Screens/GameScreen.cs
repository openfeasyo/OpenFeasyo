/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Katarina Kostkoa
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using GhostlyLib.Animations;
using GhostlyLib.Elements;
using GhostlyLib.Elements.Character;
using GhostlyLib.Level;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.GameTools.Core;
using OpenFeasyo.GameTools.Effects;
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Drivers;
using System.Diagnostics;

namespace GhostlyLib.Screens
{
    public abstract class GameScreen : Screens.Screen
    {
        public const int CONST_SPEED = 3;
        public abstract float SPEED { get; }// { get { return 3f; } }

        //#region Private members

        //private GameState? _gameState = null;
        public GameState? State { get; protected set; }
        //private double _checkpoint = 0;

        //private SpriteFont _font24, _font36, _font48;
        //private int _currentLevel = 1;
        public int CurrentLevel = 1;
        
        private IDevice _emgDevice;
        private IEmgSensorInput _emgInput;
        //private ILevel _level;
        public ILevel Level;

        protected double Checkpoint = 0;

        //private LevelElements _elements;
        public LevelElements Elements;

        public State EmgState = new State(false, false);

        private SpriteFont[] _font = new SpriteFont[4];
        public SpriteFont[] Font { get { return _font; } }

        //private Vector3 Position { get; set; }

        //#endregion Private members

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

            ///_gameState = GameState.DeviceConnected;
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
            this.CurrentLevel += 1;
            this.CurrentLevel = Math.Min(this.CurrentLevel, 180); // Max level cannot exceed 180
            LoadLevel();
        }
        
        public abstract void LoadLevel();
        /*public void LoadLevel()
        {
            Debug.WriteLine("reloading level, checkpoint " + _checkpoint);
            this._gameState = GameState.Running;
            //clear elements & start logging
            this._elements = new LevelElements();
            OnGameStarted(_currentLevel);

            if (this._currentLevel <= 30)
            {
                this._level = new EarthLevel(this, this._elements);
                this._level.LoadMap("earth.map" + this._currentLevel + ".txt", _checkpoint);
                GameBackground.SetParallaxLayers(new List<Texture2D> { _level.BackgroundFurthest, _level.BackgroundFurther, _level.BackgroundFar, _level.BackgroundClose, _level.BackgroundCloser, _level.BackgroundClosest });
            }
            else if (this._currentLevel <= 60)
            {
                this._level = new WaterLevel(this, this._elements);
                this._level.LoadMap("water.map" + this._currentLevel + ".txt", _checkpoint);
                GameBackground.SetParallaxLayers(new List<Texture2D> { _level.BackgroundClose, _level.BackgroundCloser, _level.BackgroundClosest });
            }
            else if (this._currentLevel <= 90)
            {
                this._level = new RockLevel(this, this._elements);
                this._level.LoadMap("rock.map" + this._currentLevel + ".txt", _checkpoint);
                GameBackground.SetParallaxLayers(new List<Texture2D> { _level.BackgroundFurthest, _level.BackgroundClose, _level.BackgroundClosest });

            }
            else if (this._currentLevel <= 120)
            {
                this._level = new IceLevel(this, this._elements);
                this._level.LoadMap("ice.map" + this._currentLevel + ".txt", _checkpoint);
                GameBackground.SetParallaxLayers(new List<Texture2D> { _level.BackgroundFurthest, _level.BackgroundClose, _level.BackgroundClosest });
            }
            else if (this._currentLevel <= 140)
            {
                this._level = new RockLevel(this, this._elements);
                this._level.LoadMap("land.map" + this._currentLevel + ".txt", _checkpoint);
                GameBackground.SetParallaxLayers(new List<Texture2D> { _level.BackgroundFurthest, _level.BackgroundClose, _level.BackgroundClosest });
            }
            else if (this._currentLevel <= 160)
            {
                this._level = new SpaceLevel(this, this._elements);
                this._level.LoadMap("space.map" + this._currentLevel + ".txt", _checkpoint);
                GameBackground.SetParallaxLayers(new List<Texture2D> { _level.BackgroundFurthest, _level.BackgroundClose, _level.BackgroundCloser });//, _level.BackgroundClosest });
            }

            this.GameCharacter = this._level.Character;
            GameBackground.ContinuousLayer = this._level.Background;
            GhostlyActionHandlers.CurrentLevel = this._level;

            Position = Vector3.Zero;
            GhostlyGame.Instance.GameObjects.TryUpdate("PlayerPosition", Position);
        }*/
        #endregion Loaders

        public void UnloadContent()
        {
        }

        /*public void Update(GameTime gameTime)
        {
            Position = new Vector3((float)(Position.X - GameBackground.HorizontalSpeed), (float)(Screen.Height - Level.Character.Y), GameBackground.HorizontalSpeed);
            GhostlyGame.Instance.GameObjects.TryUpdate("PlayerPosition", Position + new Vector3((float)(((Drawable)GameCharacter).X + GameCharacter.Width / 2), 0, 0));
            KeyboardUpdate();

            if (State.Equals(GameState.Running))
            {
                UpdateAllElements(gameTime);
            }
        }*/

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

        
        /*private void UpdateAllElements(GameTime gameTime)
        {
            this.Level.ProcessPrimaryAction(emgState.Primary);
            this.Level.ProcessSecondaryAction(emgState.Secondary);

            this.GameCharacter.Update(gameTime);

            this.Elements.Update(gameTime);

            UpdateOnetimeAnimations();

            this.GameBackground.Update(gameTime);
        }*/

        private void UpdateOnetimeAnimations()
        {
            foreach (OnetimeAnimation anim in OnetimeAnimations)
            {
                if (anim.IsVisible)
                {
                    anim.Update(8);
                }
            }
            OnetimeAnimations.RemoveAll(o => o.IsVisible == false);
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
        }

        public abstract void SetCheckpoint(double checkpoint);
        /*{
            Debug.WriteLine("checkpoint " + checkpoint);
            this._checkpoint = checkpoint;
            this.State = GameState.Running;
        }*/

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
            //Debug.WriteLine("Game Finished - Score: " + score);
            if (GameFinished != null)
            {
                GameFinished(this, new GameFinishedEventArgs("", score, level, reason));
            }
        }

        public abstract void Update(GameTime gameTime);
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