using GhostlyGame;
using OpenFeasyo.Platform.Controls.Drivers;

namespace maui.net9;

using Microsoft.Maui.Controls;

public partial class MainPage : ContentPage
{
	int count = 0;
	private readonly IPlatformNavigator _navigator;
	
	public MainPage(IPlatformNavigator navigator)
	{
		InitializeComponent();
		_navigator = navigator;
		InputDeviceManager.Instance = new StaticDriverManager();
	}

	private void OnCounterClicked(object? sender, EventArgs e)
	{
		_navigator.OpenGameView();
	}
}
