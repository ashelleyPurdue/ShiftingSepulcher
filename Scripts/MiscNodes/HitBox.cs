using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    public class HitBox : Area2D
    {
        [Signal] public delegate void DealtDamage(HurtBox hurtBox);
        [Signal] public delegate void DealtDamageNoParams();
        [Signal] public delegate void DealtDamageTo(HealthPointsComponent hp);

        [Export] public int Damage = 1;
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

        public void Ignore(HealthPointsComponent hp)
        {
            _ignoredHealthPoints.Add(hp);
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Monitoring)
            {
                foreach (var other in GetOverlappingAreas())
                {
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
            if (other.HasComponent<HealthPointsComponent>(out var hp))
            {
                if (IsIgnored(hp))
                    return;

                if (hp.IsInvulnerable)
                    return;

                hp.OnTookDamageFromHitBox(this);
                CallDeferred("emit_signal", nameof(DealtDamageTo), hp);
                CallDeferred("emit_signal", nameof(DealtDamageNoParams));

                return;
            }

            // Legacy: be backwards-compatible with the old hurtbox system
            if (other is HurtBox hurtBox)
            {
                if (IsIgnored(hurtBox))
                    return;

                hurtBox.TakeDamage(this);
                CallDeferred("emit_signal", nameof(DealtDamage), hurtBox);
                CallDeferred("emit_signal", nameof(DealtDamageNoParams));

                return;
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
