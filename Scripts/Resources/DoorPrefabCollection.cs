using Godot;

namespace ShiftingSepulcher
{
    public class DoorPrefabCollection : Resource
    {
        [Export] public PackedScene Wall;
        [Export] public PackedScene Lock;
        [Export] public PackedScene Warp;
        [Export] public PackedScene Bars;

        [Export] public PackedScene OneWayOpenSide;
        [Export] public PackedScene OneWayClosedSide;

        // Technically not a door.  Should probably rename this class.
        [Export] public PackedScene KeyChest;

        [Export] public PackedScene StairsUpModel;
        [Export] public PackedScene StairsDownModel;
    }
}
