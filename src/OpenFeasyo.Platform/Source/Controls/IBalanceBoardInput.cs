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
