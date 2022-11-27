using System;
using Godot;

namespace RandomDungeons
{
    public class BombWitch : Node
    {
        [Export] public float TeleportRadius = 6 * 32;
        [Export] public bool RotateTowardPlayer = false;

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private Node2D _body => GetParent<Node2D>();

        public override void _PhysicsProcess(float delta)
        {
            var player = GetTree().FindPlayer();

            // Rotate toward the player while aiming
            if (RotateTowardPlayer && player != null)
            {
                float targetRotRad = _body
                    .GlobalPosition
                    .AngleToPoint(player.GlobalPosition);

                targetRotRad -= Mathf.Deg2Rad(90 + 180);

                _body.Rotation = Mathf.LerpAngle(
                    _body.Rotation,
                    targetRotRad,
                    0.125f
                );
            }
        }

        public void ResetAnimator()
        {
            _animator.Reset();
        }

        public void TeleportToRandomSpot()
        {
            float angle = GD.Randf() * Mathf.Deg2Rad(360);
            float radius = GD.Randf() * TeleportRadius;

            _body.Position = new Vector2(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle)
            );
        }
    }
}
