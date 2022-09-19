using Godot;
using RandomDungeons.Graphs;

namespace RandomDungeons.Nodes.Elements
{
    public class OneWayDoorClosedSide : Node2D
    {
        private OneWayClosedSideGraphDoor _graphDoor;

        public void SetGraphDoor(OneWayClosedSideGraphDoor graphDoor)
        {
            _graphDoor = graphDoor;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_graphDoor.IsOpened)
                QueueFree();
        }
    }
}
