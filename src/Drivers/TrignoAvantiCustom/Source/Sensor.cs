using OpenFeasyo.Platform.Controls;
using Plugin.BLE.Abstractions.Contracts;
using System.Diagnostics;

namespace TrignoAvantiCustom;

public class Sensor
{
    private TrignoAvanti.BufferAssembler _buffer = new TrignoAvanti.BufferAssembler();

    private string _name;
    public string Name {  get => _name; }

    public int Channel { get; private set; }
    

    public Sensor(string name, int channel)
    {
        _name = name;
        InitializeSignalProcessing();
        ActivationThreshold = new float[2];
        Channel = channel;

    }

    private List<Guid> _services = new List<Guid>();

    public List<Guid> Services
    {
        get { return _services; }
    }

    public byte[]? BatteryStatus { get; set; } = null;
    public byte[]? OriginalConfig { get; set; } = null;
    public byte[]? PowerStatus { get; set; } = null;
    
    public ICharacteristic? DataStreamer { get; set; } = null;


    private DateTime _lastTime = DateTime.Now; // marks the beginning the measurement began
    private int _framesReceived = 0; // an increasing count
    private int _fps = 0;

    public const int SAMPLES_PER_PACKET = 15;
    private double[] signal = new double[SAMPLES_PER_PACKET]; // configuration 1 has 15 bytes
    private const double Scale = 3.3 / 19660800;
    private const int Offset = 32768;
    private bool firstCall = false;
 
   
    
    internal void ParseSensorData(byte[] data)
    {
        if (firstCall == false)
        {
            Console.WriteLine("First call from "+ this.Name);
            firstCall = true;
        }

        var completedData = _buffer.AddChunk(data);
        foreach (var packet in completedData)
        {
            for (int i = 0; i < SAMPLES_PER_PACKET; i++)
            {
                double d = packet[4 + (i * 2)] << 8 | packet[4 + (i * 2)+1];//BitConverter.ToUInt16(packet, 4 + (i * 2)); // packet[4 + (i * 2)+1] << 8 | packet[4 + (i * 2)];
                signal[i] = (d - Offset) * Scale;
            }
            
            
            processPacket(signal, _buffer.LastPacketId);

            _framesReceived++;
            // FPS Updates
            if ((DateTime.Now - _lastTime).TotalSeconds >= 1)
            {
                _fps = _framesReceived;
                _framesReceived = 0;
                _lastTime = DateTime.Now;
                Console.WriteLine("Update freq. " + _name + ": " + _fps + " fps " + data.Length);
            }
        }
    }

   
    private void processPacket(double[] packet, int packetId)
    {
        //var data = new double[][] { packet };
        
        switch (_calibrationState)
        {
            case CalibrationState.Calibrating:
                Console.WriteLine("CALIBRATING");
                Train(packet);
                OnMuscleActivationChanged(new MuscleActivationChangedEventArgs(new []{new TrignoEmgSignal(packet,Channel,packetId)}));
                break;
            case CalibrationState.Calibrated:
                OnMuscleActivationChanged(new MuscleActivationChangedEventArgs(Process(packet,packetId)));
                break;
            default: //CalibrationState.Uncalibrated
                OnMuscleActivationChanged(new MuscleActivationChangedEventArgs(new []{new TrignoEmgSignal(packet,Channel,packetId)} ));
                break;
        }
        
       
    }


    public event EventHandler<MuscleActivationChangedEventArgs> MuscleActivationChanged;
    private void OnMuscleActivationChanged(MuscleActivationChangedEventArgs args) {
        if (MuscleActivationChanged != null) {
            MuscleActivationChanged(this, args);
        }
    }

    
    public override string ToString()
    {
        string FormatBytes(byte[]? data) =>
            data == null ? "null" : BitConverter.ToString(data);

        return $"Sensor:\n" +
               $"- Services: {(Services.Count > 0 ? string.Join(", ", Services) : "none")}\n" +
               $"- BatteryStatus: {FormatBytes(BatteryStatus)}\n" +
               $"- OriginalConfig: {FormatBytes(OriginalConfig)}\n" +
               $"- PowerStatus: {FormatBytes(PowerStatus)}";
    }

    public float[] ActivationThreshold { get; }

    #region EMG signal processing related variables

    private int _samplingRate = 1000;

    private BandPassFilter[] _bandPassFilters;
    private int _nChannels = 1;

    private double _movingWindowLength = 1;
    private List<double>[] _movingWindowData;

    private List<double>[] _baselineData;
    private int[] _baselineDataCounters;

    private int _baselineDataLength = 100;
    private int _baselineThrowOut = 20;

    private double[] _baselineMean;
    private double[] _baselineStdev;

    private CalibrationState _calibrationState = CalibrationState.Uncalibrated;

    #endregion

    #region EMG signal processing

    private void InitializeSignalProcessing()
    {
        this._movingWindowLength = Math.Floor((float)_samplingRate / 10);
        this._baselineDataLength = (int)_samplingRate * 2;
        this._baselineThrowOut = (int)Math.Floor((float)_baselineDataLength / 5);

        this._movingWindowData = new List<double>[_nChannels];
        this._baselineData = new List<double>[_nChannels];
        this._bandPassFilters = new BandPassFilter[_nChannels];

        this._baselineDataCounters = new int[_nChannels];
        this._baselineMean = new double[_nChannels];
        this._baselineStdev = new double[_nChannels];

        for (int i = 0; i < _nChannels; i++)
        {
            this._movingWindowData[i] = new List<double>();
            this._baselineData[i] = new List<double>();
            this._bandPassFilters[i] =
                new BandPassFilter(BandPassFilter.BAND_PASS, _samplingRate, new double[] { 5, 25 }, 6);

            this._baselineDataCounters[i] = 0;
            this._baselineMean[i] = -1;
            this._baselineStdev[i] = -1;
        }
    }

    private void TrainOldOneParam(double[] rawData)
    {
        int count = Math.Min(_nChannels, rawData.Length);

        for (int index = 0; index < count; index++)
        {
            double filtered = _bandPassFilters[index].filterData(rawData[index]);

            // Full wave rectification
            double value = Math.Abs(filtered);

            if (_baselineDataCounters[index] < _baselineThrowOut)
            {
                //throw these first few away
            }
            else if (_baselineDataCounters[index] < _baselineDataLength + _baselineThrowOut)
            {
                _baselineData[index].Add(value);
            }
            else if (_baselineDataCounters[index] == _baselineDataLength + _baselineThrowOut)
            {
                _baselineMean[index] = Mean(_baselineData[index]);
                _baselineStdev[index] = StandardDeviation(_baselineData[index], _baselineMean[index]);

                OnCalibrationChanged(CalibrationResults.Finished);
                _calibrationState = CalibrationState.Calibrated;
            }

            _baselineDataCounters[index]++;
        }
    }

    private void Train(double[] rawData)
    {
            double[] filtered = _bandPassFilters[0].filterData(rawData);

            for (int i = 0; i < rawData.Length; i++)
            {
                //Full wave rectification
                double value = Math.Abs(filtered[i]);


                if (_baselineDataCounters[0] < _baselineThrowOut)
                {
                    //throw these first few away
                }
                else if (_baselineDataCounters[0] < _baselineDataLength + _baselineThrowOut)
                {
                    _baselineData[0].Add(value);
                }
                else if (_baselineDataCounters[0] == _baselineDataLength + _baselineThrowOut)
                {
                    _baselineMean[0] = Mean(_baselineData[0]);
                    _baselineStdev[0] = StandardDeviation(_baselineData[0], _baselineMean[0]);

                    OnCalibrationChanged(CalibrationResults.Finished);
                    _calibrationState = CalibrationState.Calibrated;
                }
                else
                {
                    break;
                }

                _baselineDataCounters[0]++;
            }
        
    }

    private TrignoEmgSignal[] Process(double[] rawData, int sequenceNumber)
    {
        TrignoEmgSignal[] signals = new TrignoEmgSignal[1];

        TrignoEmgSignal signal = new TrignoEmgSignal(rawData,Channel, sequenceNumber);

        signal.BpfSample = _bandPassFilters[0].filterData(signal.RawSample);
        FullWaveRectification(signal.BpfSample, signal.FullWaveSample);
        signals[0] = MovingWindowAverageFilter(signal, 0);
       
        //Debug.WriteLine("Signals " + signals);

        return signals;
    }

//    private double[] FullWaveRectification(Double value)
//    {
//        return new double[] { Math.Abs(value) };
//    }

    private void FullWaveRectification(double[] values, double[] destination)
    {
        //TODO use vector approach
        for (int i = 0; i < values.Length; i++)
        {
            destination[i] = Math.Abs(values[i]);
        }
    }

    private TrignoEmgSignal MovingWindowAverageFilter(TrignoEmgSignal signal, int index)
    {
        bool activated = false;
        double onOff = 0;
        for (int i = 0; i < signal.FullWaveSample.Length; i++)
        {
            if (_movingWindowData[index].Count == _movingWindowLength)
            {
                _movingWindowData[index].RemoveAt(0);
                _movingWindowData[index].Add(signal.FullWaveSample[i]);
            }
            else
            {
                _movingWindowData[index].Add(signal.FullWaveSample[i]);
            }

            double currentMean = Mean(_movingWindowData[index]);

            signal.AveragedSample[i] = currentMean;

            double threshold = ActivationThreshold[index] <= 0
                ? (_baselineMean[index] + (3 * _baselineStdev[index]))
                : ActivationThreshold[index];
            
            if (currentMean > threshold)
            {
                onOff = currentMean;
            }
            else
            {
                onOff = 0;
            }

            signal.OnOff[i] = onOff;
            activated = activated || !(onOff == 0);
            signal.RestingMean[i] = _baselineMean[index];
            signal.RestingStdev[i] = _baselineStdev[index];
        }

        signal.MuscleActivated = activated;

        return signal;
    }

    private double Mean(List<double> data)
    {
        Double sum = 0;
        foreach (Double val in data)
        {
            sum += val;
        }

        return sum / data.Count;
    }

    private double StandardDeviation(List<double> data, double mean)
    {
        Double res = 0;
        foreach (Double value in data)
        {
            res += (value - mean) * (value - mean);
        }

        return Math.Sqrt(res / data.Count);
    }

    public event EventHandler<CalibrationChangedEventArgs>? CalibrationChanged;

    private void OnCalibrationChanged(CalibrationResults calibEvent)
    {
        if (CalibrationChanged != null)
        {
            CalibrationChangedEventArgs args = new CalibrationChangedEventArgs(calibEvent, _name);
            if (calibEvent == CalibrationResults.Finished)
            {
                args.CalibrationsData = _baselineData;
                args.ZeroMean = _baselineMean;
                args.ZeroStandardDeviation = _baselineStdev;
            }

            CalibrationChanged(this, args);
        }
    }

    public void Calibrate()
    {
        _baselineDataCounters = new int[2] { 0, 0 };
        _baselineData = new List<double>[2] { new List<double>(), new List<double>() };
        _baselineMean = new double[] { -1, -1 };
        _baselineStdev = new double[] { -1, -1 };
        _calibrationState = CalibrationState.Calibrating;
        OnCalibrationChanged(CalibrationResults.Started);
    }

    #endregion
}