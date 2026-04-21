using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Drivers;

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

        private float max1 = float.NegativeInfinity;
        public float Max1 { get { return max1; } set { if (value != max1) max1 = value; } }
        private float max2 = float.NegativeInfinity;
        public float Max2 { get { return max2; } set { if (value != max2) max2 = value; } }

        public string LeftSensor { get; set; }
        public string RightSensor { get; set; }

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
        /*public int LevelToPlay()
        {
            return GameSessionInfo.firstPossibleLevel + (((int)GameSessionInfo.Instance.SelectedPatient.LevelToPlay - GameSessionInfo.firstPossibleLevel) + this.LevelsCompleted) % (GameSessionInfo.lastPossibleLevel - GameSessionInfo.firstPossibleLevel + 1);
        }*/

        public void UpdateSelectedPatientsLevelToPlay()
        {
            GameSessionInfo.Instance.SelectedPatient.LevelToPlay = GameSessionInfo.firstPossibleLevel + (((int)GameSessionInfo.Instance.SelectedPatient.LevelToPlay - GameSessionInfo.firstPossibleLevel) + this.LevelsCompleted) % (GameSessionInfo.lastPossibleLevel - GameSessionInfo.firstPossibleLevel + 1);
        }

        public IEmgSensorInput GetSensorInput()
        {
            Dictionary<string, IDevice> devices = PrepareDevicesByName();
            if (!devices.ContainsKey("Trigno Avanti"))
            {
                throw new ApplicationException("TrignoAvantiCustomEmg not loaded");
            }
            IDevice dev = devices["Trigno Avanti"];
            if (!dev.IsLoaded)
            {
                dev.LoadDriver(new Dictionary<string, string>());
            }

            return dev.GamingInput as IEmgSensorInput;
        }

        private static Dictionary<string, IDevice> PrepareDevicesByName()
        {
            Dictionary<string, IDevice> devicesByName = new Dictionary<string, IDevice>();
            foreach (IDevice device in InputDeviceManager.Drivers)
            {
                devicesByName.Add(device.Name, device);
            }
            return devicesByName;
        }
    }

    public class User
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
    }

    public class Session
    {
        public float BFR_target_vop_percentage_ch1 { get; set; }
        public float BFR_target_vop_percentage_ch2 { get; set; }
        public Int16 Rpe_post_session { get; set; }
    }
}
