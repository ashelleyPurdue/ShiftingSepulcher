using Godot;
using RandomDungeons.Utils;
using RandomDungeons.Nodes.Components;

namespace RandomDungeons.Nodes.Elements
{
    public class PlayerSword : Node2D
    {
        public bool IsSwinging => _swingTimer > 0;
        public bool IsInHitStop => _hitStopTimer > 0;

        private const float SwingDuration = 0.15f;
        private const float SwingAngleDeg = 90;
        private float SwingSpeed => SwingAngleDeg / SwingDuration;

        private const float HitStopDuration = 0.15f;

        private HitBox _hitBox => GetNode<HitBox>("HitBox");

        private float _swingTimer = 0;
        private float _hitStopTimer = 0;

        public override void _PhysicsProcess(float delta)
        {
            Visible = IsSwinging;
            _hitBox.Monitoring  = IsSwinging;
            _hitBox.Monitorable = IsSwinging;

            if (!IsInHitStop)
            {
                RotationDegrees += SwingSpeed * delta;
                _swingTimer -= delta;
            }

            _hitStopTimer -= delta;
        }

        public void StartSwinging(EightDirection dir)
        {
            _swingTimer = SwingDuration;
            RotationDegrees = dir.ToAngleDeg() - (SwingAngleDeg / 2);
        }

        public void OnDealtDamage(HurtBox other)
        {
            if (!IsInHitStop)
                _hitStopTimer = HitStopDuration;
        }
    }
}
