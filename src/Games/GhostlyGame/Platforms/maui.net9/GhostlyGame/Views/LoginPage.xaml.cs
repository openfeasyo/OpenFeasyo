using GhostlyGame.Models;
using Microsoft.Maui.Controls;
using System.Globalization;
using System.Windows;

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

    private async void OnFR_langClicked(object sender, EventArgs e)
    {
        // Change the current culture to fr-FR
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR", false);
        Console.WriteLine("CurrentCulture is now {0}.", CultureInfo.CurrentCulture.Name);
    }

    private async void OnNL_langClicked(object sender, EventArgs e)
    {
        // Change the current culture to nl-NL
        CultureInfo.CurrentCulture = new CultureInfo("nl-NL", false);
        Console.WriteLine("CurrentCulture is now {0}.", CultureInfo.CurrentCulture.Name);
    }

    private async void OnEN_langClicked(object sender, EventArgs e)
    {
        // Change the current culture to en-EN
        CultureInfo.CurrentCulture = new CultureInfo("en-EN", false);
        Console.WriteLine("CurrentCulture is now {0}.", CultureInfo.CurrentCulture.Name);
    }

    private async void NavigateToPatientSelection()
    {
        await Shell.Current.GoToAsync(nameof(PatientSelectionPage));
    }
}