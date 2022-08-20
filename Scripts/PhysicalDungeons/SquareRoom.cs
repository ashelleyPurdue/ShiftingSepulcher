using System;
using System.Linq;
using Godot;
using RandomDungeons.DungeonGraphs;

namespace RandomDungeons.PhysicalDungeons
{
    public class SquareRoom : Node2D
    {
        public DungeonRoom GraphRoom
        {
            get => _graphRoom;
            set
            {
                _graphRoom = value;
                Refresh();
            }
        }
        private DungeonRoom _graphRoom;

        private void Refresh()
        {
            this.Position = new Vector2(_graphRoom.Position.X, -_graphRoom.Position.Y) * 512;

            GetNode<SquareRoomDoor>("%NorthDoor").GraphDoor = _graphRoom.NorthDoor;
            GetNode<SquareRoomDoor>("%SouthDoor").GraphDoor = _graphRoom.SouthDoor;
            GetNode<SquareRoomDoor>("%EastDoor").GraphDoor = _graphRoom.EastDoor;
            GetNode<SquareRoomDoor>("%WestDoor").GraphDoor = _graphRoom.WestDoor;

            GetNode<Label>("%KeyLabel").Text = _graphRoom.HasKey
                ? $"Key {_graphRoom.KeyId}"
                : "";

            // Highlight this room if it's the starting room
            if (GraphRoom.Position.Equals(RoomCoordinates.Origin))
            {
                GetNode<Label>("%KeyLabel").Text += "*";
            }
        }
    }
}






