using OpenFeasyo.Platform.Controls.Drivers;
using System.Collections.ObjectModel;

using TrignoAvantiCustom;

namespace GhostlyGame;

public class StaticDriverManager : IDriverManager
{
    ObservableCollection<IDevice> _drivers = new ObservableCollection<IDevice>();

    internal StaticDriverManager()
    {
        _drivers.Add(new TrignoAvanti());
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