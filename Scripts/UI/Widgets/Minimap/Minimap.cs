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
        [Export] public int VisibleFloor
        {
            get => _visibleFloor;
            set
            {
                bool changed = _visibleFloor != value;
                _visibleFloor = value;

                if (changed && _layout != null)
                    Refresh();
            }
        }

        private Node2D _rooms => GetNode<Node2D>("%Rooms");

        private DungeonLayout _layout;
        private int _visibleFloor;

        public void SetLayout(DungeonLayout layout)
        {
            _layout = layout;
            VisibleFloor = 0;
            Refresh();
        }

        private void Refresh()
        {
            // Clear out old rooms
            foreach (var child in _rooms.EnumerateChildren())
            {
                ((Node)child).QueueFree();
            }

            // Create new rooms
            var roomsOnFloor = _layout
                .AllRooms()
                .Where(r => r.Position.z == _visibleFloor);

            foreach (var layoutRoom in roomsOnFloor)
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
            var xValues = _layout.AllRooms().Select(r => r.Position.x * RoomSize);
            var yValues = _layout.AllRooms().Select(r => r.Position.y * RoomSize);

            Vector2 center = new Vector2(
                (xValues.Min() + xValues.Max()) / 2,
                -(yValues.Min() + yValues.Max()) / 2
            );
            _rooms.Position = -center;
        }
    }
}
