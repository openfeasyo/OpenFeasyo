using GhostlyGame.Models;
using maui.net9;
using Microsoft.Maui.Controls;
using OpenFeasyo.Platform.Data;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace GhostlyGame.Views;

public partial class PatientLogin : ContentPage, INotifyPropertyChanged
{
    private readonly IPlatformNavigator _navigator;
    private string storedUsername;
    private string storedPassword;

    private string storedPatient;
    public string StoredPatient
    {
        get => storedPatient;
        set
        {
            storedPatient = value;
            OnPropertyChanged();
        }
    }
    private string storedLanguage;

    private bool _initialized = false;

    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;

    public PatientLogin(IPlatformNavigator navigator)
    {
        InitializeComponent();
        BindingContext = this;
        _navigator = navigator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Prevent re-entry (OnAppearing can fire multiple times)
        if (_initialized)
            return;

        _initialized = true;

        await CheckLoginAsync();
    }

    private async Task CheckLoginAsync()
    {
        try
        {
            //take stored login info, patient ID, and navigate to game screen
            storedUsername = await SecureStorage.Default.GetAsync("username");
            storedPassword = await SecureStorage.Default.GetAsync("password");
            StoredPatient = await SecureStorage.Default.GetAsync("patient");
            storedLanguage = await SecureStorage.Default.GetAsync("language");

            if (storedUsername == null || storedPassword == null || StoredPatient == null)
                NavigateToLogin();

            if (storedLanguage == null)
            {
                //set default language
                storedLanguage = "en-EN";
                await SecureStorage.Default.SetAsync("language", storedLanguage);
            }

            //set up culture
            CultureInfo.CurrentCulture = new CultureInfo(storedLanguage);
            LocalizationResourceManager.Instance.SetCulture(new CultureInfo(storedLanguage));
        }
        catch (Exception ex)
        {
            NavigateToLogin();
        }
    }

    private async void OnYesClicked(object sender, EventArgs e)
    {
        //hide selection elements and show loading
        Selection.IsVisible = false;
        Loading.IsVisible = true;

        if (await GameSessionInfo.Instance.Uploader.SignIn(storedUsername, storedPassword))
        {
            //get all patients of the therapist, check if current pateint is his patient
            var allpatients = await Models.AllPatients.CreateAsync();

            IEnumerable<Models.Patient> p = allpatients.Patients.Where(o => o.PatientCode == StoredPatient);

            if (p.Count() > 0)
            {
                //update the currentpatient.id, which will be stored in the c3d file
                SeriousGames.CurrentPatient.Id = StoredPatient;

                GameSessionInfo.Instance.SelectedPatient = p.First();

                if (GameSessionInfo.Instance.SelectedPatient.CurrentDifficultyLevel == null)
                    GameSessionInfo.Instance.SelectedPatient.CurrentDifficultyLevel = 1;
                if (GameSessionInfo.Instance.SelectedPatient.LevelToPlay == null)
                    GameSessionInfo.Instance.SelectedPatient.LevelToPlay = 191;

                // Should navigate to game menu page
                _navigator.OpenGameView(Application.Current);
            }
            else
            {
                //current patient is not therapists patient
                GameSessionInfo.Instance.Uploader.SignOut();
                NavigateToLogin();
            }
        }
        else
        {
            //navigate to Login screen
            NavigateToLogin();
        }
    }

    private async void OnNoClicked(object sender, EventArgs e)
    {
        SecureStorage.Default.Remove("patient");
        NavigateToLogin();
    }

    private async void NavigateToLogin()
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }

    private async void OnFR_langClicked(object sender, EventArgs e)
    {
        // Change the current culture to fr-FR
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
        LocalizationResourceManager.Instance.SetCulture(new CultureInfo("fr-FR"));

        await SecureStorage.Default.SetAsync("language", "fr-FR");
    }

    private async void OnNL_langClicked(object sender, EventArgs e)
    {
        // Change the current culture to nl-NL
        CultureInfo.CurrentCulture = new CultureInfo("nl-NL");
        LocalizationResourceManager.Instance.SetCulture(new CultureInfo("nl-NL"));

        await SecureStorage.Default.SetAsync("language", "nl-NL");
    }

    private async void OnEN_langClicked(object sender, EventArgs e)
    {
        // Change the current culture to en-EN
        CultureInfo.CurrentCulture = new CultureInfo("en-EN");
        LocalizationResourceManager.Instance.SetCulture(new CultureInfo("en-EN"));

        await SecureStorage.Default.SetAsync("language", "en-EN");
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}