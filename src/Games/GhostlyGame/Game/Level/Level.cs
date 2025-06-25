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
using GhostlyLib.Animations;
using GhostlyLib.Elements;
using GhostlyLib.Elements.Character;
using GhostlyLib.Elements.Enemies;
using GhostlyLib.Screens;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace GhostlyLib.Level
{
    public abstract class Level : ILevel2D
    {
        private GameScreen gameScreen;

        public abstract Texture2D Background { get; }
        public abstract Texture2D BackgroundClosest { get; }
        public abstract Texture2D BackgroundCloser { get; }
        public abstract Texture2D BackgroundClose { get; }
        public abstract Texture2D BackgroundFar { get; }
        public abstract Texture2D BackgroundFurther { get; }
        public abstract Texture2D BackgroundFurthest { get; }

        public abstract EnemyAnimation BlackEnemyAnimation { get; }
        public abstract Texture2D CliffLeft { get; }
        public abstract Texture2D CliffRight { get; }
        public abstract LevelElements Elements { get; }
        public abstract Texture2D Dirt { get; }
        public abstract Texture2D Foreground { get; }
        public abstract EnemyAnimation GreenEnemyAnimation { get; }
        public abstract Texture2D Ground { get; }
        public abstract Texture2D LargeHill { get; }
        public abstract EnemyAnimation RedEnemyAnimation { get; }
        public abstract Texture2D SmallHill { get; }
        public abstract EnemyAnimation YellowEnemyAnimation { get; }
        public abstract Texture2D ExitSign { get; }

        public Texture2D Crate { get { return ImagesAndAnimations.Instance.Crate; } }
        public Texture2D DeepLava { get { return ImagesAndAnimations.Instance.DeepLava; } }
        public Texture2D DeepWater { get { return ImagesAndAnimations.Instance.DeepWater; } }
        public Texture2D Exclamation { get { return ImagesAndAnimations.Instance.Exclamation; } }
        public Texture2D ExitArea { get { return ImagesAndAnimations.Instance.ExitArea; } }
        public Texture2D Fence { get { return ImagesAndAnimations.Instance.Fence; } }
        public Texture2D Invisible { get { return ImagesAndAnimations.Instance.InvisibleTile; } }
        public Texture2D Lava { get { return ImagesAndAnimations.Instance.Lava; } }
        public Texture2D Water { get { return ImagesAndAnimations.Instance.Water; } }

        //Textures for space levels
        public abstract Texture2D BluePlanet { get; }
        public abstract Texture2D YellowPlanet { get; }
        public abstract Texture2D OrangePlanet { get; }
        public abstract Texture2D PinkPlanet { get;}
        public abstract Texture2D RedPlanet { get; }

        public abstract Texture2D Star { get; }
        public abstract Texture2D Ufo { get; }
        public abstract Texture2D Debris { get; }
        public abstract Texture2D SpaceSpiral { get; }
        public abstract Texture2D SpaceMist { get; }

        public int MaxScore { get; set; }

        //public abstract GameCharacter Character { get; protected set; }

        public GameScreen GameScreen { get { return gameScreen; } }

        public abstract IGameCharacter Character { get; set; }

        protected Level(GameScreen gameScreen) {
            this.gameScreen = gameScreen;
        }

        public abstract void ProcessPrimaryAction(bool state);
        public abstract void ProcessSecondaryAction(bool state);

        public abstract Enemy CreateBlackEnemy(int i, int j, double checkpoint);
        public abstract Enemy CreateGreenEnemy(int i, int j, double checkpoint);
        public abstract Enemy CreateRedEnemy(int i, int j, double checkpoint);
        public abstract Enemy CreateYellowEnemy(int i, int j, double checkpoint);

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
            else { 
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Level)).Assembly;
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

                        Drawable d = null;

                        if (ch.Equals('#'))
                        {
                            d = new Tile(i, j, TileType.Dirt, this.Elements, this.Dirt, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('l'))
                        {
                            d = new Tile(i, j, TileType.Ground, this.Elements, this.CliffLeft, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('m'))
                        {
                            d = new Tile(i, j, TileType.Ground, this.Elements, this.Ground, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('r'))
                        {
                            d = new Tile(i, j, TileType.Ground, this.Elements, this.CliffRight, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('~'))
                        {
                            d = new Tile(i, j, TileType.Water, this.Elements, this.Water, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('%'))
                        {
                            d = new Tile(i, j, TileType.DeepWater, this.Elements, this.DeepWater, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('!'))
                        {
                            d = new Tile(i, j, TileType.Exclamation, this.Elements, this.Exclamation, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('\\'))
                        {
                            d = new Tile(i, j, TileType.Crate, this.Elements, this.Crate, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('='))
                        {
                            d = new Tile(i, j, TileType.Fence, this.Elements, this.Fence, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('\''))
                        {
                            d = new Tile(i, j, TileType.Lava, this.Elements, this.Lava, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('"'))
                        {
                            d = new Tile(i, j, TileType.DeepLava, this.Elements, this.DeepLava, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('|'))
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
                        else if (ch.Equals('h'))
                        {
                            d = new Tile(i, j, TileType.HillSmall, this.Elements, this.SmallHill, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('H'))
                        {
                            d = new Tile(i, j, TileType.HillLarge, this.Elements, this.LargeHill, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('B'))
                        {
                            d = this.CreateBlackEnemy(i, j, checkpoint);
                        }
                        else if (ch.Equals('G'))
                        {
                            d = this.CreateGreenEnemy(i, j, checkpoint);
                        }
                        else if (ch.Equals('R'))
                        {
                            d = this.CreateRedEnemy(i, j, checkpoint);
                        }
                        else if (ch.Equals('Y'))
                        {
                            d = this.CreateYellowEnemy(i, j, checkpoint);
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
                        else if (ch.Equals('r'))
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
                        else if (ch.Equals('u'))
                        {
                            d = new Tile(i, j, 80, 80, TileType.Ufo, this.Elements, this.Ufo, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('d'))
                        {
                            d = new Tile(i, j, 160, 320, TileType.Debris, this.Elements, this.Debris, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('D'))
                        {
                            d = new Tile(i, j, 160, 480, TileType.DebrisLong, this.Elements, this.Debris, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('s'))
                        {
                            d = new Tile(i, j, 400, 400, TileType.SpaceSpiral, this.Elements, this.SpaceSpiral, this.GameScreen, checkpoint);
                        }
                        else if (ch.Equals('i'))
                        {
                            d = new Tile(i, j, 400, 400, TileType.SpaceMist, this.Elements, this.SpaceMist, this.GameScreen, checkpoint);
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

        public IEnemy GenerateEnemy(int i, int j)
        {
            throw new NotImplementedException();
        }
    }
}
