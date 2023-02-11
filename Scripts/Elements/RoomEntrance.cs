using Godot;

namespace ShiftingSepulcher
{
    [CustomNode(parent: "Position2D", icon: "Position2D")]
    public class RoomEntrance : Position2D, IRoomEntrance
    {
        public Node2D Node => this;
        public string EntranceName => Name;
    }
}
