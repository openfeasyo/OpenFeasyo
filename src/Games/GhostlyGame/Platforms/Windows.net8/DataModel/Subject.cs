namespace GhostlyGame.DataModel
{
    internal class Subject
    {
        private String userName;
        
        private int targetDuration;

        public String UserName { get { return userName; } }
        public int TargetDuration { get { return targetDuration; } }


        public Subject(string userName, int targetDuration) { 
        
            this.userName = userName;
            this.targetDuration = targetDuration;
        }        
    }
}
