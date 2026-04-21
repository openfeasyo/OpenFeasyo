using Supabase;
using System.Diagnostics;

namespace GhostlyGame.Models
{
    internal class Uploader
    {
        private Supabase.Client _supabaseClient;
        private String _bucketName;

        public Uploader()
        {
            if (_supabaseClient == null)
            {
                var options = new SupabaseOptions
                {
                    AutoRefreshToken = true,        // Auto-refresh JWT tokens
                    AutoConnectRealtime = false      // Not needed for upload
                };

                //TODO read from appsettings.json
                //1. connection to DB created by Guillaume
                //_supabaseClient = new Client("https://egihfsmxphqcsjotmhmm.supabase.co", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImVnaWhmc214cGhxY3Nqb3RtaG1tIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDcxMzM0MDksImV4cCI6MjA2MjcwOTQwOX0.T-SPGmTmS0gR2fHvuYgcrcrJRjROk691T9zdMvEH78E", options);
                //2. connection to DB created by Katka (Guillaume's DB was dumped and restored to this DB)
                //_supabaseClient = new Client("https://nucyljbytyasorkmtdfn.supabase.co", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im51Y3lsamJ5dHlhc29ya210ZGZuIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njk0NzE2MDksImV4cCI6MjA4NTA0NzYwOX0.JZybF-YlRH6Qcd5K9yT6MPGG0hEZTxmF7EfBxH2w2xI", options);
                //3. connection to self hosted DB (Guillaume's DB schema was respored here + some columns added)
                _supabaseClient = new Client("https://ghostlydb.etro.vub.be", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzdXBhYmFzZSIsImlhdCI6MTc3NjE3NTMyMCwiZXhwIjo0OTMxODQ4OTIwLCJyb2xlIjoiYW5vbiJ9.pyg2KqXf8DqmDccpYY8m9K_u6VoBzKLlEZct5o_z0M4", options);
                                
                _bucketName = "emg_data";//bucketName;
            }
        }

        public async Task Initialize()
        {
            await _supabaseClient.InitializeAsync();
        }

        public async Task<bool> SignIn(string email, string password)
        {
            try
            {
                var response = await _supabaseClient.Auth.SignIn(email, password);
                if (response?.User != null)
                {
                    GameSessionInfo.Instance.LoggedInUser = new User() { Id = response?.User.Id, Email = response?.User.Email };
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"❌ Auth error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Patient>> GetAssignedPatients()
        {
            try
            {
                var response = await _supabaseClient.From<Patient>().Get();
                Debug.WriteLine(response);
                return response.Models ?? new List<Patient>();
            }
            catch (Exception ex)
            {
                return new List<Patient>();
            }
        }

        public async Task<bool> UploadC3DFile(string filePath, string patientCode)
        {
            try
            {
                // Read file
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                var fileName = Path.GetFileName(filePath);

                // Storage path follows pattern: {patientCode}/{filename}
                var storagePath = $"{patientCode}/{fileName}";

                // Upload to Supabase Storage
                var storage = _supabaseClient.Storage.From(_bucketName);
                var result = await storage.Upload(
                    fileBytes,
                    storagePath,
                    new Supabase.Storage.FileOptions
                    {
                        ContentType = "application/octet-stream",
                        Upsert = false  // Don't overwrite existing files
                    }
                );

                if (!string.IsNullOrEmpty(result))
                {
                    Console.WriteLine($"✅ File uploaded: {storagePath}");
                    Console.WriteLine("ℹ️ Database record created automatically via webhook");
                    return true;
                }

                Console.WriteLine("❌ Upload failed");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Upload error: {ex.Message}");
                return false;
            }
        }

        public async void UpdatePatientsCurrentDifficultyLevel()
        {
            var updatedPatient = GameSessionInfo.Instance.SelectedPatient;

            await _supabaseClient.From<Patient>().Where(x=>x.Id == updatedPatient.Id).Update(updatedPatient);
        }

        public async Task SignOut()
        {
            await _supabaseClient.Auth.SignOut();
        }
    }
}