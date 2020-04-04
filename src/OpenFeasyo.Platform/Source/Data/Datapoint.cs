using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFeasyo.Platform.Data
{
    public interface Datapoint
    {
        void Insert<T>(T obj, bool forcePrimaryKey = true);
        T[] SelectAll<T>();
        void Remove<T>(T obj);
        void Update<T>(T obj);
    }
}
