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

            // Spawn the key
            if (_graphRoom.HasKey)
            {
                var key = GD.Load<PackedScene>("res://Prefabs/Key.tscn")
                    .Instance<Key>();
                key.KeyId = _graphRoom.KeyId;
                key.Position = GetNode<Node2D>("%KeySpawn").Position;
                AddChild(key);
            }

            // Spawn the victory chest, if this is a boss room
            // TODO: Put a boss in here instead of a chest.  That'll teach 'em!
            if (_graphRoom.IsBossRoom)
            {
                var chest = GD.Load<PackedScene>("res://Prefabs/VictoryChest.tscn")
                    .Instance<VictoryChest>();
                chest.Position = GetNode<Node2D>("%KeySpawn").Position;
                AddChild(chest);
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






