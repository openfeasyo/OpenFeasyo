using GhostlyLib.Animations;
using GhostlyLib.Elements;
using GhostlyLib.Elements.Character;
using GhostlyLib.Level;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenFeasyo.GameTools.Effects;
using System.Diagnostics;

namespace GhostlyLib.Screens
{
    public class GameScreen2D : GameScreen
    {
        public override float SPEED { get { return 3f; } }
        //private double _checkpoint = 0;

        #region Public properties

        public GameScreen2D(int level, MusicPlayer player, OpenFeasyo.GameTools.Screen screen) : base(level, player, screen)
        {
        }

        /*public int Completion
        {
            get
            {
                return (int)(((double)GameCharacter.Score / Level.MaxScore) * 100);
            }
        }*/


        private Vector3 Position { get; set; }

        #endregion Public properties

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
            else if (this.CurrentLevel <= 140)
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

            this.GameCharacter = this.Level.Character;
            this.GameBackground.ContinuousLayer = this.Level.Background;
            GhostlyActionHandlers.CurrentLevel = this.Level;

            this.Position = Vector3.Zero;
            GhostlyGame.Instance.GameObjects.TryUpdate("PlayerPosition", Position);
        }

       /* private void UpdateAllElements(GameTime gameTime)
        {
            this.GameCharacter.Update(gameTime);

            this.Elements.Update(gameTime);

            UpdateOnetimeAnimations();
        }*/

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

        /* protected override void DrawGameplay(SpriteBatch spriteBatch, GameTime gameTime)
         {
             spriteBatch.Draw(this.Level.Background, new Rectangle(0, 0, 2160, 800), Color.White);

             this.Elements.Draw(spriteBatch, gameTime);

             this.GameCharacter.Draw(spriteBatch, gameTime);

             //DrawOnetimeAnimations(spriteBatch);
             spriteBatch.DrawString(this.Font30, "Level: " + this.CurrentLevel.ToString(), new Vector2(10, 10), Color.White);

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

             spriteBatch.DrawString(this.Font30, "Score: " + this.GameCharacter.Score.ToString(), new Vector2(550, 10), Color.White);

             if (this.GameCharacter.TurningDirection == TurningDirection.Left)
             {
                 spriteBatch.Draw(ImagesAndAnimations.Instance.LeftArrow, new Rectangle(10, 220, 85, 72), Color.White);
             }
             else if (this.GameCharacter.TurningDirection == TurningDirection.Right)
             {
                 spriteBatch.Draw(ImagesAndAnimations.Instance.RightArrow, new Rectangle(700, 220, 85, 72), Color.White);
             }
         }*/

        protected override void DrawGameplay(SpriteBatch spriteBatch, GameTime gameTime)
        {
            this.GameBackground.Draw(gameTime, spriteBatch);

            this.Elements.Draw(spriteBatch, gameTime);

            this.GameCharacter.Draw(spriteBatch, gameTime);

            DrawOnetimeAnimations(spriteBatch);

            float position = 15;

            spriteBatch.DrawString(Font[2], "Level: " + this.CurrentLevel.ToString(), new Vector2(position, 20), GhostlyGame.MENU_FONT_COLOR);

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
                if (this.Level is SpaceLevel)
                {
                    ((GameCharacter)this.GameCharacter).MoveLeft();
                }
            }
            else if (state.IsKeyDown(Keys.Right) && this.State.Equals(GameState.Running))
            {
                if (this.Level is SpaceLevel)
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