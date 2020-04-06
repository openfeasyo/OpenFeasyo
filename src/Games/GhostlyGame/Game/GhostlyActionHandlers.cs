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
using GhostlyLib.Level;

namespace GhostlyLib
{
    class GhostlyActionHandlers
    {
        private static ILevel _currentLevel;
        public static ILevel CurrentLevel { get { return _currentLevel; } set { _currentLevel = value; } }

        //jump / swim
        //public static void PrimaryActionHandle(int source, bool value)
        public static void PrimaryActionHandle(int source, float value)
        {
            if (_currentLevel != null)
            {
                _currentLevel.ProcessPrimaryAction(value > 0 ? true : false);
            }
        }

        // shoot
        public static void SecondaryActionHandle(int source, float value)
        {
            if (_currentLevel != null)
            {
                _currentLevel.ProcessSecondaryAction(value > 0 ? true : false);
            }
        }
    }
    }
