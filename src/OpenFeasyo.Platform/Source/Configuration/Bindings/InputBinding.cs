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
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Analysis;


namespace OpenFeasyo.Platform.Configuration.Bindings
{
    public abstract class InputBinding
    {
        internal abstract IGamingInput Input
        {
            get;
        }

        private float _zeroAngle;
        public float ZeroAngle
        {
            get
            {
                return _zeroAngle;
            }

            set
            {
                _zeroAngle = value;
            }
        }

        private float _sensitivity;
        public float Sensitivity
        {
            get
            {
                return _sensitivity;
            }

            set
            {
                _sensitivity = value;
            }
        }

        private Configuration.InputValueHandle handle;
        internal Configuration.InputValueHandle Handle
        {
            get
            {
                return handle;
            }
        }

        protected InputBinding(
            Configuration.InputValueHandle handle,
            float zeroAngle,
            float sensitivity)
        {
            this.handle = handle;
            this._zeroAngle = zeroAngle;
            this._sensitivity = sensitivity;
        }

        protected void CallHandle(int source, float value)
        {
            handle(source, adjustAngle(value));
        }

        protected void CallPositionalHandle(int source, float value)
        {
            handle(source, (value - ZeroAngle)*Sensitivity);
        }

        private float adjustAngle(float angle)
        {
            return (angle - MathHelper.ToRadians(ZeroAngle)) * Sensitivity;
        }

        abstract internal void destroy();
    }

    public class AccelerometerBinding : AnalysableBinding
    {
        public enum BindingType
        {
            XAxis,
            YAxis,
            ZAxis
        }

        private BindingType _type;
        public BindingType Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value != _type)
                {
                    _input.AccelerometerChanged -= _event_handler;
                    _type = value;
                    _event_handler = GetHandler(_type);
                    _input.AccelerometerChanged += _event_handler;
                }
            }
        }

        private IAccelerometerInput _input;

        internal override IGamingInput Input
        {
            get { return _input; }
        }

        private EventHandler<AccelerometerChangedEventArgs> _event_handler;
        private EventHandler<AccelerometerChangedEventArgs> _analyzer_handler;

        internal AccelerometerBinding(
            IAccelerometerInput input,
            Configuration.InputValueHandle handle,
            BindingType property,
            float zeroAngle,
            float sensitivity)
            : base(handle, zeroAngle, sensitivity)
        {
            this._input = input;
            this._type = property;

            _event_handler = GetHandler(property);
            _input.AccelerometerChanged += _event_handler;

            _analyzer_handler = new EventHandler<AccelerometerChangedEventArgs>(_analyzer_handle);
            input.AccelerometerChanged += _analyzer_handler;
        }

        private EventHandler<AccelerometerChangedEventArgs> GetHandler(BindingType t)
        {
            switch (t)
            {
                case BindingType.XAxis:
                    return new EventHandler<AccelerometerChangedEventArgs>(_input_assign_X);
                case BindingType.YAxis:
                    return new EventHandler<AccelerometerChangedEventArgs>(_input_assign_Y);
                case BindingType.ZAxis:
                    return new EventHandler<AccelerometerChangedEventArgs>(_input_assign_Z);
                default:
                    throw new ApplicationException("Unknown accelerometer binding type!");
            }
        }

        void _input_assign_X(object sender, AccelerometerChangedEventArgs e)
        {
            CallHandle((int)AccelerometerBinding.BindingType.XAxis, e.AccelerometerData.AngleX);
        }

        void _input_assign_Y(object sender, AccelerometerChangedEventArgs e)
        {
            CallHandle((int)AccelerometerBinding.BindingType.YAxis, e.AccelerometerData.AngleY);
        }

        void _input_assign_Z(object sender, AccelerometerChangedEventArgs e)
        {
            CallHandle((int)AccelerometerBinding.BindingType.ZAxis, e.AccelerometerData.AngleZ);
        }

        internal override void destroy()
        {
            _input.AccelerometerChanged -= _event_handler;
            _input.AccelerometerChanged -= _analyzer_handler;
            foreach (AnalyzerWrapper executor in _analyzerWrapers)
            {
                executor.Stop();
            }
        }



        private HashSet<AnalyzerWrapper> _analyzerWrapers = new HashSet<AnalyzerWrapper>();

        public void AddAnalyzer(IAccelerometerAnalyzer analyzer, ObservableDictionary<string, string> parameters)
        {
            AnalyzerWrapper executor = new AnalyzerWrapper(analyzer, parameters);
            executor.Run();
            _analyzerWrapers.Add(executor);
            _analyzers.Add(analyzer, parameters);
        }

        void _analyzer_handle(object sender, AccelerometerChangedEventArgs e)
        {
            foreach (AnalyzerWrapper analyzer in _analyzerWrapers)
            {
                analyzer.ProcessAccelerometer(e.AccelerometerData);
            }
        }

    }

    public abstract class AnalysableBinding : InputBinding
    {
        internal protected AnalysableBinding(
            Configuration.InputValueHandle handle,
            float zeroAngle,
            float sensitivity)
            : base(handle, zeroAngle, sensitivity) { }

        protected ObservableDictionary<IAnalyzer, ObservableDictionary<string, string>> _analyzers =
            new ObservableDictionary<IAnalyzer, ObservableDictionary<string, string>>();

        internal ObservableDictionary<IAnalyzer, ObservableDictionary<string, string>> Analyzers { get { return _analyzers; } }

    }

    public class SkeletonBinding : AnalysableBinding
    {
        public enum BindingType
        {
            SingleBoneAngle,
            TwoBonesAngle
        }

        private BindingType _type;
        internal BindingType Type
        {
            get
            {
                return _type;
            }
        }

        private ISkeletonInput _input;
        internal override IGamingInput Input
        {
            get { return _input; }
        }

        private BonePartsValue _firstBoneMarkers;

        private BoneMarkers _bindedBoneFirst;
        public BoneMarkers FirstBone
        {
            get
            {
                return _bindedBoneFirst;
            }
            set
            {
                _bindedBoneFirst = value;
                _firstBoneMarkers = BonePartsValue.GetMarkersFrom(_bindedBoneFirst);
            }
        }


        private BonePartsValue _secondBoneMarkers;

        private BoneMarkers _bindedBoneSecond;
        public BoneMarkers SecondBone
        {
            get
            {
                return _bindedBoneSecond;
            }
            set
            {
                _bindedBoneSecond = value;
                _secondBoneMarkers = BonePartsValue.GetMarkersFrom(_bindedBoneSecond);
            }
        }

        private EventHandler<SkeletonChangedEventArgs> _event_handler;
        private EventHandler<SkeletonChangedEventArgs> _analyzer_handler;

        public SkeletonBinding(
                ISkeletonInput input,
                Configuration.InputValueHandle handle,
                BoneMarkers bone,
                float zeroAngle,
                float sensitivity)
            : base(handle, zeroAngle, sensitivity)
        {
            this._input = input;
            FirstBone = bone;
            _type = BindingType.SingleBoneAngle;
            _event_handler = new EventHandler<SkeletonChangedEventArgs>(_input_assign_single_bone_angle);
            _analyzer_handler = new EventHandler<SkeletonChangedEventArgs>(_analyzer_handle);
            input.SkeletonChanged += _event_handler;
            input.SkeletonChanged += _analyzer_handler;
        }

        public SkeletonBinding(
                ISkeletonInput input,
                Configuration.InputValueHandle handle,
                BoneMarkers boneFirst,
                BoneMarkers boneSecond,
                float zeroAngle,
                float sensitivity)
            : base(handle, zeroAngle, sensitivity)
        {
            this._input = input;
            FirstBone = boneFirst;
            SecondBone = boneSecond;
            _type = BindingType.TwoBonesAngle;
            _event_handler = new EventHandler<SkeletonChangedEventArgs>(_input_assign_single_bone_angle);
            _analyzer_handler = new EventHandler<SkeletonChangedEventArgs>(_analyzer_handle);
            input.SkeletonChanged += _event_handler;
            input.SkeletonChanged += _analyzer_handler;
        }

        void _input_assign_single_bone_angle(object sender, SkeletonChangedEventArgs e)
        {
            Vector3 boneVector =
                e.Skeleton.GetPositionOf(_firstBoneMarkers.FirsrMarker) -
                e.Skeleton.GetPositionOf(_firstBoneMarkers.SecondMarker);
            boneVector.Normalize();
            float angle = (float)Math.Acos(Vector3.Dot(boneVector, Vector3.Right));
            //CallHandle((int)_bindedBoneFirst, angle);
            //            angle *= boneVector.X > 0 ? 1 : -1;
            Matrix m = Matrix.CreateRotationZ(MathHelper.ToRadians(ZeroAngle));
            Vector3 v = Vector3.Transform(Vector3.Right, m);

            boneVector.Z = 0;
            boneVector.Normalize();

            float angle2 = (float)Math.Acos(Vector3.Dot(boneVector, v));
            if (Vector3.Cross(boneVector, v).Z > 0)
                angle2 = -angle2;


            Handle(0, angle2 * Sensitivity);
        }

        void _input_assign_two_bones_angle(object sender, SkeletonChangedEventArgs e)
        {
            Vector3 firstBoneVector =
                e.Skeleton.GetPositionOf(_firstBoneMarkers.SecondMarker) -
                e.Skeleton.GetPositionOf(_firstBoneMarkers.FirsrMarker);
            Vector3 secondBoneVector =
                e.Skeleton.GetPositionOf(_secondBoneMarkers.SecondMarker) -
                e.Skeleton.GetPositionOf(_secondBoneMarkers.FirsrMarker);

            firstBoneVector.Normalize();
            secondBoneVector.Normalize();

            float angle = (float)Math.Acos(Vector3.Dot(firstBoneVector, secondBoneVector));
            CallHandle((int)_bindedBoneFirst, angle);
        }

        internal override void destroy()
        {
            _input.SkeletonChanged -= _event_handler;
            _input.SkeletonChanged -= _analyzer_handler;
            foreach (AnalyzerWrapper executor in _analyzerWrapers)
            {
                executor.Stop();
            }
        }

        private HashSet<AnalyzerWrapper> _analyzerWrapers = new HashSet<AnalyzerWrapper>();

        public void AddAnalyzer(ISkeletonAnalyzer analyzer, ObservableDictionary<string, string> parameters)
        {
            AnalyzerWrapper executor = new AnalyzerWrapper(analyzer, parameters);
            executor.Run();
            _analyzerWrapers.Add(executor);
            _analyzers.Add(analyzer, parameters);
        }

        void _analyzer_handle(object sender, SkeletonChangedEventArgs e)
        {
            foreach (AnalyzerWrapper analyzer in _analyzerWrapers)
            {
                analyzer.ProcessSkeleton(_bindedBoneFirst, e.Skeleton);
            }
        }
    }

    public class AbsoluteSkeletonBinding : AnalysableBinding
    {
        public enum BindingAxis
        {
            X,
            Y,
            Z
        }

        private BindingAxis _axis;
        public BindingAxis Axis
        {
            get
            {
                return _axis;
            }

            set
            {
                _axis = value;
            }
        }

        private SkeletonMarkers _trackedJoint;
        private SkeletonMarkers? _baseJoint;

        public SkeletonMarkers TrackedJoint
        {
            get
            {
                return _trackedJoint;
            }

            set
            {
                _trackedJoint = value;
            }
        }

        public SkeletonMarkers? BaseJoint
        {
            get
            {
                return _baseJoint;
            }

            set 
            {
                _baseJoint = value;    
            }
        }

        private ISkeletonInput _input;
        internal override IGamingInput Input
        {
            get { return _input; }
        }




        private EventHandler<SkeletonChangedEventArgs> _event_handler;
        private EventHandler<SkeletonChangedEventArgs> _analyzer_handler;

        public AbsoluteSkeletonBinding(
                ISkeletonInput input,
                Configuration.InputValueHandle handle,
                SkeletonMarkers trackedJoint,
                SkeletonMarkers? relativePositionToJoint,
                BindingAxis axis,
                float offset,
                float sensitivity)
            : base(handle, offset, sensitivity)
        {
            this._input = input;
            _trackedJoint = trackedJoint;
            _baseJoint = relativePositionToJoint;
            _axis = axis;
            _event_handler = new EventHandler<SkeletonChangedEventArgs>(_input_assign_position);
            _analyzer_handler = new EventHandler<SkeletonChangedEventArgs>(_analyzer_handle);
            input.SkeletonChanged += _event_handler;
            input.SkeletonChanged += _analyzer_handler;
            
        }

        void _input_assign_position(object sender, SkeletonChangedEventArgs e)
        {
            Vector3 value =
                _baseJoint != null && _baseJoint.HasValue ?
                e.Skeleton.GetPositionOf(_trackedJoint) - e.Skeleton.GetPositionOf(_baseJoint.Value) :
                e.Skeleton.GetPositionOf(_trackedJoint);
            value = value / 1000;
            switch (_axis)
            {
                case BindingAxis.X:
                    CallPositionalHandle((int)_trackedJoint, value.X);
                    break;
                case BindingAxis.Y:
                    CallPositionalHandle((int)_trackedJoint, value.Y);
                    break;
                case BindingAxis.Z:
                    CallPositionalHandle((int)_trackedJoint, value.Z);
                    break;
            }
        }

        internal override void destroy()
        {
            _input.SkeletonChanged -= _event_handler;
            _input.SkeletonChanged -= _analyzer_handler;
            foreach (AnalyzerWrapper executor in _analyzerWrappers)
            {
                executor.Stop();
            }
        }

        private HashSet<AnalyzerWrapper> _analyzerWrappers = new HashSet<AnalyzerWrapper>();

        public void AddAnalyzer(ISkeletonAnalyzer analyzer, ObservableDictionary<string, string> parameters)
        {
            AnalyzerWrapper executor = new AnalyzerWrapper(analyzer, parameters);
            executor.Run();
            _analyzerWrappers.Add(executor);
            _analyzers.Add(analyzer, parameters);
        }

        void _analyzer_handle(object sender, SkeletonChangedEventArgs e)
        {
            foreach (AnalyzerWrapper analyzer in _analyzerWrappers)
            {
                analyzer.ProcessSkeleton(0, e.Skeleton);
            }
        }
   } 
}
