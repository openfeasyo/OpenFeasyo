using System;
using System.Collections.Generic;
using FeasyMotion.C3dSerializer;
using OpenFeasyo.Platform.Controls.Analysis;

namespace GhostlyLib
{
    public class StaticAnalysisManager : IAnalysisManager
    {
        private Dictionary<string, ISkeletonAnalyzer> _skeletonAnalyzers = new Dictionary<string, ISkeletonAnalyzer>();
        private Dictionary<string, IBalanceBoardAnalyzer> _balanceBoardAnalyzers = new Dictionary<string, IBalanceBoardAnalyzer>();
        private Dictionary<string, IAccelerometerAnalyzer> _accelerometerAnalyzers = new Dictionary<string, IAccelerometerAnalyzer>();
        private Dictionary<string, IEmgSignalAnalyzer> _emgAnalyzers = new Dictionary<string, IEmgSignalAnalyzer>();

        public StaticAnalysisManager()
        {
            _skeletonAnalyzers.Add("C3dSerializer.dll".ToLower(), new C3dSkeletonSerializer());
            _balanceBoardAnalyzers.Add("C3dSerializer.dll".ToLower(), new C3dBalanceBoardSerializer());
            _emgAnalyzers.Add("C3dSerializer.dll".ToLower(), new C3dEmgSignalSerializer());
        }

        public bool HasSkeletonAnalyzer(string fileName)
        {
            return _skeletonAnalyzers.ContainsKey(fileName.ToLower());
        }

        public bool HasAccelerometerAnalyzer(string fileName)
        {
            return _accelerometerAnalyzers.ContainsKey(fileName.ToLower());
        }

        public bool HasBalanceBoardAnalyzer(string fileName)
        {
            return _balanceBoardAnalyzers.ContainsKey(fileName.ToLower());
        }

        public bool HasEmgSignalAnalyzer(string fileName)
        {
            return _emgAnalyzers.ContainsKey(fileName.ToLower());
        }

        public ISkeletonAnalyzer GetSkeletonAnalyzer(string fileName)
        {
            if (_skeletonAnalyzers.ContainsKey(fileName.ToLower()))
            {
                return _skeletonAnalyzers[fileName.ToLower()];
            }
            return null;
        }

        public IAccelerometerAnalyzer GetAccelerometerAnalyzer(string fileName)
        {
            if (_accelerometerAnalyzers.ContainsKey(fileName.ToLower()))
            {
                return _accelerometerAnalyzers[fileName.ToLower()];
            }
            return null;
        }

        public IBalanceBoardAnalyzer GetBalanceBoardAnalyzer(string fileName)
        {
            if (_balanceBoardAnalyzers.ContainsKey(fileName.ToLower()))
            {
                return _balanceBoardAnalyzers[fileName.ToLower()];
            }
            return null;
        }

        public IEmgSignalAnalyzer GetEmgSignalAnalyzer(string fileName)
        {
            if (_emgAnalyzers.ContainsKey(fileName.ToLower()))
            {
                return _emgAnalyzers[fileName.ToLower()];
            }
            return null;
        }

        public string GetAnalyzerModuleName(ISkeletonAnalyzer analyzer)
        {
            foreach (string moduleName in _skeletonAnalyzers.Keys)
            {
                if (_skeletonAnalyzers[moduleName].GetType().Equals(analyzer.GetType()))
                {
                    return moduleName.ToLower();
                }
            }
            throw new ApplicationException("Module is not in the loaded modules. Something went seriously wrong, please, contact the support department!");
        }

        public string GetAnalyzerModuleName(IAccelerometerAnalyzer analyzer)
        {
            foreach (string moduleName in _accelerometerAnalyzers.Keys)
            {
                if (_accelerometerAnalyzers[moduleName].GetType().Equals(analyzer.GetType()))
                {
                    return moduleName.ToLower();
                }
            }
            throw new ApplicationException("Module is not in the loaded modules. Something went seriously wrong, please, contact the support department!");
        }

        public string GetAnalyzerModuleName(IBalanceBoardAnalyzer analyzer)
        {
            foreach (string moduleName in _balanceBoardAnalyzers.Keys)
            {
                if (_balanceBoardAnalyzers[moduleName].GetType().Equals(analyzer.GetType()))
                {
                    return moduleName.ToLower();
                }
            }
            throw new ApplicationException("Module is not in the loaded modules. Something went seriously wrong, please, contact the support department!");
        }

        public string GetAnalyzerModuleName(IEmgSignalAnalyzer analyzer)
        {
            foreach (string moduleName in _emgAnalyzers.Keys)
            {
                if (_emgAnalyzers[moduleName].GetType().Equals(analyzer.GetType()))
                {
                    return moduleName.ToLower();
                }
            }
            throw new ApplicationException("Module is not in the loaded modules. Something went seriously wrong, please, contact the support department!");
        }
    }
}