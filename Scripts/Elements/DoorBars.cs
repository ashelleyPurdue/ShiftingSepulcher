using Godot;

namespace RandomDungeons
{
    public class DoorBars : Node2D
    {
        private ChallengeDungeonGraphDoor _graphDoor;

        public override void _Process(float delta)
        {
            if (_graphDoor.IsOpened)
            {
                this.QueueFree();
            }
        }

        public void SetGraphDoor(ChallengeDungeonGraphDoor graphDoor)
        {
            _graphDoor = graphDoor;
        }
    }
}
