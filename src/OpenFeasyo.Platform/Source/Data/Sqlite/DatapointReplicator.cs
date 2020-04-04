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
