using Microsoft.Maui.Controls;

namespace maui.net9;

public class PlatformNavigator : IPlatformNavigator
{
    void OpenGameView()
    {
        // TODO Open Game view
        //var frame = (Frame)Window.Current.Content;
        //frame.Navigate(typeof(MyWindowsPage));
    }

    void IPlatformNavigator.OpenGameView(Application thisApp)
    {
        using (var game = new GhostlyLib.GhostlyGame())
        {
            game.Exiting += (s, args) =>
            {
                // Close MAUI when the MonoGame window is closing
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    thisApp.Quit();
                });
            };

            game.Run();
        }
    }
}