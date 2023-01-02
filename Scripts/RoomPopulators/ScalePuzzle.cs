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
        [Export] public PackedScene HoldableWeightPrefab;

        private Node2D _leftWeightSpawnPoints => GetNode<Node2D>("%WeightSpawnPoints/Left");
        private Node2D _rightWeightSpawnPoints => GetNode<Node2D>("%WeightSpawnPoints/Right");
        private Node2D _middleWeightSpawnPoints => GetNode<Node2D>("%WeightSpawnPoints/Middle");

        private ScaleBowl _leftBowl => GetNode<ScaleBowl>("%LeftBowl");
        private ScaleBowl _rightBowl => GetNode<ScaleBowl>("%RightBowl");

        private List<HoldableWeights> _weights = new List<HoldableWeights>();

        public void Populate(DungeonTreeRoom treeRoom, Random rng)
        {
            var leftSide = new List<int>();
            var rightSide = new List<int>();
            var middleSide = new List<int>();

            int weightCount = rng.Next(MinWeightCount, MaxWeightCount + 1);

            // Keep adding randomly-sized weights to the lightest side
            for (int i = 0; i < weightCount - 1; i++)
            {
                // Never choose a size that would make both sides equal, because
                // that would risk the "equalizer" needing to be 0.
                int forbiddenWeight = Math.Abs(leftSide.Sum() - rightSide.Sum());
                int weight;
                do
                {
                    weight = rng.Next(MinWeightSize, MaxWeightSize + 1);
                }
                while (weight == forbiddenWeight);

                LightestSideOrCoinFlip().Add(weight);
            }

            // Equalize them
            int equalizer = Math.Abs(leftSide.Sum() - rightSide.Sum());
            LightestSideOrCoinFlip().Add(equalizer);

            // Randomly scramble the weights, to hide the solution
            var sides = new List<int>[]
            {
                leftSide,
                rightSide,
                middleSide
            };
            while (leftSide.Sum() == rightSide.Sum())
            {
                int[] allWeights = leftSide
                    .Concat(rightSide)
                    .Concat(middleSide)
                    .ToArray();

                leftSide.Clear();
                rightSide.Clear();
                middleSide.Clear();

                foreach (int weight in allWeights)
                {
                    var side = rng.PickFrom(sides);
                    side.Add(weight);
                }
            }

            // Create the real weights
            SpawnRealWeights(leftSide, _leftWeightSpawnPoints);
            SpawnRealWeights(rightSide, _rightWeightSpawnPoints);
            SpawnRealWeights(middleSide, _middleWeightSpawnPoints);

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

            void SpawnRealWeights(List<int> weights, Node2D spawnPointsParent)
            {
                var spawnPoints = spawnPointsParent
                    .GetChildren()
                    .Cast<Node2D>();

                Node2D[] shuffledSpawnPoints = rng.Shuffle(spawnPoints);

                for (int i = 0; i < weights.Count; i++)
                {
                    var weightObj = HoldableWeightPrefab.Instance<HoldableWeights>();
                    weightObj.NumWeights = weights[i];
                    _weights.Add(weightObj);

                    shuffledSpawnPoints[i].AddChild(weightObj);
                }
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
    }
}
