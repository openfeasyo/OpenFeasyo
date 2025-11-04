namespace maui.net9;

using GhostlyGame.Views;
using Microsoft.Maui.Controls;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(PatientSelectionPage), typeof(PatientSelectionPage));
        Routing.RegisterRoute(nameof(VideoPage), typeof(VideoPage));
    }
}
