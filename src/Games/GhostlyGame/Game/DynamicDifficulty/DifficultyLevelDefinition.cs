namespace GhostlyLib.DynamicDifficulty
{
    public class DifficultyLevelDefinition
    {
        public int contractionDuration;
        public int restDuration;
        public float _MVCLevel;
    } 

    public class DifficultyLevelStateSpace
    {
        private static DifficultyLevelStateSpace _instance;

        public Dictionary<int, DifficultyLevelDefinition> DifficultyLevelDefinitions { get; set; }

        public static DifficultyLevelStateSpace Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DifficultyLevelStateSpace();
                }
                return _instance;
            }
        }

        public DifficultyLevelStateSpace()
        {
            DifficultyLevelDefinitions = new Dictionary<int, DifficultyLevelDefinition>();

            defineLevels();
        }

        private void defineLevels()
        {            
            DifficultyLevelDefinitions.Add(1, new DifficultyLevelDefinition() { _MVCLevel = 0.2f, contractionDuration = 3, restDuration = 10 });
            DifficultyLevelDefinitions.Add(2, new DifficultyLevelDefinition() { _MVCLevel = 0.45f, contractionDuration = 3, restDuration = 10 });
            DifficultyLevelDefinitions.Add(3, new DifficultyLevelDefinition() { _MVCLevel = 0.6f, contractionDuration = 3, restDuration = 10 });

            DifficultyLevelDefinitions.Add(4, new DifficultyLevelDefinition() { _MVCLevel = 0.2f, contractionDuration = 6, restDuration = 10 });
            DifficultyLevelDefinitions.Add(5, new DifficultyLevelDefinition() { _MVCLevel = 0.45f, contractionDuration = 6, restDuration = 10 });
            DifficultyLevelDefinitions.Add(6, new DifficultyLevelDefinition() { _MVCLevel = 0.6f, contractionDuration = 6, restDuration = 10 });

            DifficultyLevelDefinitions.Add(7, new DifficultyLevelDefinition() { _MVCLevel = 0.2f, contractionDuration = 10, restDuration = 10 });
            DifficultyLevelDefinitions.Add(8, new DifficultyLevelDefinition() { _MVCLevel = 0.45f, contractionDuration = 10, restDuration = 10 });
            DifficultyLevelDefinitions.Add(9, new DifficultyLevelDefinition() { _MVCLevel = 0.6f, contractionDuration = 10, restDuration = 10 });

            DifficultyLevelDefinitions.Add(10, new DifficultyLevelDefinition() { _MVCLevel = 0.2f, contractionDuration = 3, restDuration = 6 });
            DifficultyLevelDefinitions.Add(11, new DifficultyLevelDefinition() { _MVCLevel = 0.45f, contractionDuration = 3, restDuration = 6 });
            DifficultyLevelDefinitions.Add(12, new DifficultyLevelDefinition() { _MVCLevel = 0.6f, contractionDuration = 3, restDuration = 6 });

            DifficultyLevelDefinitions.Add(13, new DifficultyLevelDefinition() { _MVCLevel = 0.2f, contractionDuration = 6, restDuration = 6 });
            DifficultyLevelDefinitions.Add(14, new DifficultyLevelDefinition() { _MVCLevel = 0.45f, contractionDuration = 6, restDuration = 6 });
            DifficultyLevelDefinitions.Add(15, new DifficultyLevelDefinition() { _MVCLevel = 0.6f, contractionDuration = 6, restDuration = 6 });

            DifficultyLevelDefinitions.Add(16, new DifficultyLevelDefinition() { _MVCLevel = 0.2f, contractionDuration = 10, restDuration = 6 });
            DifficultyLevelDefinitions.Add(17, new DifficultyLevelDefinition() { _MVCLevel = 0.45f, contractionDuration = 10, restDuration = 6 });
            DifficultyLevelDefinitions.Add(18, new DifficultyLevelDefinition() { _MVCLevel = 0.6f, contractionDuration = 10, restDuration = 6 });

            DifficultyLevelDefinitions.Add(19, new DifficultyLevelDefinition() { _MVCLevel = 0.2f, contractionDuration = 3, restDuration = 3 });
            DifficultyLevelDefinitions.Add(20, new DifficultyLevelDefinition() { _MVCLevel = 0.45f, contractionDuration = 3, restDuration = 3 });
            DifficultyLevelDefinitions.Add(21, new DifficultyLevelDefinition() { _MVCLevel = 0.6f, contractionDuration = 3, restDuration = 3 });

            DifficultyLevelDefinitions.Add(22, new DifficultyLevelDefinition() { _MVCLevel = 0.2f, contractionDuration = 6, restDuration = 3 });
            DifficultyLevelDefinitions.Add(23, new DifficultyLevelDefinition() { _MVCLevel = 0.45f, contractionDuration = 6, restDuration = 3 });
            DifficultyLevelDefinitions.Add(24, new DifficultyLevelDefinition() { _MVCLevel = 0.6f, contractionDuration = 6, restDuration = 3 });

            DifficultyLevelDefinitions.Add(25, new DifficultyLevelDefinition() { _MVCLevel = 0.2f, contractionDuration = 10, restDuration = 3 });
            DifficultyLevelDefinitions.Add(26, new DifficultyLevelDefinition() { _MVCLevel = 0.45f, contractionDuration = 10, restDuration = 3 });
            DifficultyLevelDefinitions.Add(27, new DifficultyLevelDefinition() { _MVCLevel = 0.6f, contractionDuration = 10, restDuration = 3 });           
        }

        public DifficultyLevelDefinition getLevelDefinition(int level)
        {
            return this.DifficultyLevelDefinitions[level];
        }
    }
}