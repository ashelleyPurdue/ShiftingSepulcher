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
        [Export] public bool Enabled = true;

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

        public Vector2 GetKnockbackVelocity(Node2D victim, float friction)
        {
            Vector2 dir = (victim.GlobalPosition - GlobalPosition).Normalized();

            return dir * AccelMath.SpeedNeededForDistance(
                KnockbackDistance,
                friction
            );
        }

        public override void _PhysicsProcess(float delta)
        {
            Monitorable = Enabled;
            Monitoring = Enabled;
        }

        private void OnAreaEntered(Area2D other)
        {
            if (other is HurtBox hurtBox)
                OnHurtBoxEntered(hurtBox);
        }

        private void OnHurtBoxEntered(HurtBox hurtBox)
        {
            if (IsIgnored(hurtBox))
                return;

            if (IsIgnored(hurtBox.HealthPoints))
                return;

            if (!hurtBox.Enabled)
                return;

            hurtBox.FireHitBoxEntered(this);
            CallDeferred("emit_signal", nameof(DealtDamage), hurtBox);
            CallDeferred("emit_signal", nameof(DealtDamageNoParams));

            // We COULD just tell users to hook the HitBoxEntered signal up to
            // every HealthPointsComponent, but that would be tedious.
            hurtBox.HealthPoints?.OnHitBoxEntered(this);
        }

        private bool IsIgnored(HurtBox hurtBox)
        {
            return _ignoredHurtBoxes.Contains(hurtBox) || hurtBox.IsIgnoring(this);
        }

        private bool IsIgnored(HealthPointsComponent hp)
        {
            return hp != null && _ignoredHealthPoints.Contains(hp);
        }
    }
}
