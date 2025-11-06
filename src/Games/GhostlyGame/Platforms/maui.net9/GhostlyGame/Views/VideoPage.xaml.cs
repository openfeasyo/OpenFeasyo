using Microsoft.Maui.Controls;

namespace GhostlyGame.Views;

public partial class VideoPage : ContentPage
{
	public VideoPage()
	{
		InitializeComponent();
	}

    private async void OnOKClicked(object sender, EventArgs e)
    {
        //TODO navigate back (to login page)
        await Shell.Current.GoToAsync("..");
    }
}