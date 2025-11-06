using GhostlyLib.Elements;

namespace GhostlyLib.DynamicDifficulty
{
    public class LevelAnalytics
    {
        private List<TurnAnalytics> _turns;

        public LevelAnalytics()
        {
            _turns = new List<TurnAnalytics>();
        }

        public int Evaluate()
        {
            List<TurnAnalytics> turnsDonePerfectly = _turns.Where(o => o.desiredDirection.Equals(o.performedDirection) && o.contractions == 1).ToList();

            if (turnsDonePerfectly.Count() > (_turns.Count() * 0.75))
            {
                //to increase the desired contraction length
                return 1;
            }
            else if (turnsDonePerfectly.Count() < (_turns.Count() * 0.5))
            {
                //to decrease the desired contraction length
                return -1;
            }

            return 0;
        }

        public void UpdateTurn(int turnNumber, int contractions, RotationDirection desiredDirection, RotationDirection performedDirection)
        {
            if (_turns.Count < turnNumber + 1) // new turn
            {
                _turns.Add(new TurnAnalytics() { turnNumber = turnNumber, contractions = contractions, desiredDirection = desiredDirection, performedDirection = performedDirection });
            }
            else //update, i.e., another contraction and possible change of direction
            { 
                TurnAnalytics turnAnalytics = _turns[turnNumber];
                turnAnalytics.contractions += contractions;
                turnAnalytics.performedDirection = performedDirection;
            }
        }

        private class TurnAnalytics
        {
            internal int turnNumber;
            internal int contractions;
            internal RotationDirection desiredDirection;
            internal RotationDirection performedDirection;

            public TurnAnalytics() { }
        }
    }
}