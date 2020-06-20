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
using OpenFeasyo.Platform.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Vub.Etro.IO;


namespace FeasyMotion.C3dSerializer
{

    internal static class ArrayCopyHelper {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }

    public abstract class C3dBaseSerializer {
        protected C3dWriter _writer = null;
        protected string _fileName;
        protected Vub.Etro.IO.Vector4[] _currentData = null;
        protected Int16[] _analogData = null;
        protected int _score = 0;
        protected Int16 _level = -1;
        private DateTime _time;

        protected DataUploading _uploading = null;

        protected string GetCurrentContext()
        {
            string dir = "";
            try
            {
                TextReader tr = new StreamReader("current_context.cfg");
                DirectoryInfo di = new DirectoryInfo(tr.ReadLine());
                dir = di.FullName + "\\";
                tr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Couldn't load current context directory in C3DStreamSerializer");
                Console.WriteLine(e.ToString());
            }
            return dir;
        }

        public static string TimeIdentifier { 
            get {
                return DateTime.Now.Year.ToString("00") +
                       "" + DateTime.Now.Month.ToString("00") +
                       "" + DateTime.Now.Day.ToString("00") +
                       "_" + DateTime.Now.Hour.ToString("00") +
                       "-" + DateTime.Now.Minute.ToString("00") +
                       "-" + DateTime.Now.Second.ToString("00") +
                       "-" + DateTime.Now.Millisecond.ToString("0000");
            } 
        }

        internal void Create(Dictionary<string, string> parameters, IGame game, string[] pointNames, float expectedFrameRate, string[] analogChannelNames = null, Int16 analogSamplesPerFrame = 0, bool eventsEnabled = false) {
            game.GameFinished += (o, args) =>
            {
                _score = (Int16)args.Score;
                _level = (Int16)args.Level;
            };
            string version = SeriousGames.SoftwareVersion;

            string therapist = SeriousGames.GetTherapistName();
            string player = SeriousGames.CurrentPatient != null ? SeriousGames.CurrentPatient.Id : "---";
            string gameName = SeriousGames.CurrentGame != null ? SeriousGames.CurrentGame.Name : "[na]";
            string group = 
                SeriousGames.CurrentPatient != null && 
                SeriousGames.CurrentPatient.Id != null &&
                SeriousGames.CurrentPatient.HospitalId != null ? 
                SeriousGames.CurrentPatient.HospitalId : "---";
            therapist = therapist == null ? "---" : therapist;


            _fileName = SeriousGames.GetPatientDirectory(SeriousGames.CurrentPatient) + "/" + gameName +                   
                    GetTypeName() + TimeIdentifier + ".c3d";
            
            // TODO
            _uploading = new DataUploading();
            _uploading.Id = player == "Default" ? -2 : -1;
            _uploading.PatientId = player;
            _uploading.HospitalId = group;
            _uploading.PerformanceDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            _uploading.C3DFileId = _fileName;
            _uploading.Uploaded = 0;
            _uploading.GameID = game.Definition.GameId;

            _writer = new C3dWriter(pointNames, expectedFrameRate, analogChannelNames, analogSamplesPerFrame, eventsEnabled);
            _writer.SetParameter<string[]>("POINT:DATA_TYPE_LABELS", new string[] {
                "Skeleton",
                "Accelerometer",
                "BalanceBoard",
                "Emg"
            });
            //_writer.Header.AnalogChannels = (short)(7 + game.GameStream.Keys.Count);
            //_analogData = new Int16[_writer.Header.AnalogChannels];
            //_writer.Header.AnalogSamplesPerFrame = 1;

            _writer.SetParameter<string>("SUBJECTS:MARKER_SET", "Using ETRO extended marker set");
            _writer.SetParameter<string>("INFO:SYSTEM", "OpenFeasyo");
            _writer.SetParameter<string>("INFO:EVENT", "gameplay");
            _writer.SetParameter<string>("INFO:GAME_NAME", gameName);

            // set correct value to game level & optimized
            _writer.SetParameter<string>("INFO:GAME_LEVEL_NAME", " --- ");
            _writer.SetParameter<Int16>("INFO:GAME_LEVEL", -1);
            _writer.SetParameter<Int16>("INFO:OPTIMIZED", 0);

            _writer.SetParameter<string>("INFO:VERSION", version);
            //_writer.SetParameter<Int16>("ANALOG:USED", _writer.Header.AnalogChannels);

            _time = DateTime.Now;

            _writer.SetParameter<Int16>("INFO:DURATION", 0);
            _writer.SetParameter<string>("INFO:THERAPIST_ID", therapist);
            _writer.SetParameter<string>("INFO:GROUP_ID", group);
            _writer.SetParameter<string>("SUBJECTS:PLAYER_NAME", player);
            _writer.SetParameter<float>("SUBJECTS:GAME_SCORE", 0.0f);
            _writer.SetParameter<string[]>("INFO:TIME", new string [] {
                _time.Year.ToString(),
                _time.Month.ToString(),
                _time.Day.ToString(),
                _time.Hour.ToString(),
                _time.Minute.ToString(),
            });

            //string [] labels = new string[] { 
            //    "year      ", 
            //    "month     ",
            //    "day       ",
            //    "hour      ", 
            //    "minute    ",
            //    "second    ",
            //    "milisecond"};
            //_writer.SetParameter<string[]>("ANALOG:LABELS", labels.Union<string>(game.GameStream.Keys).ToArray<string>());

        }

        internal void Destroy() {
            float results = _score;
            Int16 level = _level;
            TimeSpan span = DateTime.Now.Subtract(_time);

            _writer.SetParameter<Int16>("INFO:DURATION", (Int16)span.TotalSeconds);
            _writer.SetParameter<float>("SUBJECTS:GAME_SCORE", (float) results /* TODO */);
            _writer.SetParameter<Int16>("INFO:GAME_LEVEL", _level);
            _writer.Close();
            _writer = null;

            _uploading.Results = results.ToString();

            SeriousGames.LocalDatapoint.Insert<DataUploading>(_uploading);
        }

        internal void writeGameObjects(IGame game, int vectorArrayStartIndex) {
            foreach (string s in game.GameObjects.Keys) {
                Vector3 v = game.GameObjects.GetValue(s);
                _currentData[vectorArrayStartIndex++] = new Vub.Etro.IO.Vector4(v.X,v.Y,v.Z,0);
            }
        }

        //internal void WriteAnalogData(IGame game)
        //{
        //    int pos = 0;
        //    _analogData[pos++] = (Int16)DateTime.Now.Year;
        //    _analogData[pos++] = (Int16)DateTime.Now.Month;
        //    _analogData[pos++] = (Int16)DateTime.Now.Day;
        //    _analogData[pos++] = (Int16)DateTime.Now.Hour;
        //    _analogData[pos++] = (Int16)DateTime.Now.Minute;
        //    _analogData[pos++] = (Int16)DateTime.Now.Second;
        //    _analogData[pos++] = (Int16)DateTime.Now.Millisecond;
        //    foreach (string s in game.GameStream.Keys)
        //    {
        //        _analogData[pos++] = (short)game.GameStream.GetValue(s);
        //    }

        //    _writer.WriteIntAnalogData(_analogData);
        //}
        internal abstract string GetTypeName();

    }
}
