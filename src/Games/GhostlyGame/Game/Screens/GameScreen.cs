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
using Microsoft.Xna.Framework.Input;
using OpenFeasyo.GameTools.Core;
using OpenFeasyo.GameTools.Effects;
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Drivers;
using System;
using System.Collections.Generic;
using System.IO;

namespace GhostlyLib.Screens
{
    public class GameScreen : Screens.Screen
    {
        public const int SPEED = 3;

        #region Private members

        private GameState? _gameState = null;

        //private SpriteFont _font24, _font36, _font48;
        private int _currentLevel = 1;

        private IDevice _emgDevice;
        private IEmgSensorInput _emgInput;
        private ILevel _level;

        private LevelElements _elements;

        private State emgState = new State(false,false);

        private SpriteFont[] _font = new SpriteFont[4];

        private Vector3 Position { get; set; }
        
        #endregion Private members

        #region Public properties
        public GameCharacter GameCharacter;
        public GameBackground GameBackground { get; private set; }
        public MusicPlayer MusicPlayer { get; private set; } 

        public List<OnetimeAnimation> OnetimeAnimations = new List<OnetimeAnimation>();

        public OpenFeasyo.GameTools.Screen Screen { get; private set; }


        public int Completion
        {
            get
            {
                return (int)(((double)GameCharacter.Score / _level.MaxScore) * 100);
            }
        }

        #endregion Public properties

        public GameScreen(int level,  MusicPlayer player, OpenFeasyo.GameTools.Screen screen)
        {
            this._currentLevel = level;
            this.MusicPlayer = player;
            this.Screen = screen;
            
            ///_gameState = GameState.DeviceConnected;
            _gameState = GameState.DeviceTrained;

        }

        public void Initialize()
        {
            GameBackground = new GameBackground(0,SPEED, Screen);
            
        }

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
                Console.WriteLine(e.Message);
            }
        }

        public void LoadNextLevel() {
            this._currentLevel += 1;
            LoadLevel();
        }



        public void LoadLevel()
        {
            
            this._gameState = GameState.Running;
            //clear elements & start logging
            this._elements = new LevelElements();
            OnGameStarted(_currentLevel);

            if (this._currentLevel <= 30)
            {
                this._level = new EarthLevel(this, this._elements);
                this._level.LoadMap("earth.map" + this._currentLevel + ".txt");
                GameBackground.SetParallaxLayers(new List<Texture2D> { _level.BackgroundFurthest, _level.BackgroundFurther, _level.BackgroundFar, _level.BackgroundClose, _level.BackgroundCloser, _level.BackgroundClosest });
            }
            else if (this._currentLevel <= 60)
            {
                this._level = new WaterLevel(this, this._elements);
                this._level.LoadMap("water.map" + this._currentLevel + ".txt");
                GameBackground.SetParallaxLayers(new List<Texture2D> { _level.BackgroundClose, _level.BackgroundCloser, _level.BackgroundClosest });
            }
            else if (this._currentLevel <= 90)
            {
                this._level = new RockLevel(this, this._elements);
                this._level.LoadMap("rock.map" + this._currentLevel + ".txt");
                GameBackground.SetParallaxLayers(new List<Texture2D> { _level.BackgroundFurthest, _level.BackgroundClose, _level.BackgroundClosest });

            }
            this.GameCharacter = this._level.Character;
            GameBackground.ContinuousLayer = this._level.Background;
            GhostlyActionHandlers.CurrentLevel = this._level;

            Position = new Vector3((float)_level.Character.X, (float)(Screen.Height-_level.Character.Y),0);
            GhostlyGame.Instance.GameObjects.TryUpdate("PlayerPosition", Position);
        }
        #endregion Loaders

        public void UnloadContent()
        {
        }

        
        public void Update(GameTime gameTime)
        {
            Position = new Vector3(Position.X - GameBackground.HorizontalSpeed, (float)(Screen.Height-_level.Character.Y), GameBackground.HorizontalSpeed);
            GhostlyGame.Instance.GameObjects.TryUpdate("PlayerPosition", Position);
            KeyboardUpdate();
            
            if (_gameState.Equals(GameState.Running))
            {
                UpdateAllElements(gameTime);
            }
        }
        
        
        public void Exit()
        {
            if (_gameState.Equals(GameState.Running))
            {
                _gameState = GameState.Paused;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_gameState.Equals(GameState.Running) || _gameState.Equals(GameState.Paused))
            {
                DrawGameplay(spriteBatch, gameTime);
            }
        }

        private void UpdateAllElements(GameTime gameTime)
        {
            this._level.ProcessPrimaryAction(emgState.Primary);
            this._level.ProcessSecondaryAction(emgState.Secondary);

            this.GameCharacter.Update(gameTime);

            this._elements.Update(gameTime);

            UpdateOnetimeAnimations();

            this.GameBackground.Update(gameTime);
        }

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

        private void DrawGameplay(SpriteBatch spriteBatch, GameTime gameTime)
        {
            GameBackground.Draw(gameTime,spriteBatch);

            this._elements.Draw(spriteBatch);

            this.GameCharacter.Draw(spriteBatch);

            DrawOnetimeAnimations(spriteBatch);

            spriteBatch.DrawString(_font[2], "Level: " + this._currentLevel.ToString(), new Vector2(10, 20), GhostlyGame.MENU_FONT_COLOR);

            switch (this.GameCharacter.CurrentHealth)
            {
                case 3:
                    spriteBatch.Draw(ImagesAndAnimations.Instance.HeartFull, new Rectangle(600, 20, 53, 45), Color.White);
                    break;
                case 2:
                    spriteBatch.Draw(ImagesAndAnimations.Instance.HeartHalf, new Rectangle(600, 20, 53, 45), Color.White);
                    break;
                case 1:
                    spriteBatch.Draw(ImagesAndAnimations.Instance.HeartEmpty, new Rectangle(600, 20, 53, 45), Color.White);
                    break;
                default:
                    spriteBatch.Draw(ImagesAndAnimations.Instance.InvisibleTile, new Rectangle(600, 20, 53, 45), Color.White);
                    break;
            }

            spriteBatch.DrawString(_font[2], "Score: " + GameCharacter.Score.ToString(), new Vector2(1050, 20), GhostlyGame.MENU_FONT_COLOR);
        }

        private void DrawOnetimeAnimations(SpriteBatch spriteBatch)
        {
            foreach (OnetimeAnimation anim in OnetimeAnimations)
            {
                spriteBatch.Draw(anim.Image, Screen.ToScreen(anim.X, anim.Y, anim.Width, anim.Height), Color.White);
            }
        }

        private void KeyboardUpdate()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Up) && this._gameState.Equals(GameState.Running))
            {
                if (_level is EarthLevel || _level is RockLevel)
                {
                    this.GameCharacter.Jump();
                }
                else if (_level is WaterLevel)
                {
                    this.GameCharacter.Swimming();
                }
            }
            else if (state.IsKeyDown(Keys.LeftAlt) && this._gameState.Equals(GameState.Running))
            {
                //this.GameCharacter.Shoot();
                GhostlyActionHandlers.SecondaryActionHandle(0, 1);
            }
        }

        public void PauseGame()
        {
            if (this._gameState.Equals(GameState.Running))
            {
                this._gameState = GameState.Paused;
            }
            
        }

        public void ResumeGame() {
            if (this._gameState.Equals(GameState.Paused))
            {
                this._gameState = GameState.Running;
            }
        }

        public void GameOver()
        {
            this.GameCharacter.Stop();
            this._gameState = GameState.GameOver;
            OnGameFinished(GameCharacter.Score, GameFinishedEventArgs.EndReason.GameFailed);
        }

        public void LevelDone()
        {
            MusicPlayer.PlayEffect("win");
            this.GameCharacter.Stop();
            this._gameState = GameState.LevelDone;
            OnGameFinished(GameCharacter.Score, GameFinishedEventArgs.EndReason.GoalAccomplished);
        }

        public event EventHandler<GameStartedEventArgs> GameStarted;
        private void OnGameStarted(int level)
        {
            if (GameStarted != null)
            {
                GameStarted(this, new GameStartedEventArgs(GhostlyGame.Instance.Definition.Name,level));
            }
        }

        public event EventHandler<GameFinishedEventArgs> GameFinished;

        public void OnGameFinished(int score, GameFinishedEventArgs.EndReason reason)
        {
            Console.WriteLine("Game Finished - Score: " + score);
            if (GameFinished != null)
            {
                GameFinished(this, new GameFinishedEventArgs("", score, reason));
            }
        }

    }

    class State
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
