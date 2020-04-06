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
using Microsoft.Xna.Framework;
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FeasyMotion.C3dSerializer
{
    public class C3dSkeletonSerializer : C3dBaseSerializer, ISkeletonAnalyzer, ICloneable
    {
        private string[] labels;
        //private string[] angleLabels;
        //private string[] qualityLabels;
        
        internal override string GetTypeName()
        {
            return "_Skeleton_";
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

            labels = Enum.GetNames(typeof(SkeletonMarkers));
            labels = ArrayCopyHelper.SubArray<string>(labels, 0, labels.Length - 1);

            //angleLabels = new string[labels.Length];
            //for (int i = 0; i < labels.Length; i++)
            //{
            //    angleLabels[i] = labels[i] + "Angles";
            //}
            //qualityLabels = new string[labels.Length];
            //for (int i = 0; i < labels.Length; i++)
            //{
            //    qualityLabels[i] = labels[i] + "Quality";
            //}

            string[] allLabels = labels.Union<string>(game.GameObjects.Keys)/*.Union<string>(angleLabels).Union<string>(qualityLabels)*/.ToArray<string>();
            _currentData = new Vub.Etro.IO.Vector4[allLabels.Length];

            Create(parameters,game,allLabels,30,analogLabels,1,false);

            _writer.SetParameter<Int16>("POINT:DATA_TYPE", 0);
            _writer.Open(_fileName);

        }
        public void OnDestroy()
        {
            Destroy();
        }

        public void OnSkeletonChanged(BoneMarkers marker, ISkeleton newSkeleton, IGame game)
        {
            if (_writer == null)
                return;
            for (int i = 0; i < (int)SkeletonMarkers.Count -1; i++)
            {
                Vector3 v = newSkeleton.GetPositionOf((SkeletonMarkers)i); 
                _currentData[i] = new Vub.Etro.IO.Vector4(v.X,v.Y,v.Z,0);
            }
            
            writeGameObjects(game, (int)labels.Length);

            //if (newSkeleton is IRichSkeleton)
            //{
            //  writeSkeletonAngles((IRichSkeleton)newSkeleton, labels.Length + game.GameObjects.Keys.Count);
            //  writeSkeletonQualities((IRichSkeleton)newSkeleton, labels.Length + game.GameObjects.Keys.Count + angleLabels.Length);
            //}
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
            return new C3dSkeletonSerializer();
        }
    }
}
