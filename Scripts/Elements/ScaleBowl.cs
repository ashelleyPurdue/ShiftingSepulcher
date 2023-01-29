using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class ScaleBowl : Node2D
    {
        private const int WeightForMaxDisplacement = 5;
        private const float MaxDisplacement = 2 * 32;

        [Export] public NodePath PartnerBowl;

        public int TotalWeight {get; private set;} = 0;

        private ScaleBowl _partner => GetNode<ScaleBowl>(PartnerBowl);
        private Area2D _zone => GetNode<Area2D>("%WeightZone");
        private Node2D _moveablePart => GetNode<Node2D>("%MoveablePart");
        private Node2D _attachedWeightContainer => GetNode<Node2D>("%AttachedWeightContainer");

        private Vector2? _prevPos;

        public override void _PhysicsProcess(float delta)
        {
            // Update the total weight
            var overlappingWeights = _zone
                .GetOverlappingAreas()
                .Cast<Area2D>()
                .Select(a => a.FindAncestor<HoldableWeights>())
                .Where(w => w != null)
                .Where(w => !w.IsBeingHeld)
                .Where(w => !w.IsFlying)
                .ToArray();

            TotalWeight = overlappingWeights.Sum(w => w.NumWeights);

            // Push all overlapping weights with the bowl, as if they were
            // resting inside it.
            if (!_prevPos.HasValue)
            {
                _prevPos = _moveablePart.Position;
                return;
            }
            Vector2 deltaPos = _moveablePart.Position - _prevPos.Value;
            _prevPos = _moveablePart.Position;

            foreach (var weight in overlappingWeights)
            {
                weight.Position += deltaPos;
            }
        }

        public override void _Process(float delta)
        {
            // Change height based on how heavy this bowl is compared to its
            // partner
            int weightDiff = TotalWeight - _partner.TotalWeight;
            float displacement = MaxDisplacement * ((float)weightDiff) / WeightForMaxDisplacement;
            displacement = Mathf.Clamp(displacement, -MaxDisplacement, MaxDisplacement);

            var targetPos = new Vector2(0, displacement);

            float distToTarget = targetPos.DistanceTo(_moveablePart.Position);
            float speed = 10 + (distToTarget / 2);
            _moveablePart.Position = _moveablePart.Position.MoveToward(targetPos, speed * delta);
        }
    }
}
