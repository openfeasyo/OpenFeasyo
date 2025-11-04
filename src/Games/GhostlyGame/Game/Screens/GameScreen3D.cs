using GhostlyLib.Animations;
using GhostlyLib.Elements;
using GhostlyLib.Elements.Character;
using GhostlyLib.Level;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.GameTools.Effects;

namespace GhostlyLib.Screens
{
    public class GameScreen3D : GameScreen
    {
        public override float SPEED { get { return 0.75f; } }

        private int EnemiesCount = 0;
        private int LevelTimeMilliseconds = 0;

        #region Public properties
        public GraphicsDevice GraphicsDevice { get; private set; }

       /* public int Completion
        {
            get
            {
                return (int)(((double)GameCharacter.Score / this.Level.MaxScore) * 100);
            }
        }*/

        public Vector2 LevelEntryPoint;

        #endregion Public properties

        public GameScreen3D(int level, MusicPlayer player, OpenFeasyo.GameTools.Screen screen, GraphicsDevice graphicsDevice) : base(level, player, screen)
        {
            GraphicsDevice = graphicsDevice;
        }

        public override void Initialize()
        {
            //TODO MAXIMIZE window
        }

        #region Loaders

        public override void LoadLevel()
        {
            this.State = GameState.Running;
            
            this.Elements = new LevelElements();
            this.EnemiesCount = 0;
            this.LevelTimeMilliseconds = 0;
            OnGameStarted(CurrentLevel);

            if (CurrentLevel > 160 && CurrentLevel <= 190)
            {
                this.Level = new MazeLevel3D(this, this.Elements);
                this.Level.LoadMap("maze.map" + this.CurrentLevel + ".txt", 0);
                this.GameCharacter = ((MazeLevel3D)this.Level).Character;
            }

            GhostlyActionHandlers.CurrentLevel = this.Level;
        }
        #endregion Loaders

        private void UpdateAllElements(GameTime gameTime)
        {
            //TODO should this be here???
            this.Level.ProcessPrimaryAction(EmgState.Primary);
            this.Level.ProcessSecondaryAction(EmgState.Secondary);


            this.GameCharacter.Update(gameTime);

            this.Elements.Update(gameTime);

            UpdateOnetimeAnimations();
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

        public override void Update(GameTime gameTime)
        {
            KeyboardUpdate();

            if (this.State.Equals(GameState.Running))
            {
                UpdateAllElements(gameTime);

                this.LevelTimeMilliseconds += gameTime.ElapsedGameTime.Milliseconds;

                //every 30 seconds generate an enemy, max 2 enemies
                if (((int)this.LevelTimeMilliseconds / 30000) > this.EnemiesCount && this.EnemiesCount < 2)
                {
                    this.EnemiesCount += 1;
                    this.Elements.AddElement(((ILevel3D)this.Level).GenerateEnemy((int)this.LevelEntryPoint.X, (int)this.LevelEntryPoint.Y));
                }
            }
        }

        protected override void DrawGameplay(SpriteBatch spriteBatch, GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            this.Elements.Draw(spriteBatch, gameTime);

            this.GameCharacter.Draw(spriteBatch, gameTime);

            spriteBatch.DrawString(this.Font[2], "Level: " + this.CurrentLevel.ToString(), new Vector2(10, 10), Color.White);

            switch (this.GameCharacter.CurrentHealth)
            {
                case 3:
                    spriteBatch.Draw(ImagesAndAnimations.Instance.HeartFull, new Rectangle(300, 10, 53, 45), Color.White);
                    break;
                case 2:
                    spriteBatch.Draw(ImagesAndAnimations.Instance.HeartHalf, new Rectangle(300, 10, 53, 45), Color.White);
                    break;
                case 1:
                    spriteBatch.Draw(ImagesAndAnimations.Instance.HeartEmpty, new Rectangle(300, 10, 53, 45), Color.White);
                    break;
                default:
                    break;
            }

            spriteBatch.DrawString(this.Font[2], "Score: " + this.GameCharacter.Score.ToString(), new Vector2(550, 10), Color.White);

            if (this.GameCharacter.TurningDirection == TurningDirection.Left)
            {
                spriteBatch.Draw(ImagesAndAnimations.Instance.LeftArrow, new Rectangle(10, 220, 85, 72), Color.White);
            }
            else if (this.GameCharacter.TurningDirection == TurningDirection.Right)
            {
                spriteBatch.Draw(ImagesAndAnimations.Instance.RightArrow, new Rectangle(700, 220, 85, 72), Color.White);
            }

            //Draw instruction in the middle of the screen
            switch (this.GameCharacter.Instruction) {
                case Instruction.Contract:
                    spriteBatch.DrawString(this.Font[2], "Contract", new Vector2(320, 100), Color.Red);
                    break;
                case Instruction.Hold:
                    spriteBatch.DrawString(this.Font[2], "Hold", new Vector2(320, 100), Color.Orange);
                    break;
                case Instruction.Release:
                    spriteBatch.DrawString(this.Font[2], "Release", new Vector2(320, 100), Color.Green);
                    break;
                default:
                    break;
            }
 
            //print minutes:seconds since the start of the app
            //TODO: consider what time needs to be shown and how to calculate it
            spriteBatch.DrawString(this.Font[1], gameTime.TotalGameTime.Minutes.ToString("D2") + ":" + gameTime.TotalGameTime.Seconds.ToString("D2"), new Vector2(300, 430), Color.FromNonPremultiplied(11, 206, 196, 256));
        }

        private void KeyboardUpdate()
        {
            KeyboardState state = Keyboard.GetState();

            if (this.GameCharacter.RotationStatus == RotationStatus.Rotating && (state.IsKeyUp(Keys.Left) && state.IsKeyUp(Keys.Right)))
            {
                ((GameCharacter3D)GameCharacter).TurningInterupted();
            }
            else if (state.IsKeyDown(Keys.Left) && this.State.Equals(GameState.Running))
            {
                ((GameCharacter3D)GameCharacter).TurnCounterClockwise();
            }
            else if (state.IsKeyDown(Keys.Right) && this.State.Equals(GameState.Running))
            {
                ((GameCharacter3D)GameCharacter).TurnClockwise();
            }
            else if (state.IsKeyDown(Keys.Space))
            {
                if (this.State.Equals(GameState.LevelDone))
                {
                    this.CurrentLevel += 1;
                    LoadLevel();
                    this.State = GameState.Running;
                }
                else if (this.State.Equals(GameState.GameOver))
                {
                    LoadLevel();
                    this.State = GameState.Running;
                }
                else
                {
                    PauseGame();
                }
            }
            else if (state.IsKeyDown(Keys.Escape))
            {
                if (this.State.Equals(GameState.Running))
                {
                    this.State = GameState.Paused;
                }
            }
        }

        public override void SetCheckpoint(double checkpoint)
        {
            //not applicable in Maze game
        }
    }
}