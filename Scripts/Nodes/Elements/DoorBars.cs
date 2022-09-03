using Godot;
using RandomDungeons.PhysicalDungeons;

namespace RandomDungeons.Nodes.Elements
{
    public class DoorBars : Node2D
    {
        public IDungeonRoomChallenge Challenge;

        public override void _Process(float delta)
        {
            bool solved = Challenge?.IsSolved() ?? false;

            if (solved)
            {
                this.QueueFree();
            }
        }
    }
}
