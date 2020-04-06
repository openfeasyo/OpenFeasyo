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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace OpenFeasyo.Platform.Data.Sqlite
{
    public class DatapointReplicator
    {
        public void UpdateLocalConfiguredGames<T>() { 
            Type t = typeof(T);
            Console.WriteLine("Type name: " + t.Name);
            IEnumerable<PropertyInfo> props = t.GetProperties().Where(p => Attribute.IsDefined(p, typeof(DataMemberAttribute))).OrderBy(i => i.Name);
            foreach(PropertyInfo p in props){
                Console.WriteLine(p.Name);
            }
        }

        public void CreateTable<T>() { 
            
        }





        
    }
}
