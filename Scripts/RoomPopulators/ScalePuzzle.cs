using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class ScalePuzzle : Node2D, IChallenge, IRoomPopulator
    {
        private Area2D _leftZone => GetNode<Area2D>("%LeftScaleZone");
        private Area2D _rightZone => GetNode<Area2D>("%RightScaleZone");

        private IEnumerable<CarryableWeights> _weights;

        public void Populate(DungeonGraphRoom graphRoom, Random rng)
        {
        }

        public bool IsSolved()
        {
            int totalWeight = _weights.Sum(w => w.NumWeights);
            int leftWeights = TotalWeightIn(_leftZone);
            int rightWeights = TotalWeightIn(_rightZone);

            bool sidesAreEqual = leftWeights == rightWeights;
            bool allWeightsAreUsed = leftWeights + rightWeights == totalWeight;

            return sidesAreEqual && allWeightsAreUsed;
        }

        private int TotalWeightIn(Area2D zone)
        {
            return zone
                .GetOverlappingAreas()
                .OfType<CarryableWeights>()
                .Sum(w => w.NumWeights);
        }
    }
}
