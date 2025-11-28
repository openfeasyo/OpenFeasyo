using Microsoft.Xna.Framework;


namespace GhostlyLib.Elements.Character
{
    public interface IGameCharacter : IDrawable
    {
        int CurrentHealth { get; protected set; }
        int Height { get; protected set; }
        int Width { get; protected set; }
        int Score { get; set; }
        float OriginalRotation { get; set; } // how the character was rotated at the start of the game / before rotation on turns started
        float CameraOriginalRotation { get; set; }
        float TargetRotation { get; set; }
        float CurrentRotation { get; set; } // used in character sprite drawing, i.e., rotation of the original image
        Direction Direction { get; set; }
        TurningDirection TurningDirection { get; set; }
        double SpeedX { get; protected set; }
        double SpeedY { get; protected set; }
        Rectangle Top { get; protected set; }
        Rectangle Bottom { get; protected set; }
        Rectangle LeftSide { get; protected set; }
        Rectangle RightSide { get; protected set; }
        Rectangle MainBody { get; protected set; }
        Rectangle Center { get; protected set; }
        //Movement Movement { get; protected set; }
        RotationDirection RotationDirection { get; protected set; }
        RotationStatus RotationStatus { get; protected set; }
        CharacterLiveState LiveState { get; protected set; }

        Instruction Instruction { get; protected set; }

        public ActionMovement ActionMovement { get; protected set; }
        public AutomaticMovement AutomaticMovement { get; protected set; }

        void Hit();
        void Die();
        void Stop();
    }
}
