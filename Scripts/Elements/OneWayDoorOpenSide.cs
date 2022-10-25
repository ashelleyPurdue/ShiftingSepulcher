using Godot;

namespace RandomDungeons
{
    public class OneWayDoorOpenSide : Node2D
    {
        private OneWayOpenSideGraphDoor _graphDoor;

        public void SetGraphDoor(OneWayOpenSideGraphDoor graphDoor)
        {
            _graphDoor = graphDoor;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_graphDoor.IsOpened)
                QueueFree();
        }

        public void OnUnlockTriggerBodyEnter(object body)
        {
            if (body is Player)
                _graphDoor.IsOpened = true;
        }
    }
}
