using System;

namespace OpenFeasyo.Platform.Network
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RemoteMethodAttribute : Attribute
    {        
    }

    [AttributeUsage(AttributeTargets.Event)]
    public class RemoteEventAttribute : Attribute
    {
        
    }
}
