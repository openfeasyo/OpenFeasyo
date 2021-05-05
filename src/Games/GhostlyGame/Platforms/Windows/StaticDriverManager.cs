/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2021 - Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using System.Collections.ObjectModel;
using OpenFeasyo.Platform.Controls.Drivers;

// references to linked drivers
using TrignoEmg;

namespace GhostlyLib
{
    public class StaticDriverManager : IDriverManager
    {
        ObservableCollection<IDevice> _drivers = new ObservableCollection<IDevice>();

        internal StaticDriverManager()
        {
            _drivers.Add(new TrignoEmgDevice());
        }

        public ObservableCollection<IDevice> Drivers
        {
            get { return _drivers; }
        }

        public void UnloadAll()
        {
            foreach (IDevice d in Drivers)
            {
                if (d.IsLoaded)
                {
                    d.UnloadDriver();
                }
            }
        }
    }
}
