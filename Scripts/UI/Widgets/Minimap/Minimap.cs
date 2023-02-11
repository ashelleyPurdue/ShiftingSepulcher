using System;
using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    public class Minimap : Node2D
    {
        private const float RoomSize = 5 * 32;

        [Export] public PackedScene RoomDisplayPrefab;
        private Node2D _rooms => GetNode<Node2D>("%Rooms");

        public void SetLayout(DungeonLayout layout)
        {
            // Clear out old rooms
            foreach (var child in _rooms.EnumerateChildren())
            {
                ((Node)child).QueueFree();
            }

            // Create new rooms
            foreach (var layoutRoom in layout.AllRooms())
            {
                var roomDisplay = RoomDisplayPrefab.Instance<RoomDisplay>();

                _rooms.AddChild(roomDisplay);

                roomDisplay.Position = new Vector2(
                    layoutRoom.Position.x,
                    -layoutRoom.Position.y
                ) * RoomSize;

                roomDisplay.SetRoom(layoutRoom);
            }

            // Keep the rooms centered
            var xValues = layout.AllRooms().Select(r => r.Position.x * RoomSize);
            var yValues = layout.AllRooms().Select(r => r.Position.y * RoomSize);

            Vector2 center = new Vector2(
                (xValues.Min() + xValues.Max()) / 2,
                -(yValues.Min() + yValues.Max()) / 2
            );
            _rooms.Position = -center;
        }
    }
}
