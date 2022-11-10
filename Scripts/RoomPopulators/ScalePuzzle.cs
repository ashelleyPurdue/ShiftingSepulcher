using System;
using Godot;

namespace RandomDungeons
{
    public class ScalePuzzle : Node2D, IChallenge, IRoomPopulator
    {
        public void Populate(DungeonGraphRoom graphRoom, Random rng)
        {
        }

        public bool IsSolved()
        {
            return true;
        }
    }
}
