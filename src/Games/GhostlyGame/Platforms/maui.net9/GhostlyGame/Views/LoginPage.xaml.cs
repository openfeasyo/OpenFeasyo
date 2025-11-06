using GhostlyGame.Models;
using Microsoft.Maui.Controls;

namespace GhostlyGame.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (await GameSessionInfo.Instance.Uploader.SignIn(username.Text, password.Text))
        {
            //if logged, navigate to patient selection
            NavigateToPatientSelection();
        }
        else
        {
            //if not logged update err message
            this.ErrorMsg.Text = "Username or password are incorrect!";
        }
    }

    private async void OnInstructionVideoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(VideoPage));
    }

    private async void NavigateToPatientSelection()
    {
        await Shell.Current.GoToAsync(nameof(PatientSelectionPage));
    }
}