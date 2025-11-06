using GhostlyLib.Animations;
using GhostlyLib.DynamicDifficulty;
using GhostlyLib.Elements;
using GhostlyLib.Elements.Character;
using GhostlyLib.Elements.Enemies;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace GhostlyLib.Level
{
    public abstract class Level3D : ILevel3D
    {
        #region Private members
        private GameScreen gameScreen;
        #endregion Private members

        #region Public members
        public BasicEffect Exit { get { return ThreeDEffects.Instance.Exit; } }
        public BasicEffect Invisible { get { return ThreeDEffects.Instance.Invisible; } }
        public BasicEffect Crate { get { return ThreeDEffects.Instance.Crate; } }
        public BasicEffect Star { get { return ThreeDEffects.Instance.Star; } }
        public int MaxScore { get; set; }
        public GameScreen GameScreen { get { return gameScreen; } }
        public LevelAnalytics Analytics { get; private set; }
        #endregion Public members

        #region Abstract members
        public abstract Texture2D Background { get; }
        public abstract LevelElements Elements { get; }
        public abstract EnemyAnimation RedEnemyAnimation { get; }
        public abstract IGameCharacter Character { get; set; }
        #endregion Abstract members

        protected Level3D(GameScreen gameScreen)
        {
            this.gameScreen = gameScreen;
            this.Analytics = new LevelAnalytics();
        }
        
        public void LoadMap(String p, double checkpoint)
        {
            String line;
            List<String> lines = new List<String>();
            int width = 0;
            System.IO.StreamReader file;
            if (File.Exists("Levels/" + p))
            {
                file = new System.IO.StreamReader("Levels/" + p);
            }
            else
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Level3D)).Assembly;
                string[] res = assembly.GetManifestResourceNames();
                Stream stream = assembly.GetManifestResourceStream("GhostlyGame.Content.Levels.Ghostly." + p);
                file = new System.IO.StreamReader(stream);
            }

            while ((line = file.ReadLine()) != null)
            {
                if (!line.StartsWith("!"))
                {
                    lines.Add(line);
                    width = Math.Max(width, line.Length);
                }
            }

            for (int j = 0; j < lines.Count; j++)
            {
                line = lines[j];
                for (int i = 0; i < width; i++)
                {
                    if (i < line.Length)
                    {
                        char ch = line[i];

                        Drawable3D d = null;

                        if (ch.Equals('\\'))
                        {
                            d = new Tile3D(i, j, TileType.Crate, this.Elements, this.Crate, this.GameScreen);
                        }
                        else if (ch.Equals('X'))
                        {
                            d = new Tile3D(i, j, TileType.Exit, this.Elements, this.Exit, this.GameScreen);
                        }
                        else if (ch.Equals('c'))    //checkpoint
                        {
                            d = new Tile3D(i, j, TileType.Checkpoint, this.Elements, this.Invisible, this.GameScreen);
                        }
                        else if (ch.Equals('*'))
                        {
                            d = new Star3D(i, j, this.Elements, this.Star, this.GameScreen);
                        }
                        else if (ch.Equals('E'))
                        {
                            //SetEntryPoint(i, j, (MathHelper.Pi * 2) / 4, Direction.East);
                            //SetEntryPoint(i, j, -((MathHelper.Pi * 2) / 2), Direction.East);
                            SetEntryPoint(i, j, ((MathHelper.Pi * 2) / 4) + (MathHelper.Pi * 2) / 2, Direction.East);
                        }
                        else if (ch.Equals('S'))
                        {
                            //SetEntryPoint(i, j, (MathHelper.Pi * 2) / 2, Direction.South);
                            SetEntryPoint(i, j, 0, Direction.South);
                        }
                        else if (ch.Equals('W'))
                        {
                            //SetEntryPoint(i, j, -((MathHelper.Pi * 2) / 2), Direction.West);
                            SetEntryPoint(i, j, (MathHelper.Pi * 2) / 4, Direction.West);
                        }
                        else if (ch.Equals('N'))
                        {
                            //SetEntryPoint(i, j, 0, Direction.North);
                            SetEntryPoint(i, j, (MathHelper.Pi * 2) / 2, Direction.North);
                        }
                        else if (ch.Equals('L'))
                        {
                            //d = new Tile3D(i, j, TileType.LeftRotation, this.Elements, this.Invisible, this.GameScreen);
                            //in 3D game, things are displaye mirrored - left is right
                            d = new Tile3D(i, j, TileType.RightRotation, this.Elements, this.Invisible, this.GameScreen);
                        }
                        else if (ch.Equals('R'))
                        {
                            //d = new Tile3D(i, j, TileType.RightRotation, this.Elements, this.Invisible, this.GameScreen);
                            //in 3D game, things are displaye mirrored - right is left
                            d = new Tile3D(i, j, TileType.LeftRotation, this.Elements, this.Invisible, this.GameScreen);
                        }

                        if (d != null)
                        {
                            this.Elements.AddElement(d);
                        }
                    }
                }
            }

            file.Close();
        }

        private void SetEntryPoint(int i, int j, float rotationAngle, Direction direction)
        {
            ((GameCharacter3D)this.Character).X = i * 40; // i * 0.4f; //40; //40 is the standard tile size
            ((GameCharacter3D)this.Character).Y = j * 40; // j * 0.4f; // 40;
            ((GameCharacter3D)this.Character).OriginalRotation = rotationAngle;
            ((GameCharacter3D)this.Character).Direction = direction;

            ((GameScreen3D)this.GameScreen).LevelEntryPoint = new Vector2(i, j);
            //update maze rotation based on the levelEntryPoint
            //_3DCamera.UpdateView(rotationAngle, 0, ((GameCharacter3D)this.Character).Xi, ((GameCharacter3D)this.Character).Yi);
        }

        public abstract void ProcessPrimaryAction(bool state);
        public abstract void ProcessSecondaryAction(bool state);

        public abstract IEnemy GenerateEnemy(int i, int j);
    }
}