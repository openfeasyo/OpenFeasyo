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