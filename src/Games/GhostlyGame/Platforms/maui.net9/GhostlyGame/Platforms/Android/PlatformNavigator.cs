using Android.Content;
using GhostlyLib;
using Microsoft.Maui.Controls;

namespace maui.net9;

public class PlatformNavigator : IPlatformNavigator
{
    public void OpenGameView(Application thisApp)
    {
        var context = Platform.CurrentActivity ?? Android.App.Application.Context;
        var intent = new Intent(context, typeof(GhostlyGameActivity));
        context.StartActivity(intent);
    }
}