using System;
using Godot;
using RandomDungeons.DungeonGraphs;

namespace RandomDungeons
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
            this.Position = new Vector2(_graphRoom.Position.X, -_graphRoom.Position.Y) * 64;

            GetNode<Door>("%NorthDoor").GraphDoor = _graphRoom.NorthDoor;
            GetNode<Door>("%SouthDoor").GraphDoor = _graphRoom.SouthDoor;
            GetNode<Door>("%EastDoor").GraphDoor = _graphRoom.EastDoor;
            GetNode<Door>("%WestDoor").GraphDoor = _graphRoom.WestDoor;
        }
    }

}
