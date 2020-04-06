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
using OpenFeasyo.Platform.Controls.Drivers;

namespace OpenFeasyo.Platform.Controls.Analysis
{
    internal class AnalysisManager : IAnalysisManager
    {

        private LibraryLoader<ISkeletonAnalyzer> _skeletonAnalysisModules = new LibraryLoader<ISkeletonAnalyzer>();
        private LibraryLoader<IBalanceBoardAnalyzer> _balanceBoardAnalysisModules = new LibraryLoader<IBalanceBoardAnalyzer>();
        private LibraryLoader<IAccelerometerAnalyzer> _accelerometerAnalysisModules = new LibraryLoader<IAccelerometerAnalyzer>();
        private LibraryLoader<IEmgSignalAnalyzer> _emgAnalysisModules = new LibraryLoader<IEmgSignalAnalyzer>();

        public static string ANALYZERS_PATH = "Analyzers";

        //public ObservableCollection<ISkeletonAnalyzer> SkeletonAnalyzers
        //{
        //    get
        //    {
        //        _skeletonAnalysisModules.UpdateModules(ANALYZERS_PATH);
        //        return _skeletonAnalysisModules.LoadedModules;
        //    }
        //}

        //public ObservableCollection<IAccelerometerAnalyzer> AccelerometerAnalyzers
        //{
        //    get
        //    {
        //        _accelerometerAnalysisModules.UpdateModules(ANALYZERS_PATH);
        //        return _accelerometerAnalysisModules.LoadedModules;
        //    }
        //}

        public bool HasSkeletonAnalyzer(string fileName)
        {
            _skeletonAnalysisModules.UpdateModules(ANALYZERS_PATH);
            return _skeletonAnalysisModules.ModuleExists(fileName);
        }

        public bool HasAccelerometerAnalyzer(string fileName)
        {
            _accelerometerAnalysisModules.UpdateModules(ANALYZERS_PATH);
            return _accelerometerAnalysisModules.ModuleExists(fileName);
        }

        public bool HasBalanceBoardAnalyzer(string fileName)
        {
            _balanceBoardAnalysisModules.UpdateModules(ANALYZERS_PATH);
            return _balanceBoardAnalysisModules.ModuleExists(fileName);
        }

        public bool HasEmgSignalAnalyzer(string fileName)
        {
            _emgAnalysisModules.UpdateModules(ANALYZERS_PATH);
            return _emgAnalysisModules.ModuleExists(fileName);
        }


        public ISkeletonAnalyzer GetSkeletonAnalyzer(string fileName)
        {
            _skeletonAnalysisModules.UpdateModules(ANALYZERS_PATH);
            if (!HasSkeletonAnalyzer(fileName))
            {
                return null;
            }
            return _skeletonAnalysisModules.GetModule(fileName);
        }

        public IAccelerometerAnalyzer GetAccelerometerAnalyzer(string fileName)
        {
            _accelerometerAnalysisModules.UpdateModules(ANALYZERS_PATH);
            if (!HasAccelerometerAnalyzer(fileName))
            {
                return null;
            }
            return _accelerometerAnalysisModules.GetModule(fileName);
        }

        public IBalanceBoardAnalyzer GetBalanceBoardAnalyzer(string fileName)
        {
            _balanceBoardAnalysisModules.UpdateModules(ANALYZERS_PATH);
            if (!HasBalanceBoardAnalyzer(fileName))
            {
                return null;
            }
            return _balanceBoardAnalysisModules.GetModule(fileName);
        }

        public IEmgSignalAnalyzer GetEmgSignalAnalyzer(string fileName)
        {
            _emgAnalysisModules.UpdateModules(ANALYZERS_PATH);
            if (!HasEmgSignalAnalyzer(fileName))
            {
                return null;
            }
            return _emgAnalysisModules.GetModule(fileName);
        }

        public string GetAnalyzerModuleName(ISkeletonAnalyzer analyzer)
        {
            _skeletonAnalysisModules.UpdateModules(ANALYZERS_PATH);
            return _skeletonAnalysisModules.GetModuleName(analyzer);
        }

        public string GetAnalyzerModuleName(IAccelerometerAnalyzer analyzer)
        {
            _accelerometerAnalysisModules.UpdateModules(ANALYZERS_PATH);
            return _accelerometerAnalysisModules.GetModuleName(analyzer);
        }

        public string GetAnalyzerModuleName(IBalanceBoardAnalyzer analyzer)
        {
            _balanceBoardAnalysisModules.UpdateModules(ANALYZERS_PATH);
            return _balanceBoardAnalysisModules.GetModuleName(analyzer);
        }

        public string GetAnalyzerModuleName(IEmgSignalAnalyzer analyzer)
        {
            _emgAnalysisModules.UpdateModules(ANALYZERS_PATH);
            return _emgAnalysisModules.GetModuleName(analyzer);
        }
    }
}
