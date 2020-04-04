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
