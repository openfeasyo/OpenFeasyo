using Android.Content;
using GhostlyLib;

namespace maui.net9;

public class PlatformNavigator : IPlatformNavigator
{
    public void OpenGameView()
    {
        var context = Platform.CurrentActivity ?? Android.App.Application.Context;
        var intent = new Intent(context, typeof(GhostlyGameActivity));
        context.StartActivity(intent);
    }
}