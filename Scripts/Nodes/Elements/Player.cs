using Godot;

using RandomDungeons.Services;

namespace RandomDungeons.Nodes.Elements
{
    public class Player : KinematicBody2D
    {
        public const float WalkSpeed = 200;

        public override void _Process(float deltaTime)
        {
            this.MoveAndSlide(InputService.LeftStick * WalkSpeed);
        }
    }
}
