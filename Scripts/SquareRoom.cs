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

            GetNode<Door>("%NorthDoor").IsOpen = _graphRoom.NorthRoom != null;
            GetNode<Door>("%SouthDoor").IsOpen = _graphRoom.SouthRoom != null;
            GetNode<Door>("%EastDoor").IsOpen = _graphRoom.EastRoom != null;
            GetNode<Door>("%WestDoor").IsOpen = _graphRoom.WestRoom != null;
        }
    }

}
