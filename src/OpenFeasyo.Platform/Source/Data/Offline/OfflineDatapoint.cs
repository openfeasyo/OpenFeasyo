using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFeasyo.Platform.Data.Offline
{
    public class OfflineDatapoint : Datapoint
    {
        private Dictionary<Type, object> _offlineDefinitions;
        public OfflineDatapoint() {
            _offlineDefinitions = new Dictionary<Type, object>();

            // Configured games
            ConfiguredGame cg = new ConfiguredGame();
            cg.Name = "HitTheBoxes";
            cg.Parameters = "<?xml version=\"1.0\" encoding=\"utf - 8\"?><Configuration><devices><device name=\"Kinect\"><param name=\"ShowPreview\" value=\"true\" /><analyzers><analyzer file=\"C3DSerializer.dll\" /></analyzers></device></devices><bindings><binding point=\"Horizontal\" zeroAngle=\"90.58064\" sensitivity=\"0.7419356\" device=\"Kinect\"><skeleton><type><BindingType>SingleBoneAngle</BindingType></type><firstBone><BoneMarkers>Spine</BoneMarkers></firstBone></skeleton></binding></bindings></Configuration>";
            _offlineDefinitions.Add(typeof(ConfiguredGame), new ConfiguredGame[] { cg });

            _offlineDefinitions.Add(typeof(ExtendedPatient), new ExtendedPatient[] { });
            _offlineDefinitions.Add(typeof(MaxScore), new MaxScore[] { });
        }

        public void Insert<T>(T obj, bool forcePrimaryKey = true)
        {
            // Do nothing since we are offline
        }

        public void Remove<T>(T obj)
        {
            // Do nothing since we are offline
        }

        public T[] SelectAll<T>()
        {
            if (_offlineDefinitions.ContainsKey(typeof(T)))
            {
                return (T[])_offlineDefinitions[typeof(T)];
            }
            else {
                throw new NotImplementedException("Add definitions for type: " + typeof(T));
            }
        }

        public void Update<T>(T obj)
        {
            // Do nothing since we are offline
        }
    }
}
