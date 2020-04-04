namespace OpenFeasyo.Platform.Controls.Analysis
{
    public interface IEmgSignalAnalyzer : IAnalyzer
    {
        void OnEmgSignalChanged(IEmgSignal[] emgSignal, IGame game);
    }
}
