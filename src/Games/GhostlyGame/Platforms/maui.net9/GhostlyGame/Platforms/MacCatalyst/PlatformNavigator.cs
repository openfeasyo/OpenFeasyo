namespace maui.net9;

public class PlatformNavigator : IPlatformNavigator
{
    public void OpenGameView()
    {
        using (var game = new GhostlyLib.GhostlyGame())
            game.Run();
    }
}