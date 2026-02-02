using GhostlyGame.Models;
using maui.net9;
using Microsoft.Maui.Controls;
using OpenFeasyo.Platform.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GhostlyGame.Views;

public partial class PatientSelectionPage : ContentPage, INotifyPropertyChanged
{
    private readonly IPlatformNavigator _navigator;

    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;

    private ObservableCollection<Models.Patient> _patients;
    public ObservableCollection<Models.Patient> Patients
    {
        get
        {
            return _patients;
        }
        set
        {
            _patients = value;
            PatientsCollection.ItemsSource = Patients;
            OnPropertyChanged("Patients");
        }
    }
        
    public PatientSelectionPage(IPlatformNavigator navigator)
    {
        InitializeComponent();

        BindingContext = this; // Models.AllPatients.CreateAsync();
        _navigator = navigator;

        LoadPatients();

        this.SelectPatientLabel.Text = LocalizationResourceManager.Instance["SelectPatient"].ToString();
        this.ContinueBtn.Text = LocalizationResourceManager.Instance["Continue"].ToString();
    }

    private async void LoadPatients()
    {
        var allpatients = await Models.AllPatients.CreateAsync();
        Patients = allpatients.Patients;
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        Models.Patient selectedPatient = (Models.Patient)this.PatientsCollection.SelectedItem;

        if (selectedPatient != null)
        {
            //hide selection elements and show loading
            Selection.IsVisible = false;
            Loading.IsVisible = true;

            //save selected patient to game session info
            GameSessionInfo.Instance.SelectedPatient = selectedPatient;

            //update the currentpatient.id, which is stored in the c3d file
            SeriousGames.CurrentPatient.Id = selectedPatient.PatientCode;

            await SecureStorage.Default.SetAsync("patient", selectedPatient.PatientCode);

            //TODO - fallback if dashboard returns nulls
            if (GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh1Ms == null)
                GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh1Ms = 3000;
            if (GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh2Ms == null)
                GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh2Ms = 3000;
            if (GameSessionInfo.Instance.SelectedPatient.DifficultyLevel == null)
                GameSessionInfo.Instance.SelectedPatient.DifficultyLevel = 1;
            if (GameSessionInfo.Instance.SelectedPatient.LevelToPlay == null)
                GameSessionInfo.Instance.SelectedPatient.LevelToPlay = 191;

            // Navigate to the game
            _navigator.OpenGameView(Application.Current);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}