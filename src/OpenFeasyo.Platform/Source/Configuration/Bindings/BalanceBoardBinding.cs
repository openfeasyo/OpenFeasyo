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

namespace OpenFeasyo.Platform.Configuration.Bindings
{
    public class BalanceBoardBinding : AnalysableBinding
    {
       public enum MovementOrientation { 
            Horizontal = 0,
            Vertical = 90,
            HorizontaInverted = 180,
            VerticalInverted = 270
        }

        private MovementOrientation _orientation;
        public MovementOrientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (value != _orientation)
                {
                    _input.BalanceChanged -= _event_handler;
                    _orientation = value;
                    _event_handler = GetHandler(_orientation);
                    _input.BalanceChanged += _event_handler;
                }
            }
        }

        private IBalanceBoardInput _input;

        internal override IGamingInput Input
        {
            get { return _input; }
        }

        private EventHandler<BalanceChangedEventArgs> _event_handler;
        private EventHandler<BalanceChangedEventArgs> _analyzer_handler;

        public BalanceBoardBinding(
            IBalanceBoardInput input,
            Configuration.InputValueHandle handle,
            MovementOrientation orientation,
            float displacement,
            float sensitivityRed,
            float sensitivityBlue)
            : base(handle, displacement, sensitivityRed)
        {
            
            this._input = input;
            this._orientation = orientation;

            _event_handler = GetHandler(orientation);
            _input.BalanceChanged += _event_handler;

            _analyzer_handler = new EventHandler<BalanceChangedEventArgs>(_analyzer_handle);
            input.BalanceChanged += _analyzer_handler;
        }

        private EventHandler<BalanceChangedEventArgs> GetHandler(MovementOrientation t)
        {
            switch (t)
            {
                case MovementOrientation.Horizontal:
                    return new EventHandler<BalanceChangedEventArgs>(_input_assign_X);
                case MovementOrientation.HorizontaInverted:
                    return new EventHandler<BalanceChangedEventArgs>(_input_assign_X_inverted);
                case MovementOrientation.Vertical:
                    return new EventHandler<BalanceChangedEventArgs>(_input_assign_Y);
                case MovementOrientation.VerticalInverted:
                    return new EventHandler<BalanceChangedEventArgs>(_input_assign_Y_inverted);
                default:
                    throw new ApplicationException("Unknown balance board binding type!");
            }
        }

        void _input_assign_X(object sender, BalanceChangedEventArgs e)
        {
            CallHandle((int)MovementOrientation.Horizontal, e.Balance.CenterOfPressure.X);
        }

        void _input_assign_Y(object sender, BalanceChangedEventArgs e)
        {
            CallHandle((int)MovementOrientation.Vertical, e.Balance.CenterOfPressure.Y);
        }

        void _input_assign_X_inverted(object sender, BalanceChangedEventArgs e)
        {
            CallHandle((int)MovementOrientation.Horizontal, -e.Balance.CenterOfPressure.X);
        }

        void _input_assign_Y_inverted(object sender, BalanceChangedEventArgs e)
        {
            CallHandle((int)MovementOrientation.Vertical, -e.Balance.CenterOfPressure.Y);
        }

        internal override void destroy()
        {
            _input.BalanceChanged -= _event_handler;
            _input.BalanceChanged -= _analyzer_handler;
            foreach (AnalyzerWrapper executor in _analyzerWrapers)
            {
                executor.Stop();
            }
        }



        private HashSet<AnalyzerWrapper> _analyzerWrapers = new HashSet<AnalyzerWrapper>();

        public void AddAnalyzer(IBalanceBoardAnalyzer analyzer, ObservableDictionary<string, string> parameters)
        {
            AnalyzerWrapper executor = new AnalyzerWrapper(analyzer, parameters);
            executor.Run();
            _analyzerWrapers.Add(executor);
            _analyzers.Add(analyzer, parameters);
        }

        void _analyzer_handle(object sender, BalanceChangedEventArgs e)
        {
            foreach (AnalyzerWrapper analyzer in _analyzerWrapers)
            {
                analyzer.ProcessBalanceBoard(e.Balance);
            }
        }

    }
}
