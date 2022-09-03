using Godot;

using RandomDungeons.Services;
using RandomDungeons.Nodes.Components;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.Elements
{
    public class Player : KinematicBody2D
    {
        public const float WalkSpeed = 283;

        private EightDirectionalSprite _sprite => GetNode<EightDirectionalSprite>("%Sprite");
        private PlayerSword _sword => GetNode<PlayerSword>("%Sword");

        public override void _Process(float deltaTime)
        {
            SwordControls(deltaTime);
            WalkControls(deltaTime);
        }

        private void SwordControls(float deltaTime)
        {
            // TODO: use a separate "attack" button for this
            if (InputService.ActivatePressed && !_sword.IsSwinging)
                _sword.StartSwinging(_sprite.Direction);
        }

        private void WalkControls(float deltaTime)
        {
            // Move with the left stick
            var cappedLeftStick = CapMagnitude(InputService.LeftStick, 1);
            this.MoveAndSlide(cappedLeftStick * WalkSpeed);

            // Update the sprite
            if (cappedLeftStick.Length() > 0.01)
                _sprite.Direction = cappedLeftStick.ToNearestEightDirection();

            _sprite.SpeedScale = cappedLeftStick.Length();
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
