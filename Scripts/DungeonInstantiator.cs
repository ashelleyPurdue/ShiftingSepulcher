using System.Collections.Generic;
using Godot;
using RandomDungeons.DungeonGraphs;

namespace RandomDungeons
{
    public class DungeonInstantiator : Node
    {
        public override void _Ready()
        {
            var roomPrefab = GD.Load<PackedScene>("res://Prefabs/SquareRoom.tscn");

            GD.Randomize();
            int seed = (int)GD.Randi();
            GD.Print(seed);

            var allRooms = DungeonGenerator.GenerateGraph(seed, 25);

            foreach (var coordinates in allRooms.Keys)
            {
                var graphRoom = allRooms[coordinates];
                var realRoom = roomPrefab.Instance<SquareRoom>();
                this.AddChild(realRoom);

                // Create the room at its location
                realRoom.Position = new Vector2(coordinates.X, -coordinates.Y) * 64;

                // Hide or show all the doors
                foreach (var dir in graphRoom.Doors.Keys)
                {
                    realRoom.GetDoor(dir).IsOpen = true;
                }
            }
        }
    }
}
