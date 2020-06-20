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
using OpenFeasyo.Platform.Configuration;
using OpenFeasyo.Platform.Controls;
using GhostlyLib.Activities;
using GhostlyLib.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenFeasyo.GameTools.Core;
using OpenFeasyo.GameTools.UI;
using System;
using System.Collections.Concurrent;
using OpenFeasyo.Platform.Controls.Drivers;

namespace GhostlyLib
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GhostlyGame :
#if WPF
        MonoGameControl.
#else
        Microsoft.Xna.Framework.
#endif
        Game, IGame
    {

        public static Color MENU_FONT_COLOR = Color.FromNonPremultiplied(102, 102, 102, 256);
        public static string MENU_BUTTON_FONT = "Fonts/Vitamin";
        public static int MENU_BUTTON_FONT_SIZE = 48;

        internal static GhostlyGame Instance { get; set; }


        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //private Screens.Screen screen;
        private UIEngine _engine;
        private ContentRepository _contentRepository;

        private OpenFeasyo.GameTools.Screen viewport;

        

        /// <summary>
        /// Contains the latest snapshot of the keyboard's input state.
        /// </summary>
        public KeyboardState KeyboardState { get; set; }
        /// <summary>
        /// Contains the latest snapshot of the mouse's input state.
        /// </summary>
        public MouseState MouseState { get; set; }

        public GhostlyGame()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferredBackBufferWidth = 1900;
            //graphics.PreferredBackBufferHeight = 1000;

            Content.RootDirectory = "Content";
            _contentRepository = new ContentRepository(this);

            viewport = new OpenFeasyo.GameTools.Screen( 
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            _gameEvents = new ConcurrentQueue<short>();
            _gameObjects = new PreDefinedDictionary<Vector3>(new string[] { "PlayerPosition" },Vector3.Zero);
            _gameStream = new PreDefinedDictionary<double>(new string[] { }, 0);

            MENU_BUTTON_FONT_SIZE = new int [] { 12, 24, 36, 48, 64 }[viewport.FontSize];
            //screen.GameStarted += Screen_GameStarted;
            //screen.GameFinished += Screen_GameFinished;
            //pauseScreenKey = new KeyTracker(Keys.Space, (o, i) => { screen.Pause(); });
        }
        
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            OpenFeasyo.Platform.Configuration.Configuration.ClearBindingPoints();

            OpenFeasyo.Platform.Configuration.Configuration.RegisterBindingPoint(Definition.BindingPoints[0], GhostlyActionHandlers.PrimaryActionHandle);
            OpenFeasyo.Platform.Configuration.Configuration.RegisterBindingPoint(Definition.BindingPoints[1], GhostlyActionHandlers.SecondaryActionHandle);

            _engine = new UIEngine(_contentRepository, GraphicsDevice);
            _engine.ActivitiesFinished += _engine_ActivitiesFinished;
            _engine.StartActivity(new SplashActivity(_engine));
            
            
            base.Initialize();
        }

        private void _engine_ActivitiesFinished(object sender, EventArgs e)
        {
            UnloadContent();
            this.Exit();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Context.Instance.LoadAppSettings();
            ImagesAndAnimations.Instance.LoadImages(this.Content);

            _engine.MusicPlayer.AddSoundEffect("coin", _contentRepository.LoadSoundEffect("Sounds/coin"));
            _engine.MusicPlayer.AddSoundEffect("jump", _contentRepository.LoadSoundEffect("Sounds/jump"));
            _engine.MusicPlayer.AddSoundEffect("ouch", _contentRepository.LoadSoundEffect("Sounds/ouch"));
            _engine.MusicPlayer.AddSoundEffect("drown", _contentRepository.LoadSoundEffect("Sounds/drown"));
            _engine.MusicPlayer.AddSoundEffect("death", _contentRepository.LoadSoundEffect("Sounds/death"));
            _engine.MusicPlayer.AddSoundEffect("secret", _contentRepository.LoadSoundEffect("Sounds/secret"));
            _engine.MusicPlayer.AddSoundEffect("shoot", _contentRepository.LoadSoundEffect("Sounds/shoot"));
            _engine.MusicPlayer.AddSoundEffect("hit", _contentRepository.LoadSoundEffect("Sounds/hit"));
            _engine.MusicPlayer.AddSoundEffect("win", _contentRepository.LoadSoundEffect("Sounds/win"));
            _engine.MusicPlayer.AddSoundEffect("kill", _contentRepository.LoadSoundEffect("Sounds/kill"));
            _engine.MusicPlayer.AddSong("menu", _contentRepository.LoadSong("Music/arcadia"));
            _engine.MusicPlayer.AddSong("game", _contentRepository.LoadSong("Music/scheming-weasel"));
            _engine.MusicPlayer.AddSong("game_rock", _contentRepository.LoadSong("Music/wholesome"));




            //screen.LoadContent(this.Content);

            // TODO: use this.Content to load your game content here
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            //Context.Instance.SaveAppSettings();
            // TODO: Unload any non ContentManager content here
            //screen.UnloadContent();
#if ANDROID
            InputDeviceManager.UnloadAll();
#endif
            _engine.Destroy();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            FrameworkDispatcher.Update();
            
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();


            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.
#if WPF
                MouseState;
#else
                GetState();
#endif
            OpenFeasyo.GameTools.GameTools.Update(gameTime, MouseState, KeyboardState);
            //pauseScreenKey.Update(KeyboardState);

            //screen.Update(gameTime);
            _engine.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.viewport.ScreenWidth = GraphicsDevice.Viewport.Width;
            this.viewport.ScreenHeight = GraphicsDevice.Viewport.Height;

            GraphicsDevice.Clear(Color.FromNonPremultiplied(208,244,247,256) /*Color.White*/);

            spriteBatch.Begin();
            //screen.Draw(spriteBatch, gameTime);
            _engine.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region IGame

        private PreDefinedDictionary<Vector3> _gameObjects;
        private PreDefinedDictionary<double> _gameStream;
        private ConcurrentQueue<Int16> _gameEvents;

        public void OnReport(OpenFeasyo.Platform.Controls.Reports.IReport report)
        {
            throw new NotImplementedException();
        }

        public PreDefinedDictionary<Microsoft.Xna.Framework.Vector3> GameObjects
        {
            get { return _gameObjects; }
        }

        public PreDefinedDictionary<double> GameStream
        {
            get { return _gameStream; }
        }

        public System.Collections.Concurrent.ConcurrentQueue<short> GameEvents
        {
            get { return _gameEvents; }
        }

        public GameDefinition Definition
        {
            get { return _definition; }
        }

        private static GameDefinition _definition = new GameDefinition("Ghostly", new string[] { "Jump/Swim", "Shoot" }, 43322);

        public event EventHandler<GameStartedEventArgs> GameStarted;
        internal void OnGameStarted(GameStartedEventArgs args)
        {
            ConfigurationLoader.LoadConfigurationFromString(Configuration, this);
            Console.WriteLine("Game Started ");
            if (GameStarted != null)
            {
                GameStarted(this, args);
            }
        }

        public event EventHandler<GameFinishedEventArgs> GameFinished;

        internal void OnGameFinished(int score, int level, GameFinishedEventArgs.EndReason reason)
        {
            Console.WriteLine("Game Finished - Score: " + score);
            if (GameFinished != null)
            {
                GameFinished(this, new GameFinishedEventArgs(_definition.Name, score, level, reason));
            }
            string conf = ConfigurationLoader.GetConfigurationXml(
                OpenFeasyo.Platform.Configuration.Configuration.CurrentConfigutration);
            Configuration = conf;
            OpenFeasyo.Platform.Configuration.Configuration.CurrentConfigutration.Destroy();
            OpenFeasyo.Platform.Configuration.Configuration.CurrentConfigutration.RemoveAllBindings();
        }

        public int MaxScore { get; set; }

        public string Configuration { get; set; }

        #endregion IGame
    }
}
