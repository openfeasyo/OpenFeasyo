using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyGame.Animations
{
    public class ThreeDEffects
    {
        #region Private member
        private static ThreeDEffects _instance;
        #endregion Private member

        #region Public member
        public static ThreeDEffects Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ThreeDEffects();
                }
                return _instance;
            }
        }

        public BasicEffect Exit { get; private set; }
        public BasicEffect Invisible { get; private set; }
        public BasicEffect Crate { get; private set; }
        public BasicEffect Star { get; private set; }

        public BasicEffect Character { get; private set; }
        public BasicEffect CharacterLeftRotating { get; private set; }
        public BasicEffect CharacterRightRotating { get; private set; }

        public BasicEffect Enemy { get; private set; }
        #endregion Public member

        private ThreeDEffects() { }

        public void LoadEffects(ContentManager content, GraphicsDevice graphicsDevice)
        {
            // Set up BasicEffect
            this.Exit = new BasicEffect(graphicsDevice) { TextureEnabled = true, Texture = content.Load<Texture2D>("Textures\\Ghostly\\signExit") };
            this.Invisible = new BasicEffect(graphicsDevice) { TextureEnabled = true, Texture = content.Load<Texture2D>("Textures\\Ghostly\\invisible_tile") };
            this.Crate = new BasicEffect(graphicsDevice) { TextureEnabled = true, Texture = content.Load<Texture2D>("Textures\\Ghostly\\crate") };
            this.Star = new BasicEffect(graphicsDevice) { TextureEnabled = true, Texture = content.Load<Texture2D>("Textures\\Ghostly\\star") };

            this.Character = new BasicEffect(graphicsDevice) { TextureEnabled = true, Texture = content.Load<Texture2D>("Textures\\Ghostly\\maze character\\rocketWithCharacter") };
            this.CharacterLeftRotating = new BasicEffect(graphicsDevice) { TextureEnabled = true, Texture = content.Load<Texture2D>("Textures\\Ghostly\\maze character\\rocketLeftRotating") };
            this.CharacterRightRotating = new BasicEffect(graphicsDevice) { TextureEnabled = true, Texture = content.Load<Texture2D>("Textures\\Ghostly\\maze character\\rocketRightRotating") };
            this.Enemy = new BasicEffect(graphicsDevice) { TextureEnabled = true, Texture = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\red_ufo") };

        }
    }
}