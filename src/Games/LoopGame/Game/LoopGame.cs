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
using OpenFeasyo.Platform.Configuration;
using OpenFeasyo.Platform.Controls;
using LoopLib.Activities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenFeasyo.GameTools;
using OpenFeasyo.GameTools.Core;
using OpenFeasyo.GameTools.UI;
using System;
using System.Collections.Concurrent;

namespace LoopLib
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class LoopGame :
#if WPF
        MonoGameControl.
#else
        Microsoft.Xna.Framework.
#endif
        Game, IGame//, SceneInterface
    {
#if !WPF
        static GraphicsDeviceManager graphics;
#endif

        internal static LoopGame Instance { get; set; }

        public static Color MENU_FONT_COLOR = Color.FromNonPremultiplied(102, 102, 102, 256);
        public static string MENU_BUTTON_FONT = "Fonts/PerfectDark";
        public static int MENU_BUTTON_FONT_SIZE = 48;


        /// <summary>
        /// Contains the latest snapshot of the keyboard's input state.
        /// </summary>
        public KeyboardState KeyboardState { get; set; }
        /// <summary>
        /// Contains the latest snapshot of the mouse's input state.
        /// </summary>
        public MouseState MouseState { get; set; }

        private OpenFeasyo.GameTools.Screen viewport;

        private SpriteBatch spriteBatch;


        public ContentDatabase ContentDatabase { get; set; }
        public ContentRepository ContentRepository { get; set; }
        private UIEngine _uiengine;
        

        private KeyTracker _fullScreenKey = new KeyTracker(Keys.F12, (o, i) => { graphics.ToggleFullScreen(); });

            public LoopGame()
            : base()
        {
            Instance = this;
#if !WPF
            graphics = new GraphicsDeviceManager(this);
#endif
            viewport = new OpenFeasyo.GameTools.Screen(
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            
            _gameObjects = new PreDefinedDictionary<Vector3>(new string[] { "PlayerPosition", "Objects"}, Vector3.Zero);
            _gameStream = new PreDefinedDictionary<double>(new string[] { }, 0.0);
            _gameEvents = new ConcurrentQueue<short>();

            MENU_BUTTON_FONT_SIZE = new int[] { 12, 24, 36, 48, 64 }[viewport.FontSize];

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Content.RootDirectory = "Content";
            ContentDatabase = new ContentDatabase();
            ContentRepository = new ContentRepository(this);

            OpenFeasyo.Platform.Configuration.Configuration.ClearBindingPoints();

            //
            // "Horizontal"  - 0
            //

            OpenFeasyo.Platform.Configuration.Configuration.RegisterBindingPoint(Definition.BindingPoints[0], Engine.HorizontalMovementHandle);
            OpenFeasyo.Platform.Configuration.Configuration.RegisterBindingPoint(Definition.BindingPoints[1], Engine.LeftMovementHandle);
            OpenFeasyo.Platform.Configuration.Configuration.RegisterBindingPoint(Definition.BindingPoints[2], Engine.RightMovementHandle);
            // TODO: Add your initialization logic here


            _uiengine = new UIEngine(ContentRepository, GraphicsDevice);
            _uiengine.ActivitiesFinished += _engine_ActivitiesFinished;
            _uiengine.StartActivity(new SplashActivity(_uiengine));

            IsMouseVisible = true;
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
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _uiengine.MusicPlayer.AddSong("menu", ContentRepository.LoadSong("Music/son_of_a_rocket"));
            _uiengine.MusicPlayer.AddSong("win", ContentRepository.LoadSong("Music/take_a_chance"));
            _uiengine.MusicPlayer.AddSong("game", ContentRepository.LoadSong("Music/the_lift"));

        }



        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            OpenFeasyo.Platform.Configuration.Configuration.ClearBindingPoints();
            //MaxScore = _engine.MaxScore;
            _uiengine.Destroy();
            Content.Unload();
        }

        

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                if (graphics.IsFullScreen)
                    graphics.ToggleFullScreen();
                Exit();
            }

            //_screen.ScreenWidth = GraphicsDevice.Viewport.Width;
            //_screen.ScreenHeight = GraphicsDevice.Viewport.Height;
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.
#if WPF                
                MouseState;
#else
                GetState();
#endif
            OpenFeasyo.GameTools.GameTools.Update(gameTime, MouseState, KeyboardState);
            _fullScreenKey.Update(KeyboardState);

            

            _uiengine.Update(gameTime);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            base.Draw(gameTime);

            spriteBatch.Begin();
            _uiengine.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            
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

        private static GameDefinition _definition = new GameDefinition("Loop", new string[] { "Horizontal","Left", "Right" }
            , 10974
            );

        public event EventHandler<GameStartedEventArgs> GameStarted;
        internal void OnGameStarted()
        {
            ConfigurationLoader.LoadConfigurationFromString(Configuration, this);
            Console.WriteLine("Game Started ");
            if (GameStarted != null)
            {
                GameStarted(this, new GameStartedEventArgs(_definition.Name,1));
            }
        }


        public event EventHandler<GameFinishedEventArgs> GameFinished;

        public void OnGameFinished(int score, GameFinishedEventArgs.EndReason reason)
        {
            Console.WriteLine("Game Finished - Score: " + score);
            MaxScore = Math.Max(MaxScore, score);
            if (GameFinished != null)
            {
                GameFinished(this, new GameFinishedEventArgs(_definition.Name, score, 1, reason));
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
