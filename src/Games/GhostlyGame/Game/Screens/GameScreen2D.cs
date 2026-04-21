using GhostlyGame;
using GhostlyGame.Models;
using GhostlyGame.Resources.Localization;
using GhostlyLib.Animations;
using GhostlyLib.DynamicDifficulty;
using GhostlyLib.Elements;
using GhostlyLib.Elements.Character;
using GhostlyLib.Level;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.GameTools.Effects;
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Data;
using System.Globalization;

namespace GhostlyLib.Screens
{
    public class GameScreen2D : GameScreen
    {
        #region Private members
        private Vector3 Position { get; set; }
        //private double _checkpoint = 0;
        private OpenFeasyo.GameTools.Screen _screen;
        #endregion Private members

        #region Public members

        public override float SPEED
        {
            get
            {
                if (this.Level == null)
                    return 3f;

                //the Simple Space Level is the only level where the difficulty levels should apply, and thus influence the game speed
                if (this.Level.GetType().Equals(typeof(SimpleSpaceLevel))
                    && GameSessionInfo.Instance.SelectedPatient != null
                    && GameSessionInfo.Instance.SelectedPatient.CurrentDifficultyLevel != null)
                {
                    switch (DifficultyLevelStateSpace.Instance.getLevelDefinition((int)GameSessionInfo.Instance.SelectedPatient.CurrentDifficultyLevel).restDuration)
                    {
                        case 10:
                            return 0.9f;
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
            if (GameSessionInfo.Instance.SelectedPatient != null && GameSessionInfo.Instance.SelectedPatient.CurrentDifficultyLevel == null)
                GameSessionInfo.Instance.SelectedPatient.CurrentDifficultyLevel = 1;

            _screen = screen;
        }

        public override void Initialize()
        {
            this.GameBackground = new GameBackground(0, SPEED, this.Screen);
        }

        public override void LoadLevel()
        {
            //re-initialize BG
            this.GameBackground = new GameBackground(0, SPEED, this.Screen);

            //update activation threshold for the sensors based on current difficulty settings
            IEmgSensorInput _emgInput = GameSessionInfo.Instance.GetSensorInput();
            _emgInput.ActivationThreshold[0] = GameSessionInfo.Instance.Max1 * ((DifficultyLevelStateSpace.Instance.getLevelDefinition((int)GameSessionInfo.Instance.SelectedPatient.CurrentDifficultyLevel))._MVCLevel);
            _emgInput.ActivationThreshold[1] = GameSessionInfo.Instance.Max2 * ((DifficultyLevelStateSpace.Instance.getLevelDefinition((int)GameSessionInfo.Instance.SelectedPatient.CurrentDifficultyLevel))._MVCLevel);

            this.State = GameState.Running;
            //clear elements & start logging
            this.Elements = new LevelElements();
            OnGameStarted(CurrentLevel);

            UpdateC3DDictionaryValues();

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
                this.Level = new SpaceLevel(this, this.Elements);
                this.Level.LoadMap("space.map" + this.CurrentLevel + ".txt", this.Checkpoint);
                this.GameBackground.SetParallaxLayers(new List<Texture2D> { ((ILevel2D)Level).BackgroundFurthest, ((ILevel2D)Level).BackgroundClose, ((ILevel2D)Level).BackgroundCloser });//, _level.BackgroundClosest });
            }
            else if (this.CurrentLevel > 190 && this.CurrentLevel <= 200)
            {
                this.Level = new SimpleSpaceLevel(this, this.Elements);
                this.Level.LoadMap("simpleSpace.map" + this.CurrentLevel + ".txt", this.Checkpoint);
                this.GameBackground.SetParallaxLayers(new List<Texture2D> { ((ILevel2D)Level).BackgroundFurthest, ((ILevel2D)Level).BackgroundClose, ((ILevel2D)Level).BackgroundCloser });
            }

            this.GameCharacter = this.Level.Character;
            this.GameBackground.ContinuousLayer = this.Level.Background;
            GhostlyActionHandlers.CurrentLevel = this.Level;

            this.Position = Vector3.Zero;
            GhostlyGame.Instance.GameObjects.TryUpdate("PlayerPosition", Position);
        }

        private async void UpdateC3DDictionaryValues()
        {
            //update ghostlyPlus data to be stored in c3d file
            SeriousGames.C3dDataStore.Clear();
            SeriousGames.C3dDataStore = new Dictionary<string, C3DGhostlyPlusData>();

            int difficultyLevel = (int)GameSessionInfo.Instance.SelectedPatient.CurrentDifficultyLevel;
            DifficultyLevelDefinition diff_lev_def = DifficultyLevelStateSpace.Instance.getLevelDefinition(difficultyLevel);

            SeriousGames.C3dDataStore.Add("INFO:DIFFICULTY_LEVEL", new C3DGhostlyPlusData(typeof(Int16), (int)difficultyLevel));
            SeriousGames.C3dDataStore.Add("INFO:DIFFICULTY_LEVEL_MVC", new C3DGhostlyPlusData(typeof(float), diff_lev_def._MVCLevel * 100));
            SeriousGames.C3dDataStore.Add("INFO:DIFFICULTY_LEVEL_CONTRACTION_DURATION", new C3DGhostlyPlusData(typeof(Int16), diff_lev_def.contractionDuration * 1000));    //server expects milliseconds
            SeriousGames.C3dDataStore.Add("INFO:DIFFICULTY_LEVEL_REST_DURATION", new C3DGhostlyPlusData(typeof(Int16), diff_lev_def.restDuration * 1000));                  //server expects milliseconds


            var max_ch1 = ExtractScientificParts(GameSessionInfo.Instance.Max1);
            SeriousGames.C3dDataStore.Add("INFO:MAX_MVC_CH1_SIGNIFICAND", new C3DGhostlyPlusData(typeof(float), max_ch1.significand));
            SeriousGames.C3dDataStore.Add("INFO:MAX_MVC_CH1_EXPONENT", new C3DGhostlyPlusData(typeof(float), max_ch1.exponent));

            var max_ch2 = ExtractScientificParts(GameSessionInfo.Instance.Max2);
            SeriousGames.C3dDataStore.Add("INFO:MAX_MVC_CH2_SIGNIFICAND", new C3DGhostlyPlusData(typeof(float), max_ch2.significand));
            SeriousGames.C3dDataStore.Add("INFO:MAX_MVC_CH2_EXPONENT", new C3DGhostlyPlusData(typeof(float), max_ch2.exponent));


            //save which sensor is on which muscle, e.g. B345 = left, 9N34 = right
            SeriousGames.C3dDataStore.Add("INFO:LEFT_SENSOR", new C3DGhostlyPlusData(typeof(string), GameSessionInfo.Instance.LeftSensor));
            SeriousGames.C3dDataStore.Add("INFO:RIGHT_SENSOR", new C3DGhostlyPlusData(typeof(string), GameSessionInfo.Instance.RightSensor));


            IEmgSensorInput _emgInput = GameSessionInfo.Instance.GetSensorInput();
            var res_ch1 = ExtractScientificParts(_emgInput.ActivationThreshold[0]);
            SeriousGames.C3dDataStore.Add("INFO:ACTIVATION_THRESHOLD_CH1_SIGNIFICAND", new C3DGhostlyPlusData(typeof(float), res_ch1.significand));
            SeriousGames.C3dDataStore.Add("INFO:ACTIVATION_THRESHOLD_CH1_EXPONENT", new C3DGhostlyPlusData(typeof(float), res_ch1.exponent));

            var res_ch2 = ExtractScientificParts(_emgInput.ActivationThreshold[1]);
            SeriousGames.C3dDataStore.Add("INFO:ACTIVATION_THRESHOLD_CH2_SIGNIFICAND", new C3DGhostlyPlusData(typeof(float), res_ch2.significand));
            SeriousGames.C3dDataStore.Add("INFO:ACTIVATION_THRESHOLD_CH2_EXPONENT", new C3DGhostlyPlusData(typeof(float), res_ch2.exponent));

            string therapist = await SecureStorage.Default.GetAsync("username");
            SeriousGames.C3dDataStore.Add("INFO:THERAPIST_ID", new C3DGhostlyPlusData(typeof(string), therapist));

            CultureInfo ci = AppResources.Culture;
            SeriousGames.C3dDataStore.Add("INFO:LANGUAGE", new C3DGhostlyPlusData(typeof(string), ci.Name));
        }

        private void UpdateAllElements(GameTime gameTime)
        {
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
            //KeyboardUpdate();

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
            if (this.Level.GetType() == typeof(SimpleSpaceLevel))
            {
                DrawTextsForSimpleSpaceLevel(spriteBatch, gameTime);
            }
            else
            {
                spriteBatch.DrawString(Font[1], LocalizationResourceManager.Instance["Level"].ToString() + ": " + this.CurrentLevel.ToString() + "  /  " + GameSessionInfo.Instance.SelectedPatient.CurrentDifficultyLevel.ToString(), new Vector2(position, 20), Color.FromNonPremultiplied(11, 206, 196, 256)); // GhostlyGame.MENU_FONT_COLOR);
                position = _screen.ScreenMiddle.X - 60;

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

                position += 300;
                spriteBatch.DrawString(Font[2], LocalizationResourceManager.Instance["Score"].ToString() + ": " + GameCharacter.Score.ToString(), new Vector2(position, 20), GhostlyGame.MENU_FONT_COLOR);
            }
        }

        private void DrawTextsForSimpleSpaceLevel(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 textOrigin = new Vector2(0, 0);
            float position = 200;
            float textRotation = MathHelper.ToRadians(90);

            textOrigin = this.Font[1].MeasureString(LocalizationResourceManager.Instance["Level"].ToString() + ": " + this.CurrentLevel.ToString() + "  /  " + GameSessionInfo.Instance.SelectedPatient.CurrentDifficultyLevel.ToString()) / 2;
            spriteBatch.DrawString(Font[1], LocalizationResourceManager.Instance["Level"].ToString() + ": " + this.CurrentLevel.ToString() + "  /  " + GameSessionInfo.Instance.SelectedPatient.CurrentDifficultyLevel.ToString(), new Vector2(40, 60), Color.FromNonPremultiplied(11, 206, 196, 256), textRotation, textOrigin, 1.0f, SpriteEffects.None, 0.5f); // GhostlyGame.MENU_FONT_COLOR);

            position += 300;
            textOrigin = this.Font[1].MeasureString(LocalizationResourceManager.Instance["Score"].ToString() + ": " + GameCharacter.Score.ToString()) / 2;
            spriteBatch.DrawString(Font[1], LocalizationResourceManager.Instance["Score"].ToString() + ": " + GameCharacter.Score.ToString(), new Vector2(40, _screen.ScreenHeight - 200), Color.FromNonPremultiplied(11, 206, 196, 256), textRotation, textOrigin, 1.0f, SpriteEffects.None, 0.5f);

            //Draw instruction in the middle of the screen
            switch (this.GameCharacter.Instruction)
            {
                case Instruction.Contract:
                    textOrigin = this.Font[3].MeasureString(LocalizationResourceManager.Instance["Contract"].ToString()) / 2;
                    spriteBatch.DrawString(this.Font[3], LocalizationResourceManager.Instance["Contract"].ToString(), new Vector2(_screen.ScreenMiddle.X, _screen.ScreenMiddle.Y), Color.FromNonPremultiplied(255, 255, 255, 200), textRotation, textOrigin, 2.0f, SpriteEffects.None, 0.5f); //Color.Red
                    break;
                case Instruction.Hold:
                    textOrigin = this.Font[3].MeasureString(LocalizationResourceManager.Instance["Hold"].ToString()) / 2;
                    spriteBatch.DrawString(this.Font[3], LocalizationResourceManager.Instance["Hold"].ToString(), new Vector2(_screen.ScreenMiddle.X, _screen.ScreenMiddle.Y), Color.FromNonPremultiplied(255, 255, 255, 200), textRotation, textOrigin, 2.0f, SpriteEffects.None, 0.5f); //Color.Orange
                    break;
                case Instruction.Release:
                    textOrigin = this.Font[3].MeasureString(LocalizationResourceManager.Instance["Release"].ToString()) / 2;
                    spriteBatch.DrawString(this.Font[3], LocalizationResourceManager.Instance["Release"].ToString(), new Vector2(_screen.ScreenMiddle.X, _screen.ScreenMiddle.Y), Color.FromNonPremultiplied(255, 255, 255, 200), textRotation, textOrigin, 2.0f, SpriteEffects.None, 0.5f); //Color.Green
                    break;
                default:
                    break;
            }

            //print minutes:seconds since the start of the app
            textOrigin = this.Font[1].MeasureString(gameTime.TotalGameTime.Minutes.ToString("D2") + ":" + gameTime.TotalGameTime.Seconds.ToString("D2")) / 2;
            spriteBatch.DrawString(this.Font[1], gameTime.TotalGameTime.Minutes.ToString("D2") + ":" + gameTime.TotalGameTime.Seconds.ToString("D2"), new Vector2(40, _screen.ScreenHeight / 2), Color.FromNonPremultiplied(11, 206, 196, 256), textRotation, textOrigin, 1.0f, SpriteEffects.None, 0.5f);
        }

        private void DrawOnetimeAnimations(SpriteBatch spriteBatch)
        {
            foreach (OnetimeAnimation anim in OnetimeAnimations)
            {
                spriteBatch.Draw(anim.Image, Screen.ToScreen(anim.X, anim.Y, anim.Width, anim.Height), Color.White);
            }
        }

        /*private void KeyboardUpdate()
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
        }*/

        public override void SetCheckpoint(double checkpoint)
        {
            this.Checkpoint = checkpoint;
            this.State = GameState.Running;
        }

        public static (float significand, float exponent) ExtractScientificParts(float value)
        {
            if (value == 0f)
                return (0f, 0f);

            int exponent = 0;
            float absValue = Math.Abs(value);

            // Normalize to scientific notation form (1 <= absValue < 10)
            while (absValue < 1f)
            {
                absValue *= 10f;
                exponent--;
            }

            while (absValue >= 10f)
            {
                absValue /= 10f;
                exponent++;
            }

            // Restore original sign
            if (value < 0)
                absValue *= -1f;

            return (absValue, (float)exponent);
        }
    }
}