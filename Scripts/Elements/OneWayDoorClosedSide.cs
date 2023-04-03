using Godot;

namespace ShiftingSepulcher
{
    public class OneWayDoorClosedSide : Node2D
    {
        public OneWayDoorOpenSide OpenSide;

        public override void _PhysicsProcess(float delta)
        {
            if (OpenSide != null && OpenSide.IsOpened)
                QueueFree();
        }
    }
}
