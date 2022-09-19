using Godot;

namespace RandomDungeons.Resources
{
    public class DoorPrefabCollection : Resource
    {
        [Export] public PackedScene Wall;
        [Export] public PackedScene Lock;
        [Export] public PackedScene Warp;
        [Export] public PackedScene Bars;

        [Export] public PackedScene OneWayOpenSide;
        [Export] public PackedScene OneWayClosedSide;
    }
}