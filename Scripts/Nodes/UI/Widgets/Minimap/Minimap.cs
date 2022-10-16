using System;
using System.Linq;
using Godot;
using RandomDungeons.Graphs;

namespace RandomDungeons.Nodes.UI.Widgets.Minimap
{
    [Tool]
    public class Minimap : Node2D
    {
        private const float RoomSize = 5 * 32;

        [Export] public PackedScene RoomDisplayPrefab;
        private Node2D _rooms => GetNode<Node2D>("%Rooms");

        public void SetGraph(DungeonGraph graph)
        {
            // Clear out old rooms
            foreach (var child in _rooms.GetChildren())
            {
                ((Node)child).QueueFree();
            }

            // Create new rooms
            foreach (var c in graph.AllRoomCoordinates())
            {
                var graphRoom = graph.GetRoom(c);
                var roomDisplay = RoomDisplayPrefab.Instance<RoomDisplay>();

                _rooms.AddChild(roomDisplay);
                roomDisplay.Position = RoomSize * new Vector2(c.x, -c.y);
                roomDisplay.SetGraphRoom(graphRoom);
            }

            // Keep the rooms centered
            var xValues = graph.AllRoomCoordinates().Select(c => c.x * RoomSize);
            var yValues = graph.AllRoomCoordinates().Select(c => c.y * RoomSize);

            Vector2 center = new Vector2(
                (xValues.Min() + xValues.Max()) / 2,
                -(yValues.Min() + yValues.Max()) / 2
            );
            _rooms.Position = -center;
        }
    }
}
