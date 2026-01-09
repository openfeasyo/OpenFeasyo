namespace GhostlyGame.Models
{
    internal class GameSessionInfo
    {
        private const int firstPossibleLevel = 191; // first level of the simple space game used in the Ghostly+ study
        private const int lastPossibleLevel = 200; // last level of the simple space game used in the Ghostly+ study

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

        /*public bool SessionCompleted()
        {
            return LevelsCompleted == RequiredLevels;
        }*/

        //helper method to ensure only levels 191 - 200 are played, in a loop, i.e. 200 -> 191 ...
        public int LevelToPlay()
        {
            return GameSessionInfo.firstPossibleLevel + (((int)GameSessionInfo.Instance.SelectedPatient.LevelToPlay - GameSessionInfo.firstPossibleLevel) + this.LevelsCompleted) % (GameSessionInfo.lastPossibleLevel - GameSessionInfo.firstPossibleLevel + 1);
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
