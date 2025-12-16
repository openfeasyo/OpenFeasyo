namespace GhostlyGame.Models
{
    internal class GameSessionInfo
    {
        private static GameSessionInfo _instance;
        internal static GameSessionInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameSessionInfo();
                }
                return _instance;
            }
        }

        public Models.User? LoggedInUser { get; set; }
        public Models.Patient? SelectedPatient { get; set; }
        public Session? Session { get; set; }

        public Uploader Uploader { get; }

        private int _startLevel = 191;

        public int StartLevel
        {
            get
            {
                return _startLevel;
                //TODO - read from the patient
                //SelectedPatient.FirstLevel
            }
        }

        public int LevelsCompleted { get; set; }
        public int RequiredLevels { get { return 3; } }//TODO read this from server??

        public GameSessionInfo()
        {
            Uploader = new Uploader();
            InitializeUploader();

            Session = new Session();
            LevelsCompleted = 0;
        }

        private async void InitializeUploader()
        {
            await Uploader.Initialize();
        }

        public bool SessionCompleted()
        {
            return LevelsCompleted == RequiredLevels;
        }
    }

    public class User
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
    }

    public class Session
    {
        public float BFR_target_lop_percentage_ch1 { get; set; }
        public float BFR_target_lop_percentage_ch2 { get; set; }
        public Int16 Rpe_post_session { get; set; }
    }
}
