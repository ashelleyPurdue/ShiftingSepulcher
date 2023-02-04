using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class HitBox : Area2D
    {
        [Signal] public delegate void DealtDamage(HurtBox hurtBox);
        [Signal] public delegate void DealtDamageNoParams();
        [Signal] public delegate void DealtDamageTo(HealthPointsComponent hp);

        [Export] public int Damage = 1;
        [Export] public float InvlunerabilityTime = 1.5f;
        [Export] public float KnockbackDistance = 92.5f;

        [Export] public NodePath[] IgnoredHurtBoxes = new NodePath[] {};
        private HashSet<HurtBox> _ignoredHurtBoxes = new HashSet<HurtBox>();

        [Export] public NodePath[] IgnoredHealthPoints = new NodePath[] {};
        private HashSet<HealthPointsComponent> _ignoredHealthPoints = new HashSet<HealthPointsComponent>();


        public override void _Ready()
        {
            Connect("area_entered", this, nameof(OnAreaEntered));

            foreach (var path in IgnoredHurtBoxes)
            {
                _ignoredHurtBoxes.Add(GetNode<HurtBox>(path));
            }

            foreach (var path in IgnoredHealthPoints)
            {
                _ignoredHealthPoints.Add(GetNode<HealthPointsComponent>(path));
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
                    // Legacy: be backwards-compatible with the old hurtbox system
                    if (other is HurtBox hurtBox)
                    {
                        if (IsIgnored(hurtBox))
                            continue;

                        if (hurtBox.IsInvulnerable)
                            continue;

                        hurtBox.TakeDamage(this);
                        EmitSignal(nameof(DealtDamage), hurtBox);
                        EmitSignal(nameof(DealtDamageNoParams));

                        return;
                    }
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

        private void OnAreaEntered(Area2D other)
        {
            if (other is IComponent c && c.HasComponent<HealthPointsComponent>(out var hp))
            {
                if (IsIgnored(hp))
                    return;

                if (hp.IsInvulnerable)
                    return;

                hp.OnTookDamageFromHitBox(this);
                EmitSignal(nameof(DealtDamageTo), hp);
                EmitSignal(nameof(DealtDamageNoParams));
            }
        }

        private bool IsIgnored(HurtBox hurtBox)
        {
            return _ignoredHurtBoxes.Contains(hurtBox) || hurtBox.IsIgnoring(this);
        }

        private bool IsIgnored(HealthPointsComponent hp)
        {
            return _ignoredHealthPoints.Contains(hp);
        }
    }
}
