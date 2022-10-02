using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

using RandomDungeons.Utils;

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

            return dir * AccelMath.SpeedNeededForDistance(
                KnockbackDistance,
                friction
            );
        }

        private bool IsIgnored(HurtBox hurtBox)
        {
            return _ignoredHurtBoxes.Contains(hurtBox) || hurtBox.IsIgnoring(this);
        }
    }
}
