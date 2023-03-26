using System.Collections.Generic;
using Godot;

namespace ShiftingSepulcher
{
    public class HurtBox : Area2D
    {
        [Signal] public delegate void HitBoxEntered(HitBox hitBox);

        [Export] public NodePath HealthPointsPath;
        [Export] public float RecoilDistance = 32;
        [Export] public bool Enabled = true;

        public HealthPointsComponent HealthPoints
        {
            get
            {
                // Fun fact: string.IsNullOrEmpty() throws a null reference
                // exception if you pass in a null NodePath.
                //
                // "You had ONE job, string.IsNullOrEmpty()!!!"
                //
                // For real though: the null ref exception happens because
                // Godot tries to implicitly cast NodePath to a string, but the
                // implicit cast operator wasn't written to expect nulls.
                if (HealthPointsPath == null || HealthPointsPath.IsEmpty())
                {
                    return null;
                }

                return GetNode<HealthPointsComponent>(HealthPointsPath);
            }
        }

        private HashSet<HitBox> _ignoredHitboxes = new HashSet<HitBox>();

        public void FireHitBoxEntered(HitBox hitBox)
        {
            EmitSignal(nameof(HitBoxEntered), hitBox);
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
