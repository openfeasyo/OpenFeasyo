namespace TrignoAvantiCustom;

class BandPassFilter
{
    public const int LOW_PASS = 0;
    public const int HIGH_PASS = 1;
    public const int BAND_PASS = 2;
    public const int BAND_STOP = 3;

    // filter parameters    
    private double samplingRate = Double.NaN;
    private double[] cornerFrequency;
    private int nTaps;

    // buffered data (for filtering streamed data)
    private List<Double> bufferedX = new List<Double>();
    private List<Double> bufferedY = new List<Double>();

    // filter coefficients {a,b}
    private double[] coefficients;

    // input parameters are invalid
    private Boolean validparameters = false;

    // default parameters
    private double defaultSamplingRate = 512;
    private double[] defaultCornerFrequency = { 0.5 };
    private int defaultNTaps = 200;

    MathNet.Filtering.OnlineFilter fastFilter = null;

    public BandPassFilter()
    {
        SetFilterParameters(BandPassFilter.LOW_PASS, defaultSamplingRate, defaultCornerFrequency, defaultNTaps);
        System.Console.WriteLine("Using default filter type. Default is low pass.");
        System.Console.WriteLine("Using default sampling rate. Default is 512Hz.");
        System.Console.WriteLine("Using default corner frequency. Default is 0.5Hz.");
        System.Console.WriteLine("Using default number of taps. Default is 200.");
    }

    public BandPassFilter(int filterType)
    {
        SetFilterParameters(filterType, defaultSamplingRate, defaultCornerFrequency, defaultNTaps);
        System.Console.WriteLine("Using default sampling rate. Default is 512Hz.");
        System.Console.WriteLine("Using default corner frequency. Default is 0.5Hz.");
        System.Console.WriteLine("Using default number of taps. Default is 200.");
    }

    public BandPassFilter(int filterType, double samplingRate)
    {
        SetFilterParameters(filterType, samplingRate, defaultCornerFrequency, defaultNTaps);
        System.Console.WriteLine("Using default corner frequency. Default is 0.5Hz.");
        System.Console.WriteLine("Using default number of taps. Default is 200.");
    }

    public BandPassFilter(int filterType, double samplingRate, double[] cornerFrequency)
    {
        SetFilterParameters(filterType, samplingRate, cornerFrequency, defaultNTaps);
        System.Console.WriteLine("Using default number of taps. Default is 200.");
    }

    public BandPassFilter(int filterType, double samplingRate, double[] cornerFrequency, int nTaps)
    {
        SetFilterParameters(filterType, samplingRate, cornerFrequency, nTaps);
    }

    public void resetFilterBuffer()
    {
        this.bufferedX.Clear();
        this.bufferedY.Clear();
    }

    public void SetFilterParameters(int LoHi, double samplingRate, double[] cornerFrequency, int nTaps)
    {
        this.resetFilterBuffer();

        if (nTaps % 2 != 0)
        {
            if (nTaps == 1)
            {
                nTaps++;
            }
            else
            {
                nTaps--;
            }

            System.Console.WriteLine("Warning. nTaps is not an even number. nTaps will be rounded to " + nTaps);
        }

        if (LoHi == LOW_PASS || LoHi == HIGH_PASS)
        {
            this.samplingRate = samplingRate;
            this.cornerFrequency = cornerFrequency;
            this.nTaps = nTaps;

            double fc = (cornerFrequency[0] / samplingRate);

            coefficients = new double[nTaps];
            coefficients = calculateCoefficients(fc, LoHi, nTaps);
            this.validparameters = true;
        }
        else if (LoHi == BAND_PASS || LoHi == BAND_STOP)
        {
            if (cornerFrequency.Length != 2)
            {
                throw new System.ArgumentException(
                    "Error. Bandpass or bandstop filter requires two corner frequencies to be specified");
            }

            this.samplingRate = samplingRate;
            this.nTaps = nTaps;

            // filter parameters: corners at +/- 1Hz from notch
            // center frequency
            double fcHigh;
            double fcLow;

            fcHigh = cornerFrequency.Max() / samplingRate;
            fcLow = cornerFrequency.Min() / samplingRate;

            // calculate filter coefficients
            double[] coefficientHighPass = calculateCoefficients(fcHigh, HIGH_PASS, nTaps);
            double[] coefficientLowPass = calculateCoefficients(fcLow, LOW_PASS, nTaps);
            coefficients = new double[coefficientHighPass.Length];

            for (int i = 0; i < coefficientHighPass.Length; i++)
            {
                if (LoHi == BAND_PASS)
                {
                    coefficients[i] =
                        -(coefficientHighPass[i] +
                          coefficientLowPass
                              [i]); //sum of HPF and LPF for bandstop filter, spectral inversion for bandpass filter
                }
                else
                {
                    coefficients[i] =
                        coefficientHighPass[i] + coefficientLowPass[i]; //sum of HPF and LPF for bandstop filter
                }
            }

            if (LoHi == BAND_PASS)
            {
                coefficients[(nTaps / 2) + 1] = coefficients[(nTaps / 2) + 1] + 1;
            }

            this.validparameters = true;
            fastFilter = new MathNet.Filtering.FIR.OnlineFirFilter(coefficients);
        }
        else
        {
            throw new System.ArgumentException(
                "Error. Undefined filter type: use 0 - lowpass, 1 - highpass, 2- bandpass, or 3- bandstop",
                LoHi.ToString());
        }
    }

    public double[] filterData(double[] data)
    {
        return fastFilter.ProcessSamples(data);
    }

    public double filterData(double data)
    {
        return fastFilter.ProcessSample(data);
    }
    private double[] calculateCoefficients(double fc, int LoHi, int nTaps)
    {
        if (!(LoHi == LOW_PASS || LoHi == HIGH_PASS))
        {
            throw new System.ArgumentException(
                "Error. The function calculateCoefficients() can only be called for LPF or HPF.");
        }

        //initialisation

        int M = nTaps;
        double[] h = new double[M];
        for (int i = 0; i < M; i++)
        {
            h[i] = 0;
        }

        for (int i = 0; i < M; i++)
        {
            h[i] = 0.42 - 0.5 * Math.Cos((2 * Math.PI * i) / M) + 0.08 * Math.Cos((4 * Math.PI * i) / M);
            if (i != M / 2)
            {
                h[i] = h[i] * (Math.Sin(2 * Math.PI * fc * (i - (M / 2)))) / (i - (M / 2));
            }
            else
            {
                h[i] = h[i] * (2 * Math.PI * fc);
            }
        }

        double gain = 0;
        for (int i = 0; i < h.Length; i++)
        {
            gain += h[i];
        }

        for (int i = 0; i < h.Length; i++)
        {
            if (LoHi == HIGH_PASS)
            {
                h[i] = -h[i] / gain;
            }
            else
            {
                h[i] = h[i] / gain;
            }
        }

        if (LoHi == HIGH_PASS)
        {
            h[M / 2] = h[M / 2] + 1;
        }

        return h;
    }

    public int GetNTaps()
    {
        return nTaps;
    }

    protected void SetNTaps(int numberOfTaps)
    {
        nTaps = numberOfTaps;
    }

    public double GetSamplingRate()
    {
        return samplingRate;
    }

    protected void SamplingRate(double samplingrate)
    {
        samplingRate = samplingrate;
    }

    public double[] GetCornerFrequency()
    {
        return cornerFrequency;
    }

    protected void SetCornerFrequency(double[] cf)
    {
        cornerFrequency = cf;
    }
}