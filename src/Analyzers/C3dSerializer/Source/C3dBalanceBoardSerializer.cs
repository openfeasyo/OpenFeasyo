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
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FeasyMotion.C3dSerializer
{
    public class C3dBalanceBoardSerializer : C3dBaseSerializer, IBalanceBoardAnalyzer, ICloneable
    {
        private string[] labels;
        //private string[] angleLabels;
        //private string[] qualityLabels;

        internal override string GetTypeName()
        {
            return "_BalanceBoard_";
        }

        public void OnCreate(Dictionary<string, string> parameters, IGame game)
        {
            string[] analogLabels = new string[] {
                "year      ",
                "month     ",
                "day       ",
                "hour      ",
                "minute    ",
                "second    ",
                "milisecond"};

            _analogData = new Int16[analogLabels.Length];

            labels = new string[] { "CoP" };
            string[] allLabels = labels.Union<string>(game.GameObjects.Keys).ToArray<string>();
            _currentData = new Vub.Etro.IO.Vector4[_writer.PointsCount];

            Create(parameters, game, allLabels, 30, analogLabels, 1, false);

            _writer.SetParameter<Int16>("POINT:DATA_TYPE", 2);
            
            _writer.Open(_fileName);

        }

        public void OnDestroy()
        {
            Destroy();
        }

        public void OnBalanceChanged(IBalanceBoard newBalance, IGame game)
        {
            if (_writer == null)
                return;
            _currentData[0] = new Vub.Etro.IO.Vector4(newBalance.CenterOfPressure.X, newBalance.CenterOfPressure.Y, newBalance.Weight, 0);
            
            writeGameObjects(game, (int)labels.Length);
                        
            _writer.WriteIntFrame(_currentData);
            WriteAnalogData(game);
        }

        private void WriteAnalogData(IGame game)
        {
            int pos = 0;
            _analogData[pos++] = (Int16)DateTime.Now.Year;
            _analogData[pos++] = (Int16)DateTime.Now.Month;
            _analogData[pos++] = (Int16)DateTime.Now.Day;
            _analogData[pos++] = (Int16)DateTime.Now.Hour;
            _analogData[pos++] = (Int16)DateTime.Now.Minute;
            _analogData[pos++] = (Int16)DateTime.Now.Second;
            _analogData[pos++] = (Int16)DateTime.Now.Millisecond;
            foreach (string s in game.GameStream.Keys)
            {
                _analogData[pos++] = (short)game.GameStream.GetValue(s);
            }

            _writer.WriteIntAnalogData(_analogData);
        }

        public object Clone()
        {
            return new C3dBalanceBoardSerializer();
        }
    }
}
