namespace OpenFeasyo.Platform.Controls.Analysis
{
    public interface IAnalysisManager
    {
        bool HasSkeletonAnalyzer(string fileName);

        bool HasAccelerometerAnalyzer(string fileName);

        bool HasBalanceBoardAnalyzer(string fileName);

        bool HasEmgSignalAnalyzer(string fileName);

        ISkeletonAnalyzer GetSkeletonAnalyzer(string fileName);

        IAccelerometerAnalyzer GetAccelerometerAnalyzer(string fileName);

        IBalanceBoardAnalyzer GetBalanceBoardAnalyzer(string fileName);

        IEmgSignalAnalyzer GetEmgSignalAnalyzer(string fileName);

        string GetAnalyzerModuleName(ISkeletonAnalyzer analyzer);

        string GetAnalyzerModuleName(IAccelerometerAnalyzer analyzer);

        string GetAnalyzerModuleName(IBalanceBoardAnalyzer analyzer);

        string GetAnalyzerModuleName(IEmgSignalAnalyzer analyzer);

    }
}
