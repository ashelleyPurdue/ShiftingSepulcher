using Godot;

namespace RandomDungeons
{
    public class Player : KinematicBody2D
    {
        public const float WalkSpeed = 100;

        public override void _Process(float deltaTime)
        {
            var leftStick = new Vector2(
                Input.GetAxis("MoveLeft", "MoveRight"),
                Input.GetAxis("MoveUp", "MoveDown")
            );

            this.MoveAndSlide(leftStick * WalkSpeed);
        }
    }
}
