using System.Collections.Generic;
using Godot;
using RandomDungeons.DungeonGraphs;

namespace RandomDungeons
{
    public class DungeonInstantiator : Node
    {
        public override void _Ready()
        {
            var wallsPrefab = GD.Load<PackedScene>("res://Prefabs/SquareRoomWalls.tscn");

            GD.Randomize();
            int seed = (int)GD.Randi();
            GD.Print(seed);

            var allRooms = DungeonGenerator.GenerateGraph((int)GD.Randi(), 25);

            foreach (var coordinates in allRooms.Keys)
            {
                var node = wallsPrefab.Instance<Node2D>();
                node.Position = new Vector2(coordinates.X, coordinates.Y) * 64;
                this.AddChild(node);
            }
        }
    }
}
