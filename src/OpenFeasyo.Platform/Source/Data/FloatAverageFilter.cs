/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */

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
