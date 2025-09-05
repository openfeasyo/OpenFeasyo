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
	}

	private void OnCounterClicked(object? sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else {
			CounterBtn.Text = $"Clicked {count} times";
			_navigator.OpenGameView();
		}
		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}
