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
using System;

namespace OpenFeasyo.Platform.Controls
{
    ///<summary>
    /// Interface for skeleton based inputs for rehabilitation games.</summary>
    public interface ISkeletonInput : IGamingInput
    {
        event EventHandler<SkeletonChangedEventArgs> SkeletonChanged;

    }

    ///<summary>
    /// Specialized event class for the skeletonchanged event. It contains
    /// joint points coordinates in unified form. </summary>
    public class SkeletonChangedEventArgs : EventArgs
    {

        ///<summary>
        /// Instance variable to store the unified skeleton. </summary>
        private ISkeleton _skeleton;

        ///<summary>
        /// Constructor that sets current skeleton for the event. </summary>
        public SkeletonChangedEventArgs(ISkeleton skeleton) { _skeleton = skeleton; }

        ///<summary>
        /// Read only property for the skeleton. </summary>
        public ISkeleton Skeleton { get { return _skeleton; } }

    }
}
