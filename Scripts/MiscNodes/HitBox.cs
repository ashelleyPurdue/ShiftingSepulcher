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

        private HashSet<HealthPointsComponent> _previouslyOverlappingHealthPoints = new HashSet<HealthPointsComponent>();
        private HashSet<HurtBox> _previouslyOverlappingHurtBoxes = new HashSet<HurtBox>();

        public override void _Ready()
        {
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
            // Detect areas that just started overlapping this frame,
            // or that are already overlapping on the same frame we became
            // enabled
            if (!Enabled)
            {
                _previouslyOverlappingHealthPoints.Clear();
                _previouslyOverlappingHurtBoxes.Clear();
                return;
            }

            var currentlyOverlappingAreas = GetOverlappingAreas().Cast<Area2D>();
            DetectNewlyOverlappingHealthPoints(currentlyOverlappingAreas);
            DetectNewlyOverlappingHurtBoxes(currentlyOverlappingAreas);
        }

        private void DetectNewlyOverlappingHealthPoints(IEnumerable<Area2D> currentlyOverlappingAreas)
        {
            var currentlyOverlappingHealthPoints = currentlyOverlappingAreas
                .Select(a => a.GetComponent<HealthPointsComponent>())
                .Where(hp => hp != null)
                .Where(hp => !hp.IsInvulnerable);

            var newlyOverlappingHealthPoints = currentlyOverlappingHealthPoints
                .Where(hp => !_previouslyOverlappingHealthPoints.Contains(hp));

            foreach (var hp in newlyOverlappingHealthPoints)
            {
                OnHealthPointsComponentEntered(hp);
            }

            _previouslyOverlappingHealthPoints.Clear();
            foreach (var hp in currentlyOverlappingHealthPoints)
            {
                _previouslyOverlappingHealthPoints.Add(hp);
            }
        }

        private void DetectNewlyOverlappingHurtBoxes(IEnumerable<Area2D> currentlyOverlappingAreas)
        {
            var currentlyOverlappingHurtBoxes = currentlyOverlappingAreas
                .OfType<HurtBox>()
                .Where(hb => hb.Enabled);

            var newlyOverlappingHurtBoxes = currentlyOverlappingHurtBoxes
                .Where(hb => !_previouslyOverlappingHurtBoxes.Contains(hb));

            foreach (var hurtBox in newlyOverlappingHurtBoxes)
            {
                OnHurtBoxEntered(hurtBox);
            }

            _previouslyOverlappingHurtBoxes.Clear();
            foreach (var hb in currentlyOverlappingHurtBoxes)
            {
                _previouslyOverlappingHurtBoxes.Add(hb);
            }
        }

        private void OnHealthPointsComponentEntered(HealthPointsComponent hp)
        {
            if (IsIgnored(hp))
                return;

            hp.OnTookDamageFromHitBox(this);
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
