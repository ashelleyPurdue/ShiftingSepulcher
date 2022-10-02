using System.Collections.Generic;
using Godot;

namespace RandomDungeons.Nodes.Components
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
            return dir * CalculateRecoilSpeed(RecoilDistance, friction);
        }

        /// <summary>
        /// Returns the speed that would be required to send an object a given
        /// distance with a given amount of friction, using a timestep of 1/60.
        ///
        /// The answer is only an estimate, because some combinations of
        /// distances/frictions are impossible to hit exactly with a fixed
        /// timestep.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="friction"></param>
        /// <returns></returns>
        private static float CalculateRecoilSpeed(float distance, float friction)
        {
            float tolerance = 0.1f;
            float speed = 0;
            float predictedDist = 0;

            while(Mathf.Abs(predictedDist - distance) > tolerance)
            {
                speed += tolerance;
                predictedDist = CalculateDistance(speed);
            }
            return speed;

            float CalculateDistance(float s)
            {
                float delta = 1f / 60;
                float v = s;
                float d = 0;

                while (v > 0)
                {
                    d += v * delta;
                    v = Mathf.MoveToward(v, 0, friction * delta);
                }

                return d;
            }
        }
    }
}
