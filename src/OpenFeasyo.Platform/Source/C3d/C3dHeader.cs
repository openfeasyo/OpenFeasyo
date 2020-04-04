//-----------------------------------------------------------------------------
// C3dHeader.cs
//
// Class representing C3D file header and exposing information as properties
//
// ETRO, Vrije Universiteit Brussel
// Copyright (C) 2015 Lubos Omelina. All rights reserved.
//-----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vub.Etro.IO
{
    public class C3dHeader
    {
        private byte[] _data;

        internal C3dHeader(Int16 numberOfPoints, Int16 analogMeasurementsPerFrame = 0, Int16 analogSamplingRate = 0)
        {
            _data = new byte[512];

            FirstWord = 0x5002;
            FirstSampleNumber = 1;
            LastSampleNumber = 1;
            FrameRate = 30;

            AnalogSamplingRate = analogSamplingRate;
            AnalogMeasurementsPerFrame = analogMeasurementsPerFrame;
            NumberOfPoints = numberOfPoints;
            
            ScaleFactor = -1f;
            Support4CharEventLabels = true;

            
        }

        internal C3dHeader()
        {
            _data = new byte[512];
        }

        public Int16 FirstWord { get { return BitConverter.ToInt16(_data, 0); }                 set { Array.Copy(BitConverter.GetBytes(value), 0, _data,0, sizeof(Int16)); } }
        public byte FirstParameterBlock { get { return _data[0]; }                              set { _data[0] = value; } }
        public Int16 NumberOfPoints { get { return BitConverter.ToInt16(_data, 2); }            private set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 2, sizeof(Int16)); } }
        public Int16 AnalogMeasurementsPerFrame { get { return BitConverter.ToInt16(_data, 4); } private set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 4, sizeof(Int16)); } }

        public Int16 FirstSampleNumber { get { return BitConverter.ToInt16(_data, 6); }         set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 6, sizeof(Int16)); }  }
        public Int16 LastSampleNumber     { get { return BitConverter.ToInt16(_data, 8); }      set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 8, sizeof(Int16)); } }
        public Int16 MaxInterpolationGaps { get { return BitConverter.ToInt16(_data, 10); }     set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 10, sizeof(Int16)); } }
        public float ScaleFactor { get { return BitConverter.ToSingle(_data, 12); }             set { Array.Copy(BitConverter.GetBytes(value), 0, _data,12, sizeof(float)); } }
        public Int16 DataStart { get { return BitConverter.ToInt16(_data, 16); }                set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 16, sizeof(Int16)); } }
        public Int16 AnalogSamplingRate { get { return BitConverter.ToInt16(_data, 18); }       private set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 18, sizeof(Int16)); } }
        public float FrameRate { get { return BitConverter.ToSingle(_data, 20); }               set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 20, sizeof(float)); } }

        public bool Support4CharEventLabels { get { return BitConverter.ToInt16(_data, 149*2) == 12345; } set { Array.Copy(BitConverter.GetBytes(value == true? 12345:0), 0, _data, 149*2, sizeof(Int16)); } }

        internal void SetHeader(byte[] headerData)
        {
            Array.Copy(headerData, _data, 512);
        }

        internal byte [] GetRawData() 
        {
            return _data;
        }

    }
}
