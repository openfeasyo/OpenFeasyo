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
using GhostlyLib.Elements.Character;
using GhostlyLib.Elements.Enemies;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GhostlyLib.Level
{
    public interface ILevel
    {
        /*  Texture2D Background { get; }
          Texture2D BackgroundClosest { get; }
          Texture2D BackgroundCloser { get; }
          Texture2D BackgroundClose { get; }
          Texture2D BackgroundFar { get; }
          Texture2D BackgroundFurther { get; }
          Texture2D BackgroundFurthest { get; }

          EnemyAnimation BlackEnemyAnimation { get; }
          Texture2D CliffLeft { get; }
          Texture2D CliffRight { get; }
          Texture2D Crate { get; }
          Texture2D DeepLava { get; }
          Texture2D DeepWater { get; }
          Texture2D Dirt { get; }
          Texture2D ExitSign { get; }
          Texture2D Exclamation { get; }
          Texture2D Fence { get; }
          Texture2D Foreground { get; }
          EnemyAnimation GreenEnemyAnimation { get; }
          Texture2D Ground { get; }
          Texture2D Invisible { get; }
          Texture2D LargeHill { get; }
          Texture2D Lava { get; }
          EnemyAnimation RedEnemyAnimation { get; }
          Texture2D SmallHill { get; }
          Texture2D Water { get; }
          EnemyAnimation YellowEnemyAnimation { get; }


          //Textures for space levels
          Texture2D BluePlanet { get; }
          Texture2D YellowPlanet { get; }
          Texture2D OrangePlanet { get; }
          Texture2D PinkPlanet { get; }
          Texture2D RedPlanet { get; }


          int MaxScore { get; set; }
          void ProcessPrimaryAction(bool state);
          void ProcessSecondaryAction(bool state);
          void LoadMap(String p, double checkpoint);

          GameCharacter Character { get; }*/


        IGameCharacter Character { get; set;  }
        Texture2D Background { get; }
        int MaxScore { get; set; }
        void ProcessPrimaryAction(bool state);
        void ProcessSecondaryAction(bool state);
        void LoadMap(String p, double checkpoint);
        //IEnemy GenerateEnemy(int i, int j);
    }
}
