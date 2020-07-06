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
using System;

namespace LoopLib
{
    public class Globals
    {
        private static int _totalScore;
        public static int TotalScore
        {
            get { return _totalScore; }
            set { _totalScore = value; }
        }

        private static int _lives = -1;
        public static int Lives { 
            get { return _lives; }
            set { 
                
                _lives = Math.Max(0,value);
                if (_lives == 0) {
                    IsFinnished = true;
                }
            }
        }
        
        public static bool IsFinnished { get; set; }
    }
}
