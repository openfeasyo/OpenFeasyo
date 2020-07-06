/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */

namespace LoopLib.World.Levels
{
    class LevelGenerator 
    {
        private int currentLevel;

        public LevelGenerator(){
            Reset();
        }

        public void Reset(){
            currentLevel = 0;
        }

        public Level GenerateNext(){
            Level l = null;
            currentLevel ++;
            switch(currentLevel){
                case 1:
                    l = new Level(0.2f,0,30);
                    break;
                case 2:
                    l = new Level(0.3f, 0.01f, 30);
                    break;
                case 3:
                    l = new Level(0.4f, 0.03f, 35);
                    break;
                case 4:
                    l = new Level(0.4f, 0.1f, 40);
                    break;
                case 5:
                    l = new Level(0.5f, 0.2f, 45);
                    break;
                case 6:
                    l = new Level(0.6f, 0.3f, 50);
                    break;
                case 7:
                    l = new Level(0.7f, 0.4f, 53);
                    break;
                case 8:
                    l = new Level(0.6f, 0.5f, 58);
                    break;
                case 9:
                    l = new Level(0.3f, 0.7f, 63);
                    break;
                default:
                    l = new Level(0.5f,1,70);
                    break;
            }
            l.LevelId = currentLevel;
            return l;
        }
    }
}
