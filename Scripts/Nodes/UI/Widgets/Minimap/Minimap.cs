using Godot;
using RandomDungeons.Graphs;

namespace RandomDungeons.Nodes.UI.Widgets.Minimap
{
    public class Minimap : Node2D
    {
        private const float RoomSize = 6 * 32;

        [Export] public PackedScene RoomDisplayPrefab;

        public void SetGraph(DungeonGraph graph)
        {
            foreach (var child in GetChildren())
            {
                ((Node)child).QueueFree();
            }

            foreach (var c in graph.AllRoomCoordinates())
            {
                var graphRoom = graph.GetRoom(c);
                var roomDisplay = RoomDisplayPrefab.Instance<RoomDisplay>();

                AddChild(roomDisplay);
                roomDisplay.Position = RoomSize * new Vector2(c.x, c.y);
                roomDisplay.SetGraphRoom(graphRoom);
            }
        }
    }
}
