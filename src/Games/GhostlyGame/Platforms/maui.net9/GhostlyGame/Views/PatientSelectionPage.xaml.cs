using GhostlyGame.Models;
using maui.net9;
using Microsoft.Maui.Controls;
using OpenFeasyo.Platform.Data;

namespace GhostlyGame.Views;

public partial class PatientSelectionPage : ContentPage
{
    private readonly IPlatformNavigator _navigator;

    public PatientSelectionPage(IPlatformNavigator navigator)
    { 
        InitializeComponent();

        BindingContext = new Models.AllPatients();
        _navigator = navigator;
    }

    async private void OnContinueClicked(object sender, EventArgs e)
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

            //TODO - change dashboard to return default value
            if (GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh1Ms == null)
                GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh1Ms = 3000;
            if (GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh2Ms == null)
                GameSessionInfo.Instance.SelectedPatient.CurrentTargetCh2Ms = 3000;

            // Should navigate to game menu page
            //await Shell.Current.GoToAsync(nameof(GameMenuPage));
            _navigator.OpenGameView(Application.Current);
        }
    }
}