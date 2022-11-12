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

        private Area2D _leftZone => GetNode<Area2D>("%LeftScaleZone");
        private Area2D _rightZone => GetNode<Area2D>("%RightScaleZone");

        private Label _leftSideLabel => GetNode<Label>("%LeftSideLabel");
        private Label _rightSideLabel => GetNode<Label>("%RightSideLabel");

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

        public override void _Process(float delta)
        {
            _leftSideLabel.Text = "" + TotalWeightIn(_leftZone);
            _rightSideLabel.Text = "" + TotalWeightIn(_rightZone);
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

        private Node2D[] ShuffledSpawnPoints(Random rng)
        {
            var spawnPoints = _weightSpawnPoints
                .GetChildren()
                .Cast<Node2D>();

            return rng.Shuffle(spawnPoints);
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
