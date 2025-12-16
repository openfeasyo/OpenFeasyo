using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Drivers;

namespace TrignoAvantiCustom;

public enum CalibrationState
{
    Uncalibrated,
    Calibrating,
    Calibrated
}

public class TrignoEmgInput : IEmgSensorInput
{
    private TrignoAvanti _device;
    private List<Sensor> _sensors = new();
    public TrignoEmgInput(TrignoAvanti ta)
    {
        ActivationThreshold = new[] { 0.5f, 0.5f };
        _device = ta;
        _device.ConnectionEstablished += (sender, args) =>
        {
            if(_sensors.Count() == 0 ||                                                                     
                !_sensors.Any(s => s.Name != null && s.Name.EndsWith(args.Devices)))
                if (args.Sensor is Sensor)
                {
                    _sensors.Add(args.Sensor as Sensor);
                    (args.Sensor as Sensor).MuscleActivationChanged += OnMuscleActivationChanged;
                    (args.Sensor as Sensor).CalibrationChanged += OnCalibrationChanged;

                }
        };
    }

    public IDevice Device
    {
        get { return _device; }
    }

    public event EventHandler<MuscleActivationChangedEventArgs>? MuscleActivationChanged;

    private void OnMuscleActivationChanged(object sender, MuscleActivationChangedEventArgs e)
    {
        MuscleActivationChanged?.Invoke(sender, e);
    }


    public event EventHandler<CalibrationChangedEventArgs>? CalibrationChanged;

    private void OnCalibrationChanged(object sender, CalibrationChangedEventArgs e)
    {
        CalibrationChanged?.Invoke(sender, e);
    }
    
    public void Calibrate()
    {
        foreach (var s in _sensors)
        {
            s.Calibrate();
        }
    }

    public float[] ActivationThreshold { get; }
}

public class TrignoEmgSignal : IEmgSignal
{
    public TrignoEmgSignal(double[] rawSample, int channel)
    {
        RawSample = rawSample;
        Channel = channel;
        FullWaveSample = new double[rawSample.Length];
        AveragedSample = new double[rawSample.Length];
        OnOff = new double[rawSample.Length];
        RestingMean = new double[rawSample.Length];
        RestingStdev = new double[rawSample.Length];
    }

    public int Channel { get; private set; } 
        
    public bool MuscleActivated { get; set; }

    public double[] RawSample { get; set; }

    public double[] BpfSample { get; set; }

    public double[] AveragedSample { get; set; }

    public double[] FullWaveSample { get; set; }

    public double[] OnOff { get; set; }

    public double[] RestingMean { get; set; }

    public double[] RestingStdev { get; set; }
}