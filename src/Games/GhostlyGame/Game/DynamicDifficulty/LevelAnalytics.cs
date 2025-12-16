using GhostlyLib.Elements;
using System.Diagnostics;

namespace GhostlyLib.DynamicDifficulty
{
    public abstract class LevelAnalytics
    {
        public abstract int Evaluate();

    }

    public class Maze3DLevelAnalytics : LevelAnalytics
    {
        private List<TurnAnalytics> _turns;

        public Maze3DLevelAnalytics()
        {
            _turns = new List<TurnAnalytics>();
        }

        public override int Evaluate()
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

    public class SimpleSpaceLevelAnalytics : LevelAnalytics
    {

        private List<MovementAnalytics> _movements;

        public SimpleSpaceLevelAnalytics()
        {
            _movements = new List<MovementAnalytics>();
        }

        public override int Evaluate()
        {
            List<MovementAnalytics> movementsDonePerfectly = _movements.Where(o => o.desiredMovement.Equals(o.performedMovement) && o.contractions == 1).ToList();

            if (movementsDonePerfectly.Count() > (_movements.Count() * 0.75))
            {
                //to increase the desired contraction length
                return 1;
            }
            else if (movementsDonePerfectly.Count() < (_movements.Count() * 0.5))
            {
                //to decrease the desired contraction length
                return -1;
            }

            return 0;
        }

        public void UpdateMovement(int movementNumber, int contractions, ActionMovement desiredMovement, ActionMovement performedMovement, long timestamp)
        {
            // Debug.WriteLine("novement num = " + movementNumber + " contractions " + contractions + " desired Movement = " + desiredMovement + " performed " + performedMovement);
            if (_movements.Count < movementNumber + 1) // new turn
            {
                _movements.Add(new MovementAnalytics() { movementNumber = movementNumber, contractions = contractions, desiredMovement = desiredMovement, performedMovement = performedMovement, startTime = timestamp });
            }
            else //update, i.e., another contraction and possible change of direction
            {
                MovementAnalytics movementAnalytics = _movements[movementNumber];
                movementAnalytics.contractions += contractions;
                movementAnalytics.desiredMovement = desiredMovement;
                movementAnalytics.performedMovement = performedMovement;
            }
        }
        public void UpdateMovementEnd(int movementNumber, long timestamp)
        {
            MovementAnalytics movementAnalytics = _movements[movementNumber];
            movementAnalytics.endTime = timestamp;
        }

        private class MovementAnalytics
        {
            internal int movementNumber;
            internal int contractions;
            internal ActionMovement desiredMovement;
            internal ActionMovement performedMovement;
            internal long startTime;
            internal long endTime;

            public MovementAnalytics() { }
        }
    }
}