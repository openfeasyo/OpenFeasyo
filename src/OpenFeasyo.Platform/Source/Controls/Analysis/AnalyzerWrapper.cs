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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using OpenFeasyo.Platform.Controls.Reports;
using OpenFeasyo.Platform.Platform;

namespace OpenFeasyo.Platform.Controls.Analysis
{
    public class AnalyzerWrapper
    {

        private abstract class IProcessRequest
        {
            abstract internal void Process(IGame game);
        }

        private class SkeletonProcessRequest : IProcessRequest
        {
            internal ISkeletonAnalyzer Analyzer { get; set; }
            internal BoneMarkers CurrentMarker { get; set; }
            internal ISkeleton CurrentSkeleton { get; set; }
            internal override void Process(IGame game)
            {
                Analyzer.OnSkeletonChanged(CurrentMarker, CurrentSkeleton, game);
            }
        }

        private class AccelerometerProcessRequest : IProcessRequest
        {
            internal IAccelerometerAnalyzer Analyzer { get; set; }
            internal IAccelerometer CurrentAccelerometer { get; set; }
            internal override void Process(IGame game)
            {
                Analyzer.OnAccelerometerChanged(CurrentAccelerometer, game);
            }
        }

        private class BalanceBoardProcessRequest : IProcessRequest
        {
            internal IBalanceBoardAnalyzer Analyzer { get; set; }
            internal IBalanceBoard CurrentBalance { get; set; }
            internal override void Process(IGame game)
            {
                Analyzer.OnBalanceChanged(CurrentBalance, game);
            }
        }

        private class EmgSignalProcessRequest : IProcessRequest {
            internal IEmgSignalAnalyzer Analyzer { get; set; }
            internal IEmgSignal[] CurrentEmgSignal { get; set; }
            internal override void Process(IGame game)
            {
                Analyzer.OnEmgSignalChanged(CurrentEmgSignal, game);
            }
        }


        private class ProxyGame : IGame
        {
            private IGame _game = null;

            
            public ProxyGame(IGame game)
            {
                _game = game;
                
                _game.GameFinished += OnGameFinished;
                _game.GameStarted += OnGameStarted;
            }

            

            public void OnReport(IReport report)
            {
                UIThread.Invoke(
                    new Action(() =>
                    {
                        _game.OnReport(report);
                    }));
            }

            public PreDefinedDictionary<Vector3> GameObjects
            {
                get
                {
                    return _game.GameObjects;
                }
            }

            public PreDefinedDictionary<double> GameStream
            {
                get
                {
                    return _game.GameStream;
                }
            }

            public ConcurrentQueue<Int16> GameEvents
            {
                get
                {
                    return _game.GameEvents;
                }
            }


            public GameDefinition Definition
            {
                get 
                {
                    return _game.Definition;
                }
            }

            public int MaxScore {
                get { return _game.MaxScore; }
                set { _game.MaxScore = value; }
            }

            public string Configuration {
                get { return _game.Configuration; }
                set { _game.Configuration = value; }
            }

            public event EventHandler<GameStartedEventArgs> GameStarted;

            public void OnGameStarted(object sender, GameStartedEventArgs args) {
                //_dispatcher.Invoke(
                    //new Action(() =>
                    //{
                        if (GameStarted != null)
                        {
                            GameStarted(sender, args);
                        }
                    //}), System.Windows.Threading.DispatcherPriority.Normal);
                
            }

            public event EventHandler<GameFinishedEventArgs> GameFinished;

            public void OnGameFinished(object sender, GameFinishedEventArgs args)
            {
                //_dispatcher.Invoke(
                //    new Action(() =>
                //    {
                        if (GameFinished != null)
                        {
                            GameFinished(sender, args);
                        }
                //    }), System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        private ProxyGame _proxyGame;
        private IAnalyzer _analyzer;
        private BackgroundWorker _bw = null;

        private object _lockObject = new object();
        private object _lock = new object();
        private ConcurrentQueue<IProcessRequest> _requestQueue;
        private Dictionary<string, string> _parameters;

        public AnalyzerWrapper(IAnalyzer analyzer, Dictionary<string, string> parameters)
        {
            _analyzer = analyzer;
            _parameters = parameters;

            _requestQueue = new ConcurrentQueue<IProcessRequest>();

            _proxyGame = new ProxyGame(InputAnalyzerManager.CurrentGame);

            _bw = new BackgroundWorker();
            _bw.WorkerReportsProgress = true;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += new DoWorkEventHandler(_bw_DoWork);
            _bw.ProgressChanged += new ProgressChangedEventHandler(_bw_ProgressChanged);
            _bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bw_RunWorkerCompleted);
        }
        private void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //       throw new NotImplementedException();
        }

        private void _bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //       throw new NotImplementedException();
        }

        private void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (InputAnalyzerManager.CurrentGame == null) return;
            _analyzer.OnCreate(_parameters, _proxyGame);
            IProcessRequest p;
            while (!_bw.CancellationPending)
            {
                if (_requestQueue.TryDequeue(out p))
                {
                    p.Process(_proxyGame);
                }
                else if (_requestQueue.IsEmpty)
                {
                    lock (_lockObject)
                    {
                        Monitor.Wait(_lockObject);
                    }
                }
            }
            _analyzer.OnDestroy();
        }

        public void ProcessSkeleton(BoneMarkers marker, ISkeleton skeleton)
        {
            SkeletonProcessRequest p = new SkeletonProcessRequest();
            p.Analyzer = _analyzer as ISkeletonAnalyzer;
            p.CurrentMarker = marker;
            p.CurrentSkeleton = skeleton;

            lock (_lockObject)
            {
                _requestQueue.Enqueue(p);

                if (_requestQueue.Count == 1) // the only element that we just inserted is there
                {
                    Monitor.Pulse(_lockObject);
                }
            }
        }

        public void ProcessAccelerometer(IAccelerometer accelrometer)
        {
            AccelerometerProcessRequest p = new AccelerometerProcessRequest();
            p.Analyzer = _analyzer as IAccelerometerAnalyzer;
            p.CurrentAccelerometer = accelrometer;

            lock (_lockObject)
            {
                _requestQueue.Enqueue(p);

                if (_requestQueue.Count == 1) // the only element that we just inserted is there
                {
                    Monitor.Pulse(_lockObject);
                }
            }
        }

        public void ProcessBalanceBoard(IBalanceBoard balanceBoard)
        {
            BalanceBoardProcessRequest p = new BalanceBoardProcessRequest();
            p.Analyzer = _analyzer as IBalanceBoardAnalyzer;
            p.CurrentBalance = balanceBoard;

            lock (_lockObject)
            {
                _requestQueue.Enqueue(p);

                if (_requestQueue.Count == 1) // the only element that we just inserted is there
                {
                    Monitor.Pulse(_lockObject);
                }
            }
        }

        public void ProcessEmgSignal(IEmgSignal[] emgSignal) {
            EmgSignalProcessRequest p = new EmgSignalProcessRequest();
            p.Analyzer = _analyzer as IEmgSignalAnalyzer;
            p.CurrentEmgSignal = emgSignal;

            lock (_lockObject)
            {
                _requestQueue.Enqueue(p);

                if (_requestQueue.Count >= 1) // the only element that we just inserted is there
                {
                    Monitor.Pulse(_lockObject);
                }
            }
        }
        


        public void Run()
        {
            _bw.RunWorkerAsync();
        }

        public void Stop()
        {
            _bw.CancelAsync();
            lock (_lockObject)
            {
                Monitor.Pulse(_lockObject);
            }
        }



    }
}
