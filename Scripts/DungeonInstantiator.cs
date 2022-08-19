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

            var graph = DungeonGenerator.GenerateUsingRuns(
                seed: seed,
                minRunSize: 1,
                maxRunSize: 3,
                numRooms: 25
            );

            foreach (var coordinates in graph.AllRoomCoordinates())
            {
                var graphRoom = graph.GetRoom(coordinates);
                var realRoom = roomPrefab.Instance<SquareRoom>();
                this.AddChild(realRoom);

                // Create the room at its location
                realRoom.GraphRoom = graphRoom;
            }
        }
    }
}
