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

        public float FadePercent {get; set;}

        public override void _Process(float deltaTime)
        {
            // Adjust the fade curtain's transparency
            var curtain = GetNode<Polygon2D>("%FadeCurtain");

            curtain.Visible = FadePercent > 0;
            curtain.Modulate = GetBackgroundColor(1 - FadePercent);
        }

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

        private Color GetBackgroundColor(float alpha)
        {
            var c = (Color)ProjectSettings.GetSetting("rendering/environment/default_clear_color");
            c.a = alpha;

            return c;
        }
    }
}






