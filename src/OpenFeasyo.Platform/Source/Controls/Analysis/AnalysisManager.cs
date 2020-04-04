using OpenFeasyo.Platform.Controls.Drivers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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
