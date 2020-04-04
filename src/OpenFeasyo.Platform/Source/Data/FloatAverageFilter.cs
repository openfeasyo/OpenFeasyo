using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Data
{
    public class FloatAverageFilter
    {
        private float[] values;
        private int pos = 0;
        private float sum = 0;

        public FloatAverageFilter(int numOfSamples)
        {
            values = new float[numOfSamples];
            for (int i = 0; i < numOfSamples; i++)
            {
                values[i] = 0;
            }
        }

        public FloatAverageFilter(int numOfSamples, float initValue)
        {
            values = new float[numOfSamples];
            for (int i = 0; i < numOfSamples; i++)
            {
                values[i] = initValue;
            }
            sum = numOfSamples * initValue;
        }

        public void AddValue(float val)
        {
            sum -= values[pos];
            values[pos++] = val;
            sum += val;
            pos %= values.Length;
        }

        public float GetLastAverage()
        {
            return sum / values.Length;
        }
    }
}
