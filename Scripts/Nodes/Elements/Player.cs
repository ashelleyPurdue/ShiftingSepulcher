using Godot;

using RandomDungeons.Services;

namespace RandomDungeons.Nodes.Elements
{
    public class Player : KinematicBody2D
    {
        public const float WalkSpeed = 283;

        public override void _Process(float deltaTime)
        {
            var cappedLeftStick = CapMagnitude(InputService.LeftStick, 1);
            this.MoveAndSlide(cappedLeftStick * WalkSpeed);
        }

        private Vector2 CapMagnitude(Vector2 v, float maxMagnitude)
        {
            float magnitude = v.Length();

            if (magnitude > maxMagnitude)
                magnitude = maxMagnitude;

            return v.Normalized() * magnitude;
        }
    }
}
