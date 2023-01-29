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

        public override void _PhysicsProcess(float delta)
        {
            var overlappingWeights = _zone
                .GetOverlappingAreas()
                .Cast<Area2D>()
                .Select(a => a.FindAncestor<HoldableWeights>())
                .Where(w => w != null)
                .Where(w => !w.IsBeingHeld)
                .Where(w => !w.IsFlying)
                .ToArray();

            // Update the total weight
            TotalWeight = overlappingWeights.Sum(w => w.NumWeights);

            // Attach all overlapping weights to the bowl, so they move as the
            // bowl moves.
            while (_attachedWeightContainer.GetChildCount() > 0)
            {
                var weight = _attachedWeightContainer.GetChild<Node2D>(0);
                var pos = weight.GlobalPosition;

                _attachedWeightContainer.RemoveChild(weight);
                this.GetRoom().AddChild(weight);
                weight.GlobalPosition = pos;
            }

            foreach (var weight in overlappingWeights)
            {
                var pos = weight.GlobalPosition;

                weight.GetParent().RemoveChild(weight);
                _attachedWeightContainer.AddChild(weight);

                weight.GlobalPosition = pos;
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
