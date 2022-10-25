using System.Collections.Generic;
using Godot;

namespace RandomDungeons
{
    public class HurtBox : Area2D
    {
        [Signal] public delegate void TookDamage(HitBox hitBox);

        [Export] public float RecoilDistance = 32;

        public bool IsInvulnerable => _cooldownTimer > 0;
        private float _cooldownTimer = 0;

        private HashSet<HitBox> _ignoredHitboxes = new HashSet<HitBox>();

        public override void _PhysicsProcess(float delta)
        {
            if (IsInvulnerable)
                _cooldownTimer -= delta;
        }

        public void TakeDamage(HitBox hitBox)
        {
            _cooldownTimer = hitBox.InvlunerabilityTime;
            EmitSignal(nameof(TookDamage), hitBox);
        }

        public void IgnoreHitBox(HitBox hitBox)
        {
            _ignoredHitboxes.Add(hitBox);
        }

        public bool IsIgnoring(HitBox hitBox)
        {
            return _ignoredHitboxes.Contains(hitBox);
        }

        public Vector2 GetRecoilVelocity(Node2D attacker, float friction)
        {
            Vector2 dir = (attacker.GlobalPosition - GlobalPosition).Normalized();

            return dir * AccelMath.SpeedNeededForDistance(
                RecoilDistance,
                friction
            );
        }
    }
}
