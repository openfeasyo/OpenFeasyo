using GhostlyGame.Models;
using GhostlyLib.Animations;
using GhostlyLib.DynamicDifficulty;
using GhostlyLib.Elements;
using GhostlyLib.Elements.Character;
using GhostlyLib.Level;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.GameTools.Effects;

namespace GhostlyLib.Screens
{
    public class GameScreen2D : GameScreen
    {
        #region Private members
        private Vector3 Position { get; set; }
        //private double _checkpoint = 0;
        #endregion Private members

        #region Public members

        public override float SPEED
        {
            get
            {
                if (GameSessionInfo.Instance.SelectedPatient != null && GameSessionInfo.Instance.SelectedPatient.DifficultyLevel != null)
                {
                    switch (DifficultyLevelStateSpace.Instance.getLevelDefinition((int)GameSessionInfo.Instance.SelectedPatient.DifficultyLevel).restDuration)
                    {
                        case 10:
                            return 1f;
                        case 6:
                            return 1.5f;
                        default:
                            return 3f;
                    }
                }
                return 3f;
            }
        }


        #endregion Public members

        public GameScreen2D(int level, MusicPlayer player, OpenFeasyo.GameTools.Screen screen) : base(level, player, screen)
        {
        }

        public override void Initialize()
        {
            this.GameBackground = new GameBackground(0, SPEED, this.Screen);
        }

        public override void LoadLevel()
        {
            this.State = GameState.Running;
            //clear elements & start logging
            this.Elements = new LevelElements();
            OnGameStarted(CurrentLevel);

            if (this.CurrentLevel <= 30)
            {
                this.Level = new EarthLevel(this, this.Elements);
                this.Level.LoadMap("earth.map" + this.CurrentLevel + ".txt", this.Checkpoint);
                this.GameBackground.SetParallaxLayers(new List<Texture2D> { ((ILevel2D)Level).BackgroundFurthest, ((ILevel2D)Level).BackgroundFurther, ((ILevel2D)Level).BackgroundFar, ((ILevel2D)Level).BackgroundClose, ((ILevel2D)Level).BackgroundCloser, ((ILevel2D)Level).BackgroundClosest });
            }
            else if (this.CurrentLevel <= 60)
            {
                this.Level = new WaterLevel(this, this.Elements);
                this.Level.LoadMap("water.map" + this.CurrentLevel + ".txt", this.Checkpoint);
                this.GameBackground.SetParallaxLayers(new List<Texture2D> { ((ILevel2D)Level).BackgroundClose, ((ILevel2D)Level).BackgroundCloser, ((ILevel2D)Level).BackgroundClosest });
            }
            else if (this.CurrentLevel <= 90)
            {
                this.Level = new RockLevel(this, this.Elements);
                this.Level.LoadMap("rock.map" + this.CurrentLevel + ".txt", this.Checkpoint);
                this.GameBackground.SetParallaxLayers(new List<Texture2D> { ((ILevel2D)Level).BackgroundFurthest, ((ILevel2D)Level).BackgroundClose, ((ILevel2D)Level).BackgroundClosest });

            }
            else if (this.CurrentLevel <= 120)
            {
                this.Level = new IceLevel(this, this.Elements);
                this.Level.LoadMap("ice.map" + this.CurrentLevel + ".txt", this.Checkpoint);
                this.GameBackground.SetParallaxLayers(new List<Texture2D> { ((ILevel2D)Level).BackgroundFurthest, ((ILevel2D)Level).BackgroundClose, ((ILevel2D)Level).BackgroundClosest });
            }
            else if (this.CurrentLevel <= 140)  //Land Levels - Ruben designed levels
            {
                this.Level = new RockLevel(this, this.Elements);
                this.Level.LoadMap("land.map" + this.CurrentLevel + ".txt", this.Checkpoint);
                this.GameBackground.SetParallaxLayers(new List<Texture2D> { ((ILevel2D)Level).BackgroundFurthest, ((ILevel2D)Level).BackgroundClose, ((ILevel2D)Level).BackgroundClosest });
            }
            else if (this.CurrentLevel <= 160)
            {
                this.Level = new SpaceLevel(this, this.Elements, 300);
                this.Level.LoadMap("space.map" + this.CurrentLevel + ".txt", this.Checkpoint);
                this.GameBackground.SetParallaxLayers(new List<Texture2D> { ((ILevel2D)Level).BackgroundFurthest, ((ILevel2D)Level).BackgroundClose, ((ILevel2D)Level).BackgroundCloser });//, _level.BackgroundClosest });
            }
            else if (this.CurrentLevel > 220 && this.CurrentLevel <= 250)
            {
                this.Level = new SimpleSpaceLevel(this, this.Elements, 100);
                this.Level.LoadMap("simpleSpace.map" + this.CurrentLevel + ".txt", this.Checkpoint);
                this.GameBackground.SetParallaxLayers(new List<Texture2D> { ((ILevel2D)Level).BackgroundFurthest, ((ILevel2D)Level).BackgroundClose, ((ILevel2D)Level).BackgroundCloser });
            }

            this.GameCharacter = this.Level.Character;
            this.GameBackground.ContinuousLayer = this.Level.Background;
            GhostlyActionHandlers.CurrentLevel = this.Level;

            this.Position = Vector3.Zero;
            GhostlyGame.Instance.GameObjects.TryUpdate("PlayerPosition", Position);
        }

        private void UpdateAllElements(GameTime gameTime)
        {
            this.Level.ProcessPrimaryAction(EmgState.Primary);
            this.Level.ProcessSecondaryAction(EmgState.Secondary);

            ((GameCharacter)this.GameCharacter).Update(gameTime);

            this.Elements.Update(gameTime);

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

        public override void Update(GameTime gameTime)
        {
            this.Position = new Vector3((float)(Position.X - GameBackground.HorizontalSpeed), (float)(Screen.Height - ((Drawable)Level.Character).Y), GameBackground.HorizontalSpeed);
            GhostlyGame.Instance.GameObjects.TryUpdate("PlayerPosition", Position + new Vector3((float)(((Drawable)GameCharacter).X + GameCharacter.Width / 2), 0, 0));
            KeyboardUpdate();

            if (State.Equals(GameState.Running))
            {
                UpdateAllElements(gameTime);
            }
        }

        protected override void DrawGameplay(SpriteBatch spriteBatch, GameTime gameTime)
        {
            this.GameBackground.Draw(gameTime, spriteBatch);

            this.Elements.Draw(spriteBatch, gameTime);

            this.GameCharacter.Draw(spriteBatch, gameTime);

            DrawOnetimeAnimations(spriteBatch);

            float position = 15;
            if (!(this.Level is SpaceLevel) || !(this.Level is SimpleSpaceLevel)) // for space levels don't show level info, it covers the space ship
            {
                spriteBatch.DrawString(Font[2], "Level: " + this.CurrentLevel.ToString(), new Vector2(position, 20), GhostlyGame.MENU_FONT_COLOR);
            }
            position = Font[2].MeasureString("Level: " + this.CurrentLevel.ToString()).X + 100;

            switch (this.GameCharacter.CurrentHealth)
            {
                case 3:
                    spriteBatch.Draw(ImagesAndAnimations.Instance.HeartFull, new Rectangle((int)position, 20, 53, 45), Color.White);
                    break;
                case 2:
                    spriteBatch.Draw(ImagesAndAnimations.Instance.HeartHalf, new Rectangle((int)position, 20, 53, 45), Color.White);
                    break;
                case 1:
                    spriteBatch.Draw(ImagesAndAnimations.Instance.HeartEmpty, new Rectangle((int)position, 20, 53, 45), Color.White);
                    break;
                default:
                    spriteBatch.Draw(ImagesAndAnimations.Instance.InvisibleTile, new Rectangle((int)position, 20, 53, 45), Color.White);
                    break;
            }

            position += 153;

            spriteBatch.DrawString(Font[2], "Score: " + GameCharacter.Score.ToString(), new Vector2(position, 20), GhostlyGame.MENU_FONT_COLOR);


            //Draw instruction in the middle of the screen
            switch (this.GameCharacter.Instruction)
            {
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

            if ((this.Level is SimpleSpaceLevel && ((SimpleSpaceCharacter)this.GameCharacter).ActionMovement == ActionMovement.Left && (state.IsKeyUp(Keys.Left)))
                ||
                (this.Level is SimpleSpaceLevel && ((SimpleSpaceCharacter)this.GameCharacter).ActionMovement == ActionMovement.Right && (state.IsKeyUp(Keys.Right))))
            {
                ((SimpleSpaceCharacter)GameCharacter).StopLeftRightMovement();
            }

            if (state.IsKeyDown(Keys.Up) && this.State.Equals(GameState.Running))
            {
                if (this.Level is EarthLevel || this.Level is RockLevel || this.Level is IceLevel)
                {
                    ((GameCharacter)this.GameCharacter).Jump();
                }
                else if (this.Level is WaterLevel)
                {
                    ((GameCharacter)this.GameCharacter).Swimming();
                }
            }
            else if (state.IsKeyDown(Keys.LeftAlt) && this.State.Equals(GameState.Running))
            {
                //this.GameCharacter.Shoot();
                GhostlyActionHandlers.SecondaryActionHandle(0, 1);
            }
            else if (state.IsKeyDown(Keys.Left) && this.State.Equals(GameState.Running))
            {
                if (this.Level is SpaceLevel || this.Level is SimpleSpaceLevel)
                {
                    ((GameCharacter)this.GameCharacter).MoveLeft();
                }
            }
            else if (state.IsKeyDown(Keys.Right) && this.State.Equals(GameState.Running))
            {
                if (this.Level is SpaceLevel || this.Level is SimpleSpaceLevel)
                {
                    ((GameCharacter)this.GameCharacter).MoveRight();
                }
            }
        }

        public override void SetCheckpoint(double checkpoint)
        {
            this.Checkpoint = checkpoint;
            this.State = GameState.Running;
        }
    }
}