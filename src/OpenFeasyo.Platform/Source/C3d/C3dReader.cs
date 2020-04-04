//-----------------------------------------------------------------------------
// C3dReader.cs
//
// Reads in 3D position data from C3D file passed in "infile" 
// (files should be opened before calling).
//
//
// ETRO, Vrije Universiteit Brussel
// Copyright (C) 2015 Lubos Omelina. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

#if SKIP_Vector4
    using Microsoft.Xna.Framework;
#endif

namespace Vub.Etro.IO
{
    public class C3dReader
    {
        private string _c3dFile;
        private FileStream _fs;
        private BinaryReader _reader;
        private Dictionary<string, ParameterGroup> _nameToGroups;
        private Dictionary<int, ParameterGroup> _idToGroups;
        private HashSet<Parameter> _allParameters;


        private int _dataStart;
        private int _pointsNumber;
        private float _pointScale;
        private float _pointRate;

        private float _analogRate;
        private int _analogUsed;
        private float[] _analogScale;
        private float _analogGenScale;
        private Int16[] _analogZeroOffset;



        #region Properties

        private List<string> _pointsLabels;
        private Dictionary<string, int> _pointsLabelsToId;
        internal List<string> _analogLabels;
        internal Dictionary<string, int> _analogLabelsToId;


        public IList<string> Labels { get { return _pointsLabels.AsReadOnly(); } }
        public IList<string> AnalogLabels { get { return _analogLabels.AsReadOnly(); } }

        private int _currentFrame = 0;
        public int CurrentFrame { get { return _currentFrame; } }

        //private float[,] _analogData = null;
        //public float[,] AnalogData { get {return _analogData;} }

        private AnalogDataArray _analogData = null;
        public AnalogDataArray AnalogData { get { return _analogData; } }
        public int AnalogChannels { get { return (int)(_analogRate / _pointRate); } }

        public Vector4[] _points = null;
        public Vector4[] Points { get { return _points; } }

        private int _pointFrames = 0;
        public int FramesCount { get { return _pointFrames; } }

        public int NumberOfPointsInFrame
        {
            get { return _pointsNumber; }
            set { _pointsNumber = value; }
        }


        private C3dHeader _header = null;
        public C3dHeader Header { get { return _header; } }

        public bool IsInterger { get { return _pointScale >= 0; } }
        public bool IsFloat { get { return _pointScale < 0; } }

        private List<C3dEvent> _events = new List<C3dEvent>();
        public List<C3dEvent> Events { get { return _events; } }

        #endregion

        internal HashSet<Parameter> AllParameters
        {
            get { return _allParameters; }
        }

        public string GetGroupName(Parameter p)
        {

            if (_idToGroups.ContainsKey(-p.Id))
            {
                return _idToGroups[-p.Id].Name;
            }
            return "";
        }

        public C3dReader()
        {
            _nameToGroups = new Dictionary<string, ParameterGroup>();
            _idToGroups = new Dictionary<int, ParameterGroup>();
            _pointsLabels = new List<string>();
            _pointsLabelsToId = new Dictionary<string, int>();
            _allParameters = new HashSet<Parameter>();
            _analogLabels = new List<string>();
            _analogLabelsToId = new Dictionary<string, int>();
        }

        public bool Open(string c3dFile)
        {
            _c3dFile = c3dFile;
            try
            {
                _fs = new FileStream(_c3dFile, FileMode.Open, FileAccess.Read);
                _reader = new BinaryReader(_fs);
                _header = new C3dHeader();

                ReadHeader();
                ReadParameters();
                ParseRequiredParameters();

                ReadEvents();

                _reader.BaseStream.Seek(_dataStart, 0);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("C3dReader.Open(\"" + c3dFile + "\"): " + e.Message);
                return false;
            }
            return true;
        }

        private void ParseRequiredParameters()
        {
            string[] labels = GetParameter<string[]>("POINT:LABELS");
            for (int i = 0; i < labels.Length; i++)
            {
                string label = labels[i].TrimEnd(' ');
                _pointsLabelsToId.Add(label, i);
                _pointsLabels.Insert(i, label);
            }
            _dataStart = 512 * (GetParameter<Int16>("POINT:DATA_START") - 1);
            _pointsNumber = GetParameter<Int16>("POINT:USED");
            _pointFrames = GetParameter<Int16>("POINT:FRAMES");
            _pointScale = GetParameter<float>("POINT:SCALE");
            _pointRate = GetParameter<float>("POINT:RATE");

            _analogRate = GetParameter<float>("ANALOG:RATE");
            _analogUsed = GetParameter<Int16>("ANALOG:USED");
            _analogScale = GetParameter<float[]>("ANALOG:SCALE");
            _analogGenScale = GetParameter<float>("ANALOG:GEN_SCALE");
            _analogZeroOffset = GetParameter<Int16[]>("ANALOG:OFFSET");

            string[] analogLabels = GetParameter<string[]>("ANALOG:LABELS");
            for (int i = 0; i < analogLabels.Length; i++)
            {
                string label = analogLabels[i].TrimEnd(' ');
                _analogLabelsToId.Add(label, i);
                _analogLabels.Insert(i, label);
            }


        }



        private void ReadHeader()
        {
            byte[] data = new byte[512];
            int n = _reader.Read(data, 0, 512);
            Header.SetHeader(data);

#if DEBUG
            Console.WriteLine("\tFirstWord: {0:X}", Header.FirstWord);
            Console.WriteLine("\tNumber of 3D points: " + Header.NumberOfPoints);
            Console.WriteLine("\tAnalog channels: " + Header.AnalogMeasurementsPerFrame);

            Console.WriteLine("\tFirst sample number: " + Header.FirstSampleNumber);
            Console.WriteLine("\tLast sample number: " + Header.LastSampleNumber);
            Console.WriteLine("\tScale factor: " + Header.ScaleFactor);
            Console.WriteLine("\tData start: " + Header.DataStart);
            Console.WriteLine("\tAnalog samples per frame: " + Header.AnalogSamplingRate);
            Console.WriteLine("\tFrame rate: " + Header.FrameRate);
#endif
        }

        private void ReadParameters()
        {
            byte[] parameters = new byte[4];
            int nb = _reader.Read(parameters, 0, 4);
            // TODO we should not ignore first 4 bytes as it is now

            int nextItem;
            do
            {
                sbyte nameLen = ParameterModel.ReadNameLength(_reader);
                bool isLocked = nameLen < 0;
                sbyte id = ParameterModel.ReadGroupID(_reader);
                string name = ParameterModel.ReadName(_reader, Math.Abs(nameLen));
                nextItem = ParameterModel.ReadNextItemOffset(_reader);

                ParameterModel param = null;
                int parameterDataSize = 0;
                if (id > 0) //if id > 0 then it is parameter, otherwise it is group
                {
                    param = new Parameter(_reader);
                    parameterDataSize = (param as Parameter).C3DParameterSize;
                }
                else
                {
                    param = new ParameterGroup();
                }

                byte descLen = ParameterModel.ReadDescLength(_reader);
                string desc = ParameterModel.ReadName(_reader, descLen);

                // general assignment
                param.Name = name;
                param.Id = id;
                param.Description = desc;


                if (param is Parameter)
                {
                    _allParameters.Add(param as Parameter);
                }
                else
                {
                    _nameToGroups.Add(param.Name, param as ParameterGroup);
                    _idToGroups.Add(param.Id, param as ParameterGroup);
                }

                // if next item is more far read unknown data
                // I assume that there will be no more than 512 bytes of unknown data

                if (nextItem > (descLen + 3 + parameterDataSize)) // If the C3D file is correct this will never happen, but real life is far from perfect
                {
                    int toRead = nextItem - (descLen + 1);
                    _reader.Read(parameters, 0, toRead);
                }
                //else if (nextItem < (descLen + 3 + parameterDataSize)) {
                //    Console.Error.WriteLine("Error");
                //}
            } while (nextItem > 0);

            foreach (Parameter p in _allParameters)
            {
                if (_idToGroups.ContainsKey(-p.Id))
                {
                    _idToGroups[-p.Id].Parameters.Add(p);
                }
            }
        }

        public T GetParameter<T>(string path)
        {
            string[] elements = path.Split(':');
            if (elements.Length != 2)
            {
                throw new ApplicationException("Wrong path format (use GROUP:PARAMETER)");
            }

            if (!_nameToGroups.ContainsKey(elements[0]))
                throw new ApplicationException("Group " + elements[0] + " doesn't exist.");

            ParameterGroup grp = _nameToGroups[elements[0]];

            foreach (Parameter p in grp.Parameters)
            {
                if (p.Name == elements[1])
                    return p.GetData<T>();
            }
            throw new ApplicationException("Parameter " + elements[0] +":"+ elements[1] + " doesn't exist.");
        }



        public Vector4[] ReadFrame()
        {
            Vector4[] data;
            if (_fs == null)
            {
                throw new ApplicationException("The C3d file is not open. Open the file before reading.");
            }

            //if (_currentFrame >= _pointFrames)
            //{
            //    _reader.BaseStream.Seek(_dataStart, 0);
            //    _currentFrame = 0;
            //}

            data = _pointScale < 0 ? ReadFloatData() : ReadIntData();

            _currentFrame++;
            Console.WriteLine(_currentFrame);
            return data;
        }

        private Vector4[] ReadFloatData()
        {
            if (!IsFloat)
                throw new ApplicationException("Data stored in C3D file are in Inetger format. You are trying to read it as a Floating-point format.");

            _points = new Vector4[_pointsNumber];
            for (int i = 0; i < _pointsNumber; i++)
            {
                _points[i] = new Vector4(
                    /* float x = */ _reader.ReadSingle(),
                    /* float y = */ _reader.ReadSingle(),
                    /* float z = */ _reader.ReadSingle(),
                    /* float w = */ _reader.ReadSingle()
                );
                
            }


            // Read Analog data
            int samplesPerFrame = (int)(_analogRate / _pointRate);
            
            float [,] allData = new float[_analogUsed, samplesPerFrame];
            for (int rate = 0; rate < samplesPerFrame; rate++)
            {
                for (int variable = 0; variable < _analogUsed; variable++)
                {
                    float data = _reader.ReadSingle();
                    //real world value = (data value - zero offset) * channel scale * general scale
                    allData[variable, rate] =
                        (data - ((_analogZeroOffset != null && _analogZeroOffset.Length > 0) ? (float)_analogZeroOffset[variable] : 0.0f)) * _analogGenScale * (_analogScale != null && _analogScale.Length > 0 ? (float)_analogScale[variable] : 1.0f);
                }
            }
            _analogData = new AnalogDataArray(_analogLabels, _analogLabelsToId, allData);

            return _points;
        }

        private Vector4[] ReadIntData()
        {
            if (!IsInterger)
                throw new ApplicationException("Data stored in C3D file are in Floating-point format. You are trying to read it as a Integer format.");

            _points = new Vector4[_pointsNumber];
            for (int i = 0; i < _pointsNumber; i++)
            {
                _points[i] = new Vector4(
                    /* float x = */ _reader.ReadInt16() * _pointScale,
                    /* float y = */ _reader.ReadInt16() * _pointScale,
                    /* float z = */ _reader.ReadInt16() * _pointScale,
                    /* float w = */ _reader.ReadInt16() * _pointScale
                );
                
            }

            // reading of analog data
            int samplesPerFrame = (int)(_analogRate / _pointRate);
            float [,] allData = new float[_analogUsed, samplesPerFrame];
            for (int rate = 0; rate < samplesPerFrame; rate++)
            {
                for (int variable = 0; variable < _analogUsed; variable++)
                {
                    float data = _reader.ReadInt16();
                    // real world value = (data value - zero offset) * channel scale * general scale
                    allData[variable, rate] =
                        (data - ((_analogZeroOffset != null && _analogZeroOffset.Length > 0) ? (float)_analogZeroOffset[variable] : 0.0f)) * _analogGenScale * (_analogScale != null && _analogScale.Length > 0 ? (float)_analogScale[variable] : 1.0f);
                }
            }
            _analogData = new AnalogDataArray(_analogLabels, _analogLabelsToId, allData);

            return _points;
        }

        public Vector4 this[int key]
        {
            get
            {
                if (_points == null)
                {
                    throw new ApplicationException("You must open file and read freame first");
                }
                else if (key < 0 || key >= _points.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                return _points[key];
            }
        }

        public Vector4 this[string key]
        {
            get
            {
                if (_pointsLabels == null)
                {
                    throw new ApplicationException("You must open file and read freame first");
                }
                if (!_pointsLabelsToId.ContainsKey(key))
                {
                    throw new ApplicationException("Label " + key + " doesn't exist in the 3D point data section");
                }
                return _points[_pointsLabelsToId[key]];
            }
        }


        // TODO
        #region read Events

        public void ReadEventContexts()
        {
            Int16 contextsCount = GetParameter<Int16>("EVENT_CONTEXT:USED");
            string[] contexts = GetParameter<string[]>("EVENT_CONTEXT:LABELS");
            string[] desc = GetParameter<string[]>("EVENT_CONTEXT:DESCRIPTIONS");
            Int16[] icon_ids = GetParameter<Int16[]>("EVENT_CONTEXT:ICON_IDS");
            Int16[] colours = GetParameter<Int16[]>("EVENT_CONTEXT:COLOURS");
        }

        private void ReadEvents()
        {
            if (!_nameToGroups.ContainsKey("EVENT")) {
                return; // There are no events in the C3D file
            }

            Int16 contextLength = GetParameter<Int16>("EVENT:USED");
            string[] contexts = GetParameter<string[]>("EVENT:CONTEXTS");
            string[] labels = GetParameter<string[]>("EVENT:LABELS");
            string[] descriptions = GetParameter<string[]>("EVENT:DESCRIPTIONS");
            string[] subjects = GetParameter<string[]>("EVENT:SUBJECTS");
            float[,] times = GetParameter<float[,]>("EVENT:TIMES");
            Int16[] icon_ids = GetParameter<Int16[]>("EVENT:ICON_IDS");
            byte[] generic_flags = GetParameter<byte[]>("EVENT:GENERIC_FLAGS");

            for (int i = 0; i < labels.Length; i++)
            {
                float time = times[0, i] * 60 + times[1, i];
                int frame = Int32.Parse((time * _header.FrameRate).ToString());

                C3dEvent c3dEvent = new C3dEvent(labels[i], contexts[i], frame);
                c3dEvent.Description = descriptions[i];
                c3dEvent.Subject = subjects[i];
                c3dEvent.IconId = icon_ids[i];
                c3dEvent.GenericFlag = generic_flags[i];

                _events.Add(c3dEvent);
            }
        }

        #endregion read event

        public bool Close()
        {
            _reader.Close();
            _fs.Close();
            return true;
        }

    }

    public class AnalogDataArray
    {

        private List<string> _analogLabels { get; set; }
        private Dictionary<string, int> _analogLabelsToId { get; set; }
        private float[,] _analogData = null;
        public float[,] Data { get { return _analogData; } }
        public IList<string> Labels { get { return _analogLabels.AsReadOnly(); } }

        internal AnalogDataArray(List<string> analogLabels, Dictionary<string, int> analogLabelsToId, float[,] analogData)
        {
            _analogLabels = analogLabels;
            _analogLabelsToId = analogLabelsToId;
            _analogData = analogData;
        }

        public float this[int key, int channel]
        {
            get
            {
                if (_analogData == null)
                {
                    throw new ApplicationException("You must open file and read freame first");
                }
                else if (key < 0 || key >= _analogData.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                return _analogData[key, channel];
            }
        }

        public float this[int key]
        {
            get
            {
                if (_analogData == null)
                {
                    throw new ApplicationException("You must open file and read freame first");
                }
                else if (key < 0 || key >= _analogData.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                return _analogData[key, 0];
            }
        }

        public float this[string key, int channel]
        {
            get
            {
                if (_analogLabels == null)
                {
                    throw new ApplicationException("You must open file and read freame first");
                }
                if (!_analogLabelsToId.ContainsKey(key))
                {
                    throw new ApplicationException("Analog data label " + key + " doesn't exist in the 3D point data section");
                }
                return _analogData[_analogLabelsToId[key], channel];
            }
        }

        public float this[string key]
        {
            get
            {
                if (_analogLabels == null)
                {
                    throw new ApplicationException("You must open file and read freame first");
                }
                if (!_analogLabelsToId.ContainsKey(key))
                {
                    throw new ApplicationException("Analog data label " + key + " doesn't exist in the 3D point data section");
                }
                return _analogData[_analogLabelsToId[key], 0];
            }
        }
    }
}

