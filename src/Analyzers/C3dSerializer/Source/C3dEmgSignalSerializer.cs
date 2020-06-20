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
    public class C3dEmgSignalSerializer : C3dBaseSerializer, IEmgSignalAnalyzer, ICloneable
    {
        private string[] labels;
        private new float[] _analogData = null;

        internal override string GetTypeName()
        {
            return "_Emg_";
        }

        public void OnCreate(Dictionary<string, string> parameters, IGame game)
        {
            string[] analogLabels = new string[] {
                "CH1 Raw      ",
                "CH2 Raw      ",
                "CH1 activated",
                "CH2 activated",
            };
            analogLabels = analogLabels.Union<string>(game.GameStream.Keys).ToArray<string>();
            _analogData = new float[analogLabels.Length];

            labels = game.GameObjects.Keys.ToArray<string>();
            _currentData = new Vub.Etro.IO.Vector4[labels.Length];

            Create(parameters, game, labels, 33,analogLabels,30,false);

            _writer.Header.ScaleFactor = -1;
            _writer.SetParameter<float>("POINT:SCALE", -1);
            _writer.SetParameter<Int16>("POINT:DATA_TYPE", 3);
            
            _writer.Open(_fileName);
        }

        public void OnDestroy()
        {
            Destroy();
        }
       
        public void OnEmgSignalChanged(IEmgSignal[] emgSignal, IGame game)
        {
            if (_writer == null)
                return;
           
            
            writeGameObjects(game, 0);

            _writer.WriteFloatFrame(_currentData);
            WriteAnalogData(game, emgSignal);
        }

        private void WriteAnalogData(IGame game, IEmgSignal[] emgSignal)
        {
            if (emgSignal[0].RawSample.Length != 30) {
                throw new ApplicationException("ANALOG:RATE must be 30");
            }
            for(int i = 0; i<emgSignal[0].RawSample.Length; i++) { 

                int pos = 0;
                if (emgSignal.Length > 0)
                {
                    _analogData[pos++] = Convert.ToSingle(emgSignal[0].RawSample[i]);
                }
                if (emgSignal.Length > 1)
                {
                    _analogData[pos++] = Convert.ToSingle(emgSignal[1].RawSample[i]);
                }
                if (emgSignal.Length > 0)
                {
                    _analogData[pos++] = Convert.ToSingle(emgSignal[0].OnOff[i]);
                }
                if (emgSignal.Length > 1)
                {
                    _analogData[pos++] = Convert.ToSingle(emgSignal[1].OnOff[i]);
                }

                foreach (string s in game.GameStream.Keys)
                {
                    _analogData[pos++] = (short)game.GameStream.GetValue(s);
                }

                _writer.WriteFloatAnalogData(_analogData);
            }
        }

        public object Clone()
        {
            return new C3dEmgSignalSerializer();
        }
    }
}
