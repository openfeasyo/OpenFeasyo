using System;

namespace OpenFeasyo.Platform.Controls.Analysis
{
    public class InputAnalyzerManager
    {
        private static IAnalysisManager _instance = null;

        public static IAnalysisManager Instance
        {
            set
            {
                _instance = value;
            }

            get
            {
                if (_instance == null)
                {
                    _instance = new AnalysisManager();
                }
                return _instance;
            }
        }

        private static IGame _currentGame = null;
        public static IGame CurrentGame
        {
            get { return _currentGame; }
            set
            {
                _currentGame = value;
            }
        }

        //
        // A general method for cloning analysers to return completely new instance when asked for it.
        //
        private static T TryClone<T>(T analyzer)
        {
            if (analyzer is ICloneable)
            { // Try it
                return (T)((ICloneable)analyzer).Clone();
            }
            return analyzer; // Nevermind, return what you have
        }

        public static bool HasSkeletonAnalyzer(string fileName)
        {
            return Instance.HasSkeletonAnalyzer(fileName);
        }

        public static bool HasAccelerometerAnalyzer(string fileName)
        {
            return Instance.HasAccelerometerAnalyzer(fileName);
        }

        public static bool HasBalanceBoardAnalyzer(string fileName)
        {
            return Instance.HasBalanceBoardAnalyzer(fileName);
        }

        public static bool HasEmgSignalAnalyzer(string fileName)
        {
            return Instance.HasEmgSignalAnalyzer(fileName);
        }


        public static ISkeletonAnalyzer GetSkeletonAnalyzer(string fileName)
        {
            return TryClone<ISkeletonAnalyzer>(Instance.GetSkeletonAnalyzer(fileName));
        }

        public static IAccelerometerAnalyzer GetAccelerometerAnalyzer(string fileName)
        {
            return TryClone<IAccelerometerAnalyzer>(Instance.GetAccelerometerAnalyzer(fileName));
        }

        public static IBalanceBoardAnalyzer GetBalanceBoardAnalyzer(string fileName)
        {
            return TryClone<IBalanceBoardAnalyzer>(Instance.GetBalanceBoardAnalyzer(fileName));
        }

        public static IEmgSignalAnalyzer GetEmgSignalAnalyzer(string fileName)
        {
            return TryClone<IEmgSignalAnalyzer>(Instance.GetEmgSignalAnalyzer(fileName));
        }


        public static string GetAnalyzerModuleName(ISkeletonAnalyzer analyzer)
        {
            return Instance.GetAnalyzerModuleName(analyzer);
        }

        public static string GetAnalyzerModuleName(IAccelerometerAnalyzer analyzer)
        {
            return Instance.GetAnalyzerModuleName(analyzer);
        }

        public static string GetAnalyzerModuleName(IBalanceBoardAnalyzer analyzer)
        {
            return Instance.GetAnalyzerModuleName(analyzer);
        }

        public static string GetAnalyzerModuleName(IEmgSignalAnalyzer analyzer)
        {
            return Instance.GetAnalyzerModuleName(analyzer);
        }
    }
}
