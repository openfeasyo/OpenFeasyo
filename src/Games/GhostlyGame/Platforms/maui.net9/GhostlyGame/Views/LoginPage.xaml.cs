using GhostlyGame.Models;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Globalization;

namespace GhostlyGame.Views;

public partial class LoginPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (await GameSessionInfo.Instance.Uploader.SignIn(username.Text, password.Text))
        {
            //store username and password to securestorage
            //if logged, navigate to patient selection
            NavigateToPatientSelection();
            await SecureStorage.Default.SetAsync("username", username.Text);
            await SecureStorage.Default.SetAsync("password", password.Text);

            var storedUsername = await SecureStorage.Default.GetAsync("username");
            var storedPassword = await SecureStorage.Default.GetAsync("password");
        }
        else
        {
            //if not logged update err message
            this.ErrorMsg.Text = LocalizationResourceManager["UsernameOrPasswordAreIncorrect"].ToString();
        }
    }

    private async void OnInstructionVideoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(VideoPage));
    }

    private async void OnFR_langClicked(object sender, EventArgs e)
    {
        // Change the current culture to fr-FR
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
        LocalizationResourceManager.Instance.SetCulture(new CultureInfo("fr-FR"));
        Debug.WriteLine("CurrentCulture is now {0}.", CultureInfo.CurrentCulture.Name);

        await SecureStorage.Default.SetAsync("language", "fr-FR");

    }

    private async void OnNL_langClicked(object sender, EventArgs e)
    {
        // Change the current culture to nl-NL
        CultureInfo.CurrentCulture = new CultureInfo("nl-NL");
        LocalizationResourceManager.Instance.SetCulture(new CultureInfo("nl-NL")); 
        Debug.WriteLine("CurrentCulture is now {0}.", CultureInfo.CurrentCulture.Name);

        await SecureStorage.Default.SetAsync("language", "nl-NL");
    }

    private async void OnEN_langClicked(object sender, EventArgs e)
    {
        // Change the current culture to en-EN
        CultureInfo.CurrentCulture = new CultureInfo("en-EN");
        LocalizationResourceManager.Instance.SetCulture(new CultureInfo("en-EN"));
        Debug.WriteLine("CurrentCulture is now {0}.", CultureInfo.CurrentCulture.Name);

        await SecureStorage.Default.SetAsync("language", "en-EN");
    }

    private async void NavigateToPatientSelection()
    {
        await Shell.Current.GoToAsync(nameof(PatientSelectionPage));
    }
}