using System;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class ScalePuzzle : Node2D, IChallenge, IRoomPopulator
    {
        private Area2D _leftZone => GetNode<Area2D>("%LeftScaleZone");
        private Area2D _rightZone => GetNode<Area2D>("%RightScaleZone");

        public void Populate(DungeonGraphRoom graphRoom, Random rng)
        {
        }

        public bool IsSolved()
        {
            return true;
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
