using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class ScalePuzzle : Node2D, IChallenge, IRoomPopulator
    {
        [Export] public int MinWeightSize = 1;
        [Export] public int MaxWeightSize = 3;
        [Export] public int MinWeightCount = 3;
        [Export] public int MaxWeightCount = 5;
        [Export] public PackedScene CarryableWeightPrefab;

        private Area2D _leftZone => GetNode<Area2D>("%LeftScaleZone");
        private Area2D _rightZone => GetNode<Area2D>("%RightScaleZone");

        private List<CarryableWeights> _weights = new List<CarryableWeights>();

        public void Populate(DungeonGraphRoom graphRoom, Random rng)
        {
            var leftSide = new List<int>();
            var rightSide = new List<int>();

            int weightCount = rng.Next(MinWeightCount, MaxWeightCount + 1);

            // Keep adding randomly-sized weights to the lightest side
            for (int i = 0; i < weightCount - 1; i++)
            {
                int weight = rng.Next(MinWeightSize, MaxWeightSize + 1);
                var side = LightestSideOrCoinFlip();
                side.Add(weight);
            }

            // Equalize them
            int diff = Math.Abs(leftSide.Sum() - rightSide.Sum());
            if (diff != 0)
                LightestSideOrCoinFlip().Add(diff);

            // Create the real weights
            foreach (int weight in leftSide.Concat(rightSide))
            {
                var weightObj = CarryableWeightPrefab.Instance<CarryableWeights>();
                weightObj.NumWeights = weight;

                AddChild(weightObj);
                _weights.Add(weightObj);
            }

            List<int> LightestSideOrCoinFlip()
            {
                int leftWeight = leftSide.Sum(w => w);
                int rightWeight = rightSide.Sum(w => w);

                if (leftWeight == rightWeight)
                    return rng.PickFrom(new[] {leftSide, rightSide});

                if (leftWeight < rightWeight)
                    return leftSide;
                else
                    return rightSide;
            }
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
                .Cast<Area2D>()
                .Select(a => a.FindAncestor<CarryableWeights>())
                .Where(w => w != null)
                .Sum(w => w.NumWeights);
        }
    }
}
