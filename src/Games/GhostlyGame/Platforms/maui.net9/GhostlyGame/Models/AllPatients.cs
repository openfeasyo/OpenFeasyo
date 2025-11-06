using System.Collections.ObjectModel;

namespace GhostlyGame.Models
{
    internal class AllPatients
    {
        public ObservableCollection<Patient> Patients { get; set; } = new ObservableCollection<Patient>();

        public AllPatients() =>
            LoadPatients();

        public async void LoadPatients()
        {
            Patients.Clear();

            //load all patients of the signed in therapist
            var assignedPatients = await GameSessionInfo.Instance.Uploader.GetAssignedPatients();

            // Add each note into the ObservableCollection
            foreach (Patient patient in assignedPatients.OrderBy(o => o.PatientCode))
            {
                Patients.Add(patient);
            }
        }
    }
}
