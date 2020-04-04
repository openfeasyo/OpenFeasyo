using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls
{
    public interface IBalanceBoardInput : IGamingInput
    {
        event EventHandler<BalanceChangedEventArgs> BalanceChanged;
    }

    ///<summary>
    /// Specialized event class for the balancechanged event. It contains
    /// center of the pressure coordinate in unified form. </summary>
    public class BalanceChangedEventArgs : EventArgs
    {

        ///<summary>
        /// Instance variable to store the unified balance board. </summary>
        private IBalanceBoard _balance;

        ///<summary>
        /// Constructor that sets current balance state for the event. </summary>
        public BalanceChangedEventArgs(IBalanceBoard balance) { _balance = balance; }

        ///<summary>
        /// Read only property for the balance board. </summary>
        public IBalanceBoard Balance { get { return _balance; } }

    }
}
