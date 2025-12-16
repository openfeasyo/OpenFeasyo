using GhostlyLib.Animations;
using GhostlyLib.DynamicDifficulty;
using GhostlyLib.Elements;
using GhostlyLib.Elements.Character;
using GhostlyLib.Elements.Enemies;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;

namespace GhostlyLib.Level
{
    public class SpaceLevel : Level
    {
        #region Private members
        //private System.Timers.Timer _timer;
        private LevelElements _elements;
        #endregion Private members

        #region Public members
        public override Texture2D Background { get { return ImagesAndAnimations.Instance.BackgroundSpace; } }

        public override Texture2D BackgroundClosest { get { return ImagesAndAnimations.Instance.BackgroundClosestSpace; } }

        public override Texture2D BackgroundCloser { get { return ImagesAndAnimations.Instance.BackgroundCloserSpace; } }

        public override Texture2D BackgroundClose { get { return ImagesAndAnimations.Instance.BackgroundCloseSpace; } }

        public override Texture2D BackgroundFar { get { return null; } }

        public override Texture2D BackgroundFurther { get { return null; } }

        public override Texture2D BackgroundFurthest { get { return ImagesAndAnimations.Instance.BackgroundFurthestSpace; } }

        public override EnemyAnimation BlackEnemyAnimation { get { return null; } }

        public override Texture2D CliffLeft { get { return null; } }

        public override Texture2D CliffRight { get { return null; } }

        public override LevelElements Elements { get { return this._elements; } }

        public override Texture2D Dirt { get { return null; } }

        public override Texture2D Foreground { get { return null; } }

        public override EnemyAnimation GreenEnemyAnimation { get { return null; } }

        public override Texture2D Ground { get { return null; } }

        public override Texture2D LargeHill { get { return null; } }

        public override EnemyAnimation RedEnemyAnimation { get { return null; } }

        public override Texture2D SmallHill { get { return null; } }

        public override EnemyAnimation YellowEnemyAnimation { get { return null; } }

        public override IGameCharacter Character { get; set; }

        public override Texture2D ExitSign { get { return ImagesAndAnimations.Instance.ExitLine; } }

        public Texture2D BluePlanet { get { return ImagesAndAnimations.Instance.BluePlanet; } }
        public Texture2D YellowPlanet { get { return ImagesAndAnimations.Instance.YellowPlanet; } }
        public Texture2D OrangePlanet { get { return ImagesAndAnimations.Instance.OrangePlanet; } }
        public Texture2D PinkPlanet { get { return ImagesAndAnimations.Instance.PinkPlanet; } }
        public Texture2D RedPlanet { get { return ImagesAndAnimations.Instance.RedPlanet; } }
        /*public Texture2D Star { get { return ImagesAndAnimations.Instance.Star; } }*/
        public Texture2D Ufo { get { return ImagesAndAnimations.Instance.Ufo; } }
        public Texture2D Debris { get { return ImagesAndAnimations.Instance.Debris; } }
        public Texture2D DebrisLong { get { return ImagesAndAnimations.Instance.DebrisLong; } }
        public Texture2D PurpleDebris { get { return ImagesAndAnimations.Instance.PurpleDebris; } }
        public Texture2D PurpleDebrisLong { get { return ImagesAndAnimations.Instance.PurpleDebrisLong; } }
        public Texture2D GreenDebris { get { return ImagesAndAnimations.Instance.GreenDebris; } }
        public Texture2D RedDebrisLong { get { return ImagesAndAnimations.Instance.RedDebrisLong; } }
        public Texture2D DebrisRocks { get { return ImagesAndAnimations.Instance.DebrisRocks; } }
        
        public Texture2D SpaceSpiral { get { return ImagesAndAnimations.Instance.SpaceSpiral; } }
        public Texture2D SpaceMist { get { return ImagesAndAnimations.Instance.SpaceMist; } }

        public override LevelAnalytics Analytics { get; protected set; }

        #endregion Public members

        public SpaceLevel(GameScreen gameScreen, LevelElements elements) : base(gameScreen)
        {
            this._elements = elements;
            this.Character = new SpaceCharacter(gameScreen, elements);
        }

        public override void LoadMap(String p, double checkpoint)
        {
            String line;
            List<String> lines = new List<String>();
            int width = 0;
            System.IO.StreamReader file = loadFile(p);

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

                        Drawable d = null;

                        if (ch.Equals('|'))
                        {
                            d = new Tile(i, j, TileType.InvisibleTile, this.Elements, this.Invisible, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('e'))
                        {
                            d = new Tile(i, j, TileType.Exit, this.Elements, this.ExitArea, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('@'))
                        {
                            d = new Tile(i, j, TileType.ExitSign, this.Elements, this.ExitSign, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('c'))    //checkpoint
                        {
                            d = new Tile(i, j, TileType.Checkpoint, this.Elements, this.Invisible, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('b'))
                        {
                            d = new Tile(i, j, 80, 80, TileType.Planet, this.Elements, this.BluePlanet, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('o'))
                        {
                            d = new Tile(i, j, 80, 62, TileType.Planet, this.Elements, this.OrangePlanet, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('p'))
                        {
                            d = new Tile(i, j, 80, 62, TileType.Planet, this.Elements, this.PinkPlanet, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('g'))
                        {
                            d = new Tile(i, j, 80, 62, TileType.Planet, this.Elements, this.RedPlanet, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('y'))
                        {
                            d = new Tile(i, j, 80, 80, TileType.Planet, this.Elements, this.YellowPlanet, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('*'))
                        {
                            d = new Star(i, j, this.Elements, this.Star, this.GameScreen, checkpoint);
                        }
                        /*else if (ch.Equals('u'))
                        {
                            d = new Tile(i, j, 80, 80, TileType.Ufo, this.Elements, this.Ufo, this.GameScreen, checkpoint);
                        }*/
                        else if (ch.Equals('d'))
                        {
                            d = new Tile(i, j, 160, 320, TileType.Debris, this.Elements, this.Debris, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('D'))
                        {
                            d = new Tile(i, j, 160, 480, TileType.DebrisLong, this.Elements, this.DebrisLong, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('l'))
                        {
                            d = new Tile(i, j, 160, 480, TileType.PurpleDebris, this.Elements, this.PurpleDebris, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('L'))
                        {
                            d = new Tile(i, j, 160, 560, TileType.PurpleDebrisLong, this.Elements, this.PurpleDebrisLong, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('v'))
                        {
                            d = new Tile(i, j, 160, 480, TileType.GreenDebris, this.Elements, this.GreenDebris, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('V'))
                        {
                            d = new Tile(i, j, 160, 560, TileType.RedDebrisLong, this.Elements, this.RedDebrisLong, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('s'))
                        {
                            d = new Tile(i, j, 400, 400, TileType.SpaceSpiral, this.Elements, this.SpaceSpiral, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('i'))
                        {
                            d = new Tile(i, j, 400, 400, TileType.SpaceMist, this.Elements, this.SpaceMist, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('k'))
                        {
                            d = new Tile(i, j, 40, 40, TileType.DebrisRocks, this.Elements, this.DebrisRocks, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('{'))
                        {
                            d = new Tile(i, j, 40, 720, TileType.PullUp, this.Elements, this.Invisible, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('}'))
                        {
                            d = new Tile(i, j, 40, 720, TileType.PullDown, this.Elements, this.Invisible, this.GameScreen, checkpoint);
                        }
                        else if (Char.IsNumber(ch))
                        {
                            int coinValue = Int16.Parse(Char.GetNumericValue(ch).ToString());
                            if (coinValue == 1 || coinValue == 2 || coinValue == 3)
                            {
                                d = new Coin(i, j, coinValue, this.Elements, this.GameScreen, checkpoint);
                                this.MaxScore += ((Coin)d).Value;
                            }
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

        public override Enemy CreateBlackEnemy(int i, int j, double checkpoint) { return null; }

        public override Enemy CreateGreenEnemy(int i, int j, double checkpoint) { return null; }

        public override Enemy CreateRedEnemy(int i, int j, double checkpoint) { return null; }

        public override Enemy CreateYellowEnemy(int i, int j, double checkpoint) { return null; }

        public override void ProcessPrimaryAction(bool state)
        {
            if (state)  //contracted muscle
            {
                if (GameScreen.GameCharacter.ActionMovement.Equals(ActionMovement.Right))
                {
                    ((GameCharacter)GameScreen.GameCharacter).StopLeftRightMovement();
                }
                else
                {
                    ((GameCharacter)GameScreen.GameCharacter).MoveLeft();
                }
            }
            else
            {
                ((GameCharacter)GameScreen.GameCharacter).StopLeftRightMovement();
            }
        }

        public override void ProcessSecondaryAction(bool state)
        {
            if (state)  //contracted muscle
            {
                if (GameScreen.GameCharacter.ActionMovement.Equals(ActionMovement.Left))
                {
                    ((GameCharacter)GameScreen.GameCharacter).StopLeftRightMovement();
                }
                else
                {
                    ((GameCharacter)GameScreen.GameCharacter).MoveRight();
                }
            }
            else
            {
                ((GameCharacter)GameScreen.GameCharacter).StopLeftRightMovement();
            }
        }
    }
}
