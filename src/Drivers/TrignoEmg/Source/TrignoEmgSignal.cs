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
using OpenFeasyo.Platform.Controls;

namespace TrignoEmg
{
    public class TrignoEmgSignal : IEmgSignal
    {
        public TrignoEmgSignal(double[] rawSample) {
            RawSample = rawSample;
            FullWaveSample = new double[rawSample.Length];
            AveragedSample = new double[rawSample.Length];
            OnOff = new double[rawSample.Length];
            RestingMean = new double[rawSample.Length];
            RestingStdev = new double[rawSample.Length]; 
        }
       

        public bool MuscleActivated { get; set; }

        public double[] RawSample { get; set; }

        public double[] BpfSample { get; set; }

        public double[] AveragedSample { get; set; }

        public double[] FullWaveSample { get; set; }

        public double[] OnOff { get; set; }

        public double[] RestingMean { get; set; }

        public double[] RestingStdev { get; set; }
    }
}
