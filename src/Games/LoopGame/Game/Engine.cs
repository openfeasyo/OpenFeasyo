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
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Data;
using LoopLib.World;
using LoopLib.World.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenFeasyo.GameTools;
using OpenFeasyo.GameTools.Components2D;
using OpenFeasyo.GameTools.Core;
using OpenFeasyo.GameTools.Effects;
using System;

namespace LoopLib
{
    public delegate void GameStartedDelegate();
    public delegate void GameFinishedDelegate(int score, GameFinishedEventArgs.EndReason reason);

    public class Engine
    {
        private SceneInterface _sceneInterface;
        private ContentRepository ContentRepository { get; set; } 

        private
#if WPF
 MonoGameControl.
#else
        Microsoft.Xna.Framework.
#endif
                Game        _game;
        private Tunnel      _tunnel;
        private ElementManager _elemManager;
        private Player      _player;

        private LoopLib.World.Camera _camera;
        private Matrix world;
        private float farClip;

        private InfoPanel   _infoPanel;
        private Screen      _screen;
        private SpriteBatch _spriteBatch;
        private Effect      _effect;
        private Texture2D   _tile;
        private Texture2D   _inactiveBackground;
        private SpriteFont  _font;
        private SpriteFont  _fontIcons;
        private MusicPlayer _music;
        private LevelGenerator _generator;
        private double _elapsedTime;

        public int MaxScore { get; set; }

        private bool _recordAchieved = false;


        public bool IsRunning { get; set; }
        

        public Engine(SceneInterface si, ContentRepository repo,
#if WPF
 MonoGameControl.
#else
        Microsoft.Xna.Framework.
#endif
            Game game,
            Screen screen
            ) {
            _sceneInterface = si;
            _game = game;
            _music = new MusicPlayer();
            
            ContentRepository = repo;

            farClip = 400.0f;
            Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 forward = new Vector3(0.0f, 0.0f, 1.0f);

            world = Matrix.CreateWorld(Vector3.Zero, forward, up);
            Matrix projection = new Matrix();
            Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, game.GraphicsDevice.Viewport.AspectRatio, 0.01f, farClip, out projection);
            _camera = new LoopLib.World.Camera(Matrix.CreateLookAt(Vector3.Zero, forward, up), projection);
            _screen = screen;

            _inactiveBackground = new Texture2D(game.GraphicsDevice, 1, 1);
            _inactiveBackground.SetData<Color>(new Color[] { Color.Black });
            IsRunning = false;

            _generator = new LevelGenerator();


            Globals.Lives = 0;
            Globals.IsFinnished = false;
            //ContentRepository.Game.Components.Add(_explosionParticles);


        }

        
        public void StartGame() {
            
            Globals.TotalScore = 0;
            angleUpdate = 0;
            _recordAchieved = false;
            
            _player.Visible = true;
            _generator.Reset();
            _infoPanel.CurrentLevel =_elemManager.Level = _generator.GenerateNext();
            _infoPanel.CurrentLevel.LevelId = 1;
            Globals.TotalScore = 0;
            _music.Update();
            Globals.Lives = 3;
            Globals.IsFinnished = false;
            IsRunning = true;
            _elapsedTime = 0;
            OnGameStarted();
        }

        public void LoadContent() {

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
            _infoPanel = new InfoPanel(ContentRepository, _screen);

            Console.Out.WriteLine("Engine: Music/the_lift");
            _music.AddSong("the_lift", ContentRepository.LoadSong("Music/the_lift"));
            Console.Out.WriteLine("Engine: Sounds/explode");
            _music.AddSoundEffect("explode",ContentRepository.LoadSoundEffect("Sounds/explode"));
            Console.Out.WriteLine("Engine: Sounds/floop");
            _music.AddSoundEffect("star", ContentRepository.LoadSoundEffect("Sounds/floop"));

            Console.Out.WriteLine("Engine: Textures/tile");
            _tile = ContentRepository.LoadTexture("Textures/tile");
            Console.Out.WriteLine("Engine: Effects/Tunnel");
            _effect = ContentRepository.LoadEffect("Effects/Tunnel"
#if ANDROID
                +".android"
#endif
            );
            
            _tunnel = new Tunnel(32, _effect, _tile);
            _player = new Player(ContentRepository, _game, _camera, _tunnel);
            _player.Visible = false;

            _elemManager = new ElementManager(ContentRepository, _sceneInterface, _game, _camera, _tunnel, _player,_music);
            angleUpdate = 0;

            //_font[0] = ContentRepository.LoadFont("Fonts/Ubuntu12");
            //_font[1] = ContentRepository.LoadFont("Fonts/Ubuntu24");
            //_font[2] = ContentRepository.LoadFont("Fonts/Ubuntu36");
            //_font[3] = ContentRepository.LoadFont("Fonts/Ubuntu48");
            //_font[4] = ContentRepository.LoadFont("Fonts/Ubuntu64");
            Console.Out.WriteLine("Engine: " + LoopGame.MENU_BUTTON_FONT + LoopGame.MENU_BUTTON_FONT_SIZE);
            _font = ContentRepository.LoadFont(LoopGame.MENU_BUTTON_FONT+LoopGame.MENU_BUTTON_FONT_SIZE);
            Console.Out.WriteLine("Engine: " + "Fonts/Awesome" + LoopGame.MENU_BUTTON_FONT_SIZE);
            _fontIcons = ContentRepository.LoadFont("Fonts/Awesome" + LoopGame.MENU_BUTTON_FONT_SIZE);
            //_fontIcons[0] = ContentRepository.LoadFont("Fonts/Awesome12");
            //_fontIcons[1] = ContentRepository.LoadFont("Fonts/Awesome24");
            //_fontIcons[2] = ContentRepository.LoadFont("Fonts/Awesome36");
            //_fontIcons[3] = ContentRepository.LoadFont("Fonts/Awesome48");
            //_fontIcons[4] = ContentRepository.LoadFont("Fonts/Awesome64");

        }

        public void Pause() {
            OpenFeasyo.GameTools.GameTools.IsPaused = true;
        }

        public void Resume()
        {
            OpenFeasyo.GameTools.GameTools.IsPaused = false;
        }

        public void Unload() {
            if (!Globals.IsFinnished) {
                OnGameFinished(Globals.TotalScore, GameFinishedEventArgs.EndReason.InteruptedByUser);
            }
            _elemManager.DestroyAllObjects();
            _elemManager.Unload();
            _music.Destroy();
        }       

        public void Update(GameTime gameTime) {

            
            KeyboardState currentKeyboardState = Keyboard.GetState();
            if (currentKeyboardState.IsKeyDown(Keys.Right) ||
                currentKeyboardState.IsKeyDown(Keys.D))
            {
                angleUpdate -= 1f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left) ||
                currentKeyboardState.IsKeyDown(Keys.A))
            {
                angleUpdate += 1f;
            }
            
            if (Globals.IsFinnished && IsRunning) {
                OnGameFinished(Globals.TotalScore,GameFinishedEventArgs.EndReason.GoalAccomplished);
                if (MaxScore < Globals.TotalScore) {
                    _recordAchieved = true;
                    MaxScore = Globals.TotalScore;
                }
                IsRunning = false;
                _elemManager.Level = null;
                _elemManager.DestroyAllObjects();
            }
        

            angleUpdate = Math.Min(angleUpdate, 1);
            angleUpdate = Math.Max(angleUpdate, -1);

            _elemManager.DrawParticles(gameTime);

            if(!OpenFeasyo.GameTools.GameTools.IsPaused){
                _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (IsRunning && (_elapsedTime / 20)-2 > _elemManager.Level.LevelId &&
                    _elemManager.Level.LevelId < 10)
                {
                    _infoPanel.CurrentLevel = _elemManager.Level = _generator.GenerateNext();
                    
                }
                _tunnel.Update(gameTime, _elemManager.Level == null ? 10 : _elemManager.Level.Speed);
                _player.Update(gameTime, angleUpdate * (float)gameTime.ElapsedGameTime.TotalSeconds * 2, angleUpdate * 0.2f);
                _camera.Update(gameTime, _tunnel.CurrentPhase, /*this.headPos,*/ _player.Angle);
                _elemManager.Update(gameTime);
            }
            angleUpdate *= 0.9f;
            _infoPanel.Update(gameTime);

        

            if (_infoPanel.CurrentLevel != null) { 
                _infoPanel.CurrentLevel.Score = Globals.TotalScore;
            }
        }
        
        public void Draw(GameTime gametime)
        {
            _game.GraphicsDevice.BlendState = BlendState.Opaque;
            _game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            _game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            _tunnel.Draw(_game.GraphicsDevice, world, _camera.View, _camera.Projection, farClip);

            _elemManager.DrawParticles(gametime);
            _game.GraphicsDevice.BlendState = BlendState.Opaque;
            _game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            _game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            
            _spriteBatch.Begin();
            _infoPanel.Draw(gametime,_spriteBatch);
            DrawLives(_spriteBatch);
            //DrawTools(_spriteBatch);
            _spriteBatch.End();
        }

        

        private void DrawTools(SpriteBatch spriteBatch) {
            
            if (!IsRunning) {
                spriteBatch.Draw(_inactiveBackground, _screen.ToScreen(0, 0, 1, 1), Color.FromNonPremultiplied(255, 255, 255, 128));
                float startPos = 0.5f;
                if (Globals.IsFinnished)
                {
                    if (_recordAchieved)
                    {
                        DrawMessage(spriteBatch, 0.2f, "Congratulations!!!");
                        DrawMessage(spriteBatch, 0.3f, "New highest score:" + Globals.TotalScore);
                    }
                    else {
                        DrawMessage(spriteBatch, 0.3f, "Your score is " + Globals.TotalScore);
                    }
                    DrawMessage(spriteBatch,0.5f, "GAME OVER");
                    startPos = 0.7f;
                }
                DrawMessage(spriteBatch, startPos, "Press SPACE to start");
            }
            else if (OpenFeasyo.GameTools.GameTools.IsPaused) {
                spriteBatch.Draw(_inactiveBackground, _screen.ToScreen(0, 0, 1, 1), Color.FromNonPremultiplied(255, 255, 255, 128));
                DrawMessage(spriteBatch, 0.5f, "Paused");
            }
            DrawLives(spriteBatch);
        }

        private void DrawLives(SpriteBatch spriteBatch)
        {
            //SpriteFont f = _fontIcons[_screen.FontSize];
            string lives = "";
            for(int i = 0; i < Globals.Lives; i++)
            {
                lives += "";
            }
            Vector2 size = _fontIcons.MeasureString(lives);
            spriteBatch.DrawString(_fontIcons,lives,new Vector2(_screen.ScreenWidth - size.X - 10,_screen.ScreenHeight - size.Y - 10),Color.Black);
            spriteBatch.DrawString(_fontIcons, lives, new Vector2(_screen.ScreenWidth - size.X - 13, _screen.ScreenHeight - size.Y - 13), Color.Orange);

        }

        private void DrawMessage(SpriteBatch spriteBatch, float verticalPosition, string message) { 
            Vector2 size = _font.MeasureString(message);
            spriteBatch.DrawString(_font, message, new Vector2(_screen.ScreenWidth / 2 - size.X / 2, _screen.ScreenHeight * verticalPosition - size.Y / 2), Color.White);
        }

        public event GameStartedDelegate GameStarted;
        private void OnGameStarted()
        {
            if (GameStarted != null)
            {
                GameStarted();
            }
        }


        public event GameFinishedDelegate GameFinished;

        public void OnGameFinished(int score, GameFinishedEventArgs.EndReason reason)
        {
            if (GameFinished != null)
            {
                GameFinished( score, reason);
            }
        }


        public static void HorizontalMovementHandle(int source, float value)
        {
            if (float.IsNaN(value)) return;

            if (value > 1)
                value = 1;
            if (value < -1)
                value = -1;
            average.AddValue(value);
            angleUpdate = average.GetLastAverage();
        }



        public static void LeftMovementHandle(int source, float value)
        {
            if (float.IsNaN(value)) return;
            average.AddValue(value);
            angleUpdate = average.GetLastAverage();
        }

        public static void RightMovementHandle(int source, float value)
        {
            if (float.IsNaN(value)) return;
            average.AddValue(-value);
            angleUpdate = average.GetLastAverage();
        }

        private static FloatAverageFilter average = new FloatAverageFilter(10);
        private static float angleUpdate;

        private static float leftControlInput = 0;
        private static float rightControlInput = 0;
    }
}
