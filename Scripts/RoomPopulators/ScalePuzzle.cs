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

        private Node2D _weightSpawnPoints => GetNode<Node2D>("%WeightSpawnPoints");

        private ScaleBowl _leftBowl => GetNode<ScaleBowl>("%LeftBowl");
        private ScaleBowl _rightBowl => GetNode<ScaleBowl>("%RightBowl");

        private List<CarryableWeights> _weights = new List<CarryableWeights>();

        public void Populate(DungeonGraphRoom graphRoom, Random rng)
        {
            var leftSide = new List<int>();
            var rightSide = new List<int>();

            int weightCount = rng.Next(MinWeightCount, MaxWeightCount + 1);

            // Keep adding randomly-sized weights to the lightest side
            int lastChosenWeight = -1;
            for (int i = 0; i < weightCount - 1; i++)
            {
                // Don't choose the same weight twice in a row, to prevent the
                // solutions from becoming _too_ trivial
                int weight;
                do
                {
                    weight = rng.Next(MinWeightSize, MaxWeightSize + 1);
                }
                while (weight == lastChosenWeight);
                lastChosenWeight = weight;

                var side = LightestSideOrCoinFlip();
                side.Add(weight);
            }

            // Equalize them
            int diff = Math.Abs(leftSide.Sum() - rightSide.Sum());
            if (diff != 0)
                LightestSideOrCoinFlip().Add(diff);

            // Create the real weights, and give them random spawn points
            var allWeights = leftSide.Concat(rightSide).ToArray();
            Node2D[] spawnPoints = ShuffledSpawnPoints(rng);

            for (int i = 0; i < allWeights.Length; i++)
            {
                var weightObj = CarryableWeightPrefab.Instance<CarryableWeights>();
                weightObj.NumWeights = allWeights[i];
                _weights.Add(weightObj);

                spawnPoints[i].AddChild(weightObj);
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
            int leftWeights = _leftBowl.TotalWeight;
            int rightWeights = _rightBowl.TotalWeight;

            bool sidesAreEqual = leftWeights == rightWeights;
            bool allWeightsAreUsed = leftWeights + rightWeights == totalWeight;

            return sidesAreEqual && allWeightsAreUsed;
        }

        private Node2D[] ShuffledSpawnPoints(Random rng)
        {
            var spawnPoints = _weightSpawnPoints
                .GetChildren()
                .Cast<Node2D>();

            return rng.Shuffle(spawnPoints);
        }
    }
}
