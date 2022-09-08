using System;
using System.Linq;
using Godot;

namespace RandomDungeons.Nodes.Components
{
    public class HitBox : Area2D
    {
        [Signal] public delegate void DealtDamage(HurtBox hurtBox);

        [Export] public int Damage = 1;
        [Export] public float InvlunerabilityTime = 0.5f;
        [Export] public float KnockbackSpeed = 300;

        [Export] public NodePath[] IgnoredHurtBoxes = new NodePath[] {};

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

        public Vector2 GetKnockbackVelocity(Node2D victim)
        {
            Vector2 dir = (victim.GlobalPosition - GlobalPosition).Normalized();
            return dir * KnockbackSpeed;
        }

        private bool IsIgnored(HurtBox hurtBox)
        {
            return IgnoredHurtBoxes.Any(p => GetNode(p) == hurtBox);
        }
    }
}
