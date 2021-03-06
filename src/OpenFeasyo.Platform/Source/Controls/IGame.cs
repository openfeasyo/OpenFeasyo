﻿/*
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
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using OpenFeasyo.Platform.Controls.Reports;

namespace OpenFeasyo.Platform.Controls
{


    public interface IGame
    {
        void OnReport(IReport report);

        PreDefinedDictionary<Vector3> GameObjects { get; }

        PreDefinedDictionary<double> GameStream { get; }

        ConcurrentQueue<Int16> GameEvents { get; }

        GameDefinition Definition { get; }

        int MaxScore { get; set; }

        string Configuration { get; set; }

        event EventHandler<GameStartedEventArgs> GameStarted;

        event EventHandler<GameFinishedEventArgs> GameFinished;

    }

    public class GameStartedEventArgs : EventArgs
    {
        private string _name;
        private int _level;

        public string Name { get { return _name; } }

        public int Level { get { return _level; } }

        public GameStartedEventArgs(string name, int level) {
            _name = name;
            _level = level;
        }

    }

    public class GameFinishedEventArgs : EventArgs
    {
        public enum EndReason {
            GoalAccomplished,
            GameFailed,
            InteruptedByUser
        }
        private string _name;

        public string Name { get { return _name; } }

        private int _score;

        public int Score { get { return _score; } }

        private int _level;
        public int Level { get { return _level; } }

        private EndReason _reason;
        public EndReason Reason { get { return _reason; } }

        public GameFinishedEventArgs(string name, int score, int level, EndReason reason) {
            _name = name;
            _score = score;
            _reason = reason;
            _level = level;
        }
    }

    public class GameDefinition
    {
        public GameDefinition(string name, string[] bindingPoints, int gameId)
        {
            _name = name;
            _bindingPoints.AddRange(bindingPoints);
            GameId = gameId;
        }

        public int GameId { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private List<string> _bindingPoints = new List<string>();
        public List<string> BindingPoints
        {
            get { return _bindingPoints; }
        }
    }

    public class PreDefinedDictionary<T>
    {

        private ConcurrentDictionary<string, T> _dict;

        public PreDefinedDictionary(IEnumerable<string> keys, T defaultValue)
        {
            _dict = new ConcurrentDictionary<string, T>();
            foreach (string key in keys)
            {
                _dict.TryAdd(key, defaultValue);
            }
        }

        public bool TryUpdate(string name, T value)
        {
            return _dict.TryUpdate(name, value, _dict[name]);
        }

        public T GetValue(string name)
        {
            return _dict[name];
        }

        public ICollection<string> Keys { get { return _dict.Keys; } }

    }
}
