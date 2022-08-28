using Godot;

namespace RandomDungeons.Nodes.Elements
{
    public class Player : KinematicBody2D
    {
        public const float WalkSpeed = 200;

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
