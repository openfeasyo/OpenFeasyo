
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OpenFeasyo.Platform.Controls.Gaming.Task
{
    public abstract class Task : ITask
    {
        public string ConfigurationFile { get; set; }
        public string InputMethod { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

//        protected GameConfiguration _gameConfiguration;
//        protected ContentRepository _repository;
//        protected SceneInterface _sceneInterface;
        protected ContentManager _content;
        protected GraphicsDevice _graphicsDevice;

//        public GameConfiguration GameConfiguration { get { return _gameConfiguration; } set { _gameConfiguration = value; } }

        protected bool _configurationChanged = false;
        public bool ConfigurationChanged { get { return _configurationChanged; } }

        private bool _isLoaded = false;
        public bool IsLoaded { get { return _isLoaded; } }

        public Task()
        {
            ConfigurationFile = "";
        }


        public virtual void Load(ContentManager content,/* ContentRepository repo, SceneInterface scene, */ GraphicsDevice device)
        {  // maybe load the whole scene
//            _repository = repo;
//            _sceneInterface = scene;
            _content = content;
            _graphicsDevice = device;
            _isLoaded = true;
            _configurationChanged = false;
        }

        public virtual void Unload()
        { // maybe unload the whole scene
       //     MusicPlayer.Stop();
            _isLoaded = false;
        }

        private static Dictionary<string, SoundEffect> _soundEffects = new Dictionary<string, SoundEffect>();
        public static Dictionary<string, SoundEffect> SoundEffects
        {
            get
            {
                return _soundEffects;
            }
        }

        //private MusicPlayer _musicPlayer = new MusicPlayer();
        //public MusicPlayer MusicPlayer { get { return _musicPlayer; } }

        public virtual void Update(GameTime gameTime)
        {
     //       _musicPlayer.Update();
        }

        public abstract void Draw(GameTime gameTime);

        public abstract void Draw2D(GameTime gameTime, SpriteBatch spriteBatch);

        public virtual void SaveChangedConfiguration() { }
    }
    
}
