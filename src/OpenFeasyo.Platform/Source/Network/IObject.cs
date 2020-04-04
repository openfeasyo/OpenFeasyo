namespace OpenFeasyo.Platform.Network
{
    public delegate void TestEvent(string num);
    
	public delegate void GameChangedEvent(string [] bps);
    public delegate void SkeletonEvent(int [] num);
    public delegate void BalanceBoardEvent(int[] num);

    public interface IObject
    {
        #region Devices

        [RemoteMethod]
        string[] GetAvailableDevices();

        [RemoteMethod]
        bool LoadDevice(string s);

        [RemoteMethod]
        bool UnloadDevice(string s);

        #endregion Devices

        #region Games

        [RemoteMethod]
        string GetGameName();

		[RemoteEvent]
		event GameChangedEvent GameChanged;

        [RemoteMethod]
        string[] GetBindingPoints();

		#endregion Games

        #region BalanceBoardBinding

        /// <param name="bindingPoint">Id of binding point on the server.</param>
        /// <param name="direction">Direction of captured movement. 0 - horizontal, 1 - vertical, 2 horizontal inverted, 3 vertical inverted</param>
        /// <param name="positionVertical">Displacement of the center in milimeters vertically</param>
        /// <param name="positionHorizontal">Displacement of the center in (milimeters - not yet) horizontally</param>
        /// <param name="rangeRed">Displacement position Direction of captured movement.</param>
        [RemoteMethod]
        bool SetBalanceBoardBinding(string bindingPoint, int direction, double displacement, double rangeRed, double rangeBlue);

        [RemoteEvent]
        event BalanceBoardEvent NewBalanceBoard;

        #endregion BalanceBoardBinding

        
        #region SkeletonBinding

        [RemoteEvent]
        event SkeletonEvent NewSkeleton;

        [RemoteMethod]
        bool SetAngleBinding(string bindingPoint, int bone, int zeroAngle, int range);

        [RemoteMethod]
        bool SetJointPositionBinding(string bindingPoint, int jointTracked, int jointBase, double range, int axis);

        #endregion SkeletonBinding

        
        
        [RemoteMethod]
        int HeartBeat(int a);

    }
}
