using Godot;

namespace ShiftingSepulcher
{
    [CustomNode]
    public class SpawnTableEntry : Node
    {
        [Export] public int Weight = 1;
        [Export] public PackedScene Scene;
    }
}
