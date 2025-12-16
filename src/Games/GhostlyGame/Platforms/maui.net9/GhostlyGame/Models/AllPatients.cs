using System.Collections.ObjectModel;

namespace GhostlyGame.Models
{
    internal class AllPatients
    {
        public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;

        public ObservableCollection<Patient> Patients { get; set; } = new ObservableCollection<Patient>();

        public AllPatients() { }

        public static async Task<AllPatients> CreateAsync()
        {
            var instance = new AllPatients();
            await instance.LoadPatients();
            return instance;
        }

        public async Task LoadPatients()
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