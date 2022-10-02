using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons.Nodes.Components
{
    public class HitBox : Area2D
    {
        [Signal] public delegate void DealtDamage(HurtBox hurtBox);

        [Export] public int Damage = 1;
        [Export] public float InvlunerabilityTime = 0.5f;
        [Export] public float KnockbackDistance = 92.5f;

        [Export] public NodePath[] IgnoredHurtBoxes = new NodePath[] {};
        private HashSet<HurtBox> _ignoredHurtBoxes = new HashSet<HurtBox>();

        public override void _Ready()
        {
            foreach (var path in IgnoredHurtBoxes)
            {
                _ignoredHurtBoxes.Add(GetNode<HurtBox>(path));
            }
        }

        public void IgnoreHurtBox(HurtBox hurtBox)
        {
            _ignoredHurtBoxes.Add(hurtBox);
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Monitoring)
            {
                foreach (var other in GetOverlappingAreas())
                {
                    if (!(other is HurtBox hurtBox))
                        continue;

                    if (IsIgnored(hurtBox))
                        continue;

                    if (hurtBox.IsInvulnerable)
                        continue;

                    hurtBox.TakeDamage(this);
                    EmitSignal(nameof(DealtDamage), hurtBox);
                }
            }
        }

        public Vector2 GetKnockbackVelocity(Node2D victim, float friction)
        {
            Vector2 dir = (victim.GlobalPosition - GlobalPosition).Normalized();
            return dir * CalculateKnockbackSpeed(KnockbackDistance, friction);
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
        private static float CalculateKnockbackSpeed(float distance, float friction)
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

        private bool IsIgnored(HurtBox hurtBox)
        {
            return _ignoredHurtBoxes.Contains(hurtBox) || hurtBox.IsIgnoring(this);
        }
    }
}
