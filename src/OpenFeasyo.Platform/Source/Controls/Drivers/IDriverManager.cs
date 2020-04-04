using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls.Drivers
{
    public interface IDriverManager
    {
        ObservableCollection<IDevice> Drivers { get; }

        void UnloadAll();
    }
}
