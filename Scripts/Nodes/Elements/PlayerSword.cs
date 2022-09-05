using Godot;
using RandomDungeons.Utils;
using RandomDungeons.Nodes.Components;

namespace RandomDungeons.Nodes.Elements
{
    public class PlayerSword : Node2D
    {
        public bool IsSwinging => _swingTimer > 0;

        private const float SwingDuration = 0.15f;
        private const float SwingAngleDeg = 90;
        private float SwingSpeed => SwingAngleDeg / SwingDuration;

        private const float KnockbackSpeed = 300;

        private HitBox _hitBox => GetNode<HitBox>("HitBox");

        private float _swingTimer = 0;

        public override void _PhysicsProcess(float delta)
        {
            _swingTimer -= delta;
            RotationDegrees += SwingSpeed * delta;

            Visible = IsSwinging;
            _hitBox.Monitoring  = IsSwinging;
            _hitBox.Monitorable = IsSwinging;
            _hitBox.KnockbackVelocity = KnockbackSpeed * new Vector2(
                Mathf.Cos(Rotation),
                Mathf.Sin(Rotation)
            );
        }

        public void StartSwinging(EightDirection dir)
        {
            _swingTimer = SwingDuration;
            RotationDegrees = dir.ToAngleDeg() - (SwingAngleDeg / 2);
        }
    }
}
