namespace OpenFeasyo.Platform.Controls
{
    public interface IEmgSignal
    {
        bool MuscleActivated { get; }
        double [] RawSample { get; }
        double [] BpfSample { get; }
        double [] AveragedSample { get; }
        double [] FullWaveSample { get; }
        double [] OnOff { get; }
        double [] RestingMean { get; }
        double [] RestingStdev { get; }
    }
}
