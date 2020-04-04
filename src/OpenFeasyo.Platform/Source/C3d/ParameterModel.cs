//-----------------------------------------------------------------------------
// ParameterModel.cs
//
// The class representation of c3d parameters and parameter groups
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

namespace Vub.Etro.IO
{

#if !SKIP_VECTOR4
    #region 3D point representation


    public struct Vector4
    {
        public Vector4(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public float X, Y, Z, W;
    }

    #endregion
#endif

    #region ParameterBase

    public abstract class ParameterModel
    {
        public const int BLOCK_SIZE = 512;

        public ParameterModel()
        {
            Name = "";
            Description = "";
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public sbyte Id { get; set; }

        private long _offsetInFile = -1;
        internal long OffsetInFile
        {
            get { return _offsetInFile; }
            set
            {
                if (_offsetInFile == -1)
                    _offsetInFile = value;
                else
                    throw new ApplicationException("FileOffset has been set already for parameter " + Name);
            }
        }
        internal void ResetOffsetInFile() { _offsetInFile = -1; }

        protected abstract Int16 GetContentLength();
        protected abstract void WriteContent(BinaryWriter writer);

        public void WriteTo(BinaryWriter writer, bool isLast = false)
        {

            writer.Write((sbyte)Name.Length);
            writer.Write((sbyte)Id);
            writer.Write(Name.ToCharArray());//  string name = ParameterModel.ReadName(_reader, Math.Abs(nameLen));

            // compute offset of the next item
            Int16 nextItem = (Int16)(isLast ?
                0 : (Description.Length
                                            + 2 // next item number 
                                            + 1 // desc length number
                                            + GetContentLength()));


            writer.Write(nextItem);
            WriteContent(writer);

            writer.Write((byte)(Description.Length));
            writer.Write(Description.ToCharArray());
        }

        //
        // static member for C3D format reading
        //

        public static sbyte ReadGroupID(BinaryReader reader)
        {
            return reader.ReadSByte();
        }

        public static sbyte ReadNameLength(BinaryReader reader)
        {
            return reader.ReadSByte();
        }

        public static byte ReadDescLength(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        public static Int16 ReadNextItemOffset(BinaryReader reader)
        {
            return reader.ReadInt16();
        }

        public static string ReadName(BinaryReader reader, int length)
        {
            return ParameterModel.ReadString(reader, length);
        }

        public static string ReadDesc(BinaryReader reader, int length)
        {
            return ParameterModel.ReadString(reader, length);
        }

        private static string ReadString(BinaryReader reader, int length)
        {
            return System.Text.Encoding.UTF8.GetString(reader.ReadBytes(length), 0, length);
        }


    }

    #endregion ParameterBase

    #region ParameterGroup

    public class ParameterGroup : ParameterModel
    {
        public ParameterGroup()
        {
            _parameters = new HashSet<Parameter>();
        }

        private HashSet<Parameter> _parameters;
        public HashSet<Parameter> Parameters
        {
            get { return _parameters; }
        }

        public bool HasParameter(string name)
        {
            foreach (Parameter p in _parameters)
            {
                if (p.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public Parameter GetParameter(string name)
        {
            foreach (Parameter p in _parameters)
            {
                if (p.Name == name)
                {
                    return p;
                }
            }
            return null;
        }

        internal void ResetOffsetInFile()
        {
            foreach (Parameter p in _parameters)
            {
                p.ResetOffsetInFile();
            }
        }

        protected override Int16 GetContentLength() { return 0; } // ParameterGroup doesn't have content (NOTE: Parameters are children, not content)

        protected override void WriteContent(BinaryWriter writer) { }
    }

    #endregion ParameterGroup

    #region Parameter

    public class Parameter : ParameterModel
    {
        private int[] _dimensions = null;
        private byte[] _vectorData;
        private sbyte _paramType;
        private int _length;

        public bool IsScalar { get; set; }

        private int _C3DParameterSize = 0;
        public int C3DParameterSize
        {
            get
            {
                return _C3DParameterSize;
            }
        }

        public Type ParamType
        {
            get
            {
                return GetType(this._paramType);
            }
        }

        public int[] Dimensions
        {
            get { return _dimensions; }
        }

        public Parameter(BinaryReader reader)
        {
            long position = reader.BaseStream.Position;
            // read parameter
            _paramType = reader.ReadSByte();
            byte dimensions = reader.ReadByte(); // 0 means that parameter is scalar

            if (dimensions > 0)
            {
                IsScalar = false;
                ReadMatrix(reader, GetType(_paramType), dimensions);
            }
            else
            {
                IsScalar = true;
                ReadScalar(reader, GetType(_paramType));
            }

            _C3DParameterSize = (int)(reader.BaseStream.Position - position);
        }

        public Parameter()
        {

        }

        internal Parameter Clone() {
            Parameter p = new Parameter();
            p.Id = this.Id;
            p.Name = this.Name;
            p.Description = this.Description;
            p._dimensions = (int[])this._dimensions.Clone();
            p._vectorData = (byte[])this._vectorData.Clone();
            p._paramType = this._paramType;
            p._length = this._length;
            p.IsScalar = this.IsScalar;
            p._C3DParameterSize = this._C3DParameterSize;
            return p;
        }

        private void ReadMatrix(BinaryReader reader, Type t, int dimensions)
        {
            _length = 1;
            _dimensions = new int[dimensions];
            for (int i = 0; i < dimensions; i++)
            {
                _dimensions[i] = reader.ReadByte();
                _length *= _dimensions[i];
            }
            _length *= GetSize(_paramType);

            // TODO
            _vectorData = new byte[_length];
            reader.Read(_vectorData, 0, _length);
        }

        private void ReadScalar(BinaryReader reader, Type t)
        {
            _vectorData = new byte[GetSize(_paramType)];
            _dimensions = new int[] { };
            _length = 0;
            reader.Read(_vectorData, 0, GetSize(_paramType));
            // TODO
        }


        #region Serialization of data

        //public void SetData<T>(T data) 
        //{ 
        //    SetData<T>(data);k
        //}

        protected override Int16 GetContentLength()
        {
            return (Int16)(
                _vectorData.Length +
                1 + // to store parameter type (sbyte)
                1 + // to store number of dimensions type (byte)
                _dimensions.Length// +
                //_vectorData.Length
                );
        }

        protected override void WriteContent(BinaryWriter writer)
        {
            writer.Write(_paramType);
            writer.Write((byte)_dimensions.Length);
            for (int i = 0; i < _dimensions.Length; i++)
            {
                writer.Write((byte)_dimensions[i]);
            }
            writer.Write(_vectorData);
        }

        public void SetData<T>(T data)
        {
            //T ret;
            // = default(T);
            //
            //  BASIC TYPES
            //
            if (typeof(T) == typeof(char))
            {
                _paramType = -1;
                _dimensions = new int[] { };
                _vectorData = BitConverter.GetBytes((char)(object)data);
                _length = 1;
                IsScalar = true;
            }
            else if (typeof(T) == typeof(byte))
            {
                _dimensions = new int[] { };
                _paramType = 1;
                _vectorData = BitConverter.GetBytes((byte)(object)data);
                _length = 1;
                IsScalar = true;
            }
            else if (typeof(T) == typeof(Int16))
            {
                _dimensions = new int[] { };
                _paramType = 2;
                _vectorData = BitConverter.GetBytes((Int16)(object)data);
                _length = 1;
                IsScalar = true;
            }
            else if (typeof(T) == typeof(float))
            {
                _dimensions = new int[] { };
                _paramType = 4;
                _vectorData = BitConverter.GetBytes((float)(object)data);
                _length = 1;
                IsScalar = true;
            }
            else if (typeof(T) == typeof(string))
            {
                _dimensions = new int[] { ((string)(object)data).Length };
                _paramType = -1;
                _vectorData = Encoding.ASCII.GetBytes((string)(object)data);
                _length = _vectorData.Length; // it is the same length as it is in string because ASCII encoding
                IsScalar = false;
            }

            //
            // 1D Arrays
            //
            else if (typeof(T) == typeof(string[]))
            {
                int count = ((string[])(object)data).Length;
                int maxLen = 0;
                foreach (string s in ((string[])(object)data))
                {
                    maxLen = Math.Max(s.Length, maxLen);
                }
                _dimensions = new int[] { maxLen, count };
                _paramType = -1;
                _vectorData = new byte[count * maxLen];


                // in C# there is really no other method for initialising arrays to a non-default value (without creating temporary objects)
                // this is fastest way, see this -> http://www.dotnetperls.com/initialize-array
                // but yes, it's ugly, indeed
                for (int i = 0; i < _vectorData.Length; i++) _vectorData[i] = 32;



                _length = _vectorData.Length; // it is the same length as it is in string because ASCII encoding
                for (int i = 0; i < count; i++)
                {
                    string s = ((string[])(object)data)[i];
                    Encoding.ASCII.GetBytes(s, 0, s.Length, _vectorData, i * maxLen);
                }
                IsScalar = false;
            }
            else if (typeof(T) == typeof(float[]))
            {
                int count = ((float[])(object)data).Length;
                _dimensions = new int[] { count };
                _paramType = 4;
                _vectorData = new byte[count * GetSize(4)];

                _length = _vectorData.Length; // it is the same length as it is in string because ASCII encoding
                for (int i = 0; i < count; i++)
                {
                    float f = ((float[])(object)data)[i];
                    Array.Copy(BitConverter.GetBytes(f), 0, _vectorData, i * GetSize(_paramType), GetSize(_paramType));
                }
                IsScalar = false;
            }
            else if (typeof(T) == typeof(float[,]))
            {
                int dim1 = ((float[,])(object)data).GetLength(0);
                int dim2 = ((float[,])(object)data).GetLength(1);
                int count = ((float[,])(object)data).Length;
                _dimensions = new int[] { dim1, dim2 };
                _paramType = 4;
                _vectorData = new byte[count * GetSize(4)];

                _length = _vectorData.Length; // it is the same length as it is in string because ASCII encoding
                int offset = 0;
                for (int y = 0; y < dim2; y++)
                    for (int x = 0; x < dim1; x++)
                    {
                        float f = ((float[,])(object)data)[x, y];
                        Array.Copy(BitConverter.GetBytes(f), 0, _vectorData, offset * GetSize(_paramType), GetSize(_paramType));
                        offset++;
                    }
                IsScalar = false;
            }
            else if (typeof(T) == typeof(Int16[]))
            {
                int count = ((Int16[])(object)data).Length;
                _dimensions = new int[] { count };
                _paramType = 2;
                _vectorData = new byte[count * GetSize(2)];
                _length = _vectorData.Length; // it is the same length as it is in string because ASCII encoding
                for (int i = 0; i < count; i++)
                {
                    Int16 n = ((Int16[])(object)data)[i];

                    // TODO : check this
                    Array.Copy(BitConverter.GetBytes(n), 0, _vectorData, i * GetSize(_paramType), GetSize(_paramType));
                }
                IsScalar = false;
            }
            //else if (typeof(T) == typeof(char[]))
            //{
            //    ret = (T)(object)Get1DArray<char>();
            //}
            else if (typeof(T) == typeof(byte[]))
            {
                int count = ((byte[])(object)data).Length;
                _dimensions = new int[] { count };
                _paramType = 1;
                _vectorData = new byte[count * GetSize(1)];
                _length = _vectorData.Length; // it is the same length as it is in string because ASCII encoding
                for (int i = 0; i < count; i++)
                {
                    byte n = ((byte[])(object)data)[i];

                    // TODO : check this
                    Array.Copy(BitConverter.GetBytes(n), 0, _vectorData, i * GetSize(_paramType), GetSize(_paramType));
                }
                IsScalar = false;
            }
            //else if (typeof(T) == typeof(Int16[]))
            //{
            //    ret = (T)(object)Get1DArray<Int16>();
            //}
            //else if (typeof(T) == typeof(float[]))
            //{
            //    ret = (T)(object)Get1DArray<float>();
            //}
            ////
            //// 2D Arrays
            ////
            //// TODO: DO IT IF YOU NEED IT :)
            ////
            ////    else if (typeof(T) == typeof(Int16 [,]))
            ////    {
            ////        ret = (T)(object)Get2DArray<Int16>();
            ////    }

            //else
            //{
            //    throw new ApplicationException("Unknown type of parameter");
            //}
            //return ret;
        }

        #endregion

        #region Extraction of data

        public T GetData<T>()
        {
            return GetData<T>(0);
        }

        public T GetData<T>(int i)
        {
            T ret;// = default(T);

            //  BASIC TYPES
            if (typeof(T) == typeof(char))
            {
                ret = (T)(object)BitConverter.ToChar(_vectorData, i);
            }
            else if (typeof(T) == typeof(byte))
            {
                ret = (T)(object)_vectorData[i];
            }
            else if (typeof(T) == typeof(Int16))
            {
                ret = (T)(object)BitConverter.ToInt16(_vectorData, i * sizeof(Int16));
            }
            else if (typeof(T) == typeof(float))
            {
                ret = (T)(object)BitConverter.ToSingle(_vectorData, i * sizeof(float));
            }
            else if (typeof(T) == typeof(string))
            {
                ret = (T)(object)DataToString();
            }

            // 1D Arrays
            else if (typeof(T) == typeof(string[]))
            {
                ret = (T)(object)DataToStringArray();
            }
            else if (typeof(T) == typeof(char[]))
            {
                ret = (T)(object)Get1DArray<char>();
            }
            else if (typeof(T) == typeof(byte[]))
            {
                ret = (T)(object)Get1DArray<byte>();
            }
            else if (typeof(T) == typeof(Int16[]))
            {
                ret = (T)(object)Get1DArray<Int16>();
            }
            else if (typeof(T) == typeof(float[]))
            {
                ret = (T)(object)Get1DArray<float>();
            }
            //
            // 2D Arrays
            //
            else if (typeof(T) == typeof(float[,]))
            {
                ret = (T)(object)Get2DArray<float>();
            }
            // TODO: DO IT IF YOU NEED IT :)
            //
            //    else if (typeof(T) == typeof(Int16 [,]))
            //    {
            //        ret = (T)(object)Get2DArray<Int16>();
            //    }

            else
            {
                throw new ApplicationException("Unknown type of parameter");
            }
            return ret;
        }

        private T[,] Get2DArray<T>()
        {
            if (_dimensions.Length != 2)
                throw new ApplicationException("Parameter " + Name + " is not 2D array.");
            T[,] array = new T[_dimensions[0], _dimensions[1]];
            for (int x = 0; x < _dimensions[0]; x++)
            {
                for (int y = 0; y < _dimensions[1]; y++)
                {
                    // TODO: still need to test following line
                    array[x, y] = GetData<T>(x + y * x);
                }
            }
            return array;
        }


        private T[] Get1DArray<T>()
        {
            if (_dimensions.Length != 1)
                throw new ApplicationException("Parameter " + Name + " is not 1D array.");
            T[] array = new T[_dimensions[0]];
            for (int i = 0; i < _dimensions[0]; i++)
            {
                array[i] = GetData<T>(i);
            }
            return array;
        }


        private string DataToString()
        {
            if (_dimensions.Length != 1 || _paramType != -1)
                throw new ApplicationException("Parameter " + Name + " is not string type.");
            return Encoding.UTF8.GetString(_vectorData, 0, _dimensions[0]);
        }

        private string[] DataToStringArray()
        {
            string[] retArray;
            if (_dimensions.Length != 2 || _paramType != -1)
                throw new ApplicationException("Parameter " + Name + " is not string array type.");

            retArray = new string[_dimensions[1]];

            for (int i = 0; i < _dimensions[1]; i++)
            {
                retArray[i] = Encoding.UTF8.GetString(_vectorData, i * _dimensions[0], _dimensions[0]);
            }
            return retArray;
        }

        #endregion

        public int Length { get { return _length; } }

        private static Type GetType(sbyte c3dDataType)
        {
            switch (c3dDataType)
            {
                case -1:
                    return typeof(char);
                case 1:
                    return typeof(byte);
                case 2:
                    return typeof(Int16);
                case 4:
                    return typeof(float);
                default:
                    throw new ApplicationException("Unknown data type of c3d parameter");
            }
        }

        private static int GetSize(sbyte c3dDataType)
        {
            switch (c3dDataType)
            {
                case -1: return 1;
                case 1:  return 1;
                case 2:  return 2;
                case 4:  return 4;
                default: throw new ApplicationException("Unknown data type of c3d parameter");
            }
        }

        internal void CopyDataFrom(Parameter p) {
            if (_vectorData.Length != p._vectorData.Length) {
                throw new ApplicationException("Unable to copy data from parameters of different length");
            }
            p._vectorData.CopyTo(this._vectorData,0);
            
        }
    }
    #endregion Parameter
}
