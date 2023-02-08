using System.Collections.Generic;
using Godot;

namespace RandomDungeons
{
    public class HurtBox : Area2D
    {
        [Signal] public delegate void TookDamage(HitBox hitBox);
        [Signal] public delegate void TookDamageNoParams();

        [Export] public float RecoilDistance = 32;


        private HashSet<HitBox> _ignoredHitboxes = new HashSet<HitBox>();

        public void TakeDamage(HitBox hitBox)
        {
            EmitSignal(nameof(TookDamage), hitBox);
            EmitSignal(nameof(TookDamageNoParams));
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
