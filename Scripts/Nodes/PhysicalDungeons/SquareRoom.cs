using System;
using System.Linq;
using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.Nodes.Elements;

namespace RandomDungeons.PhysicalDungeons
{
    public class SquareRoom : Node2D
    {
        [Export] public PackedScene DoorWallPrefab;
        [Export] public PackedScene DoorLockPrefab;
        [Export] public PackedScene DoorWarpPrefab;

        private DungeonGraphRoom _graphRoom;

        private Node2D _northDoorSpawn => GetNode<Node2D>("%NorthDoorSpawn");
        private Node2D _southDoorSpawn => GetNode<Node2D>("%SouthDoorSpawn");
        private Node2D _eastDoorSpawn => GetNode<Node2D>("%EastDoorSpawn");
        private Node2D _westDoorSpawn => GetNode<Node2D>("%WestDoorSpawn");

        public float FadePercent {get; set;}

        public override void _Process(float deltaTime)
        {
            // Adjust the fade curtain's transparency
            var curtain = GetNode<Polygon2D>("%FadeCurtain");

            curtain.Visible = FadePercent > 0;
            curtain.Modulate = GetBackgroundColor(1 - FadePercent);
        }

        public void SetGraphRoom(DungeonGraphRoom graphRoom)
        {
            _graphRoom = graphRoom;

            this.Position = new Vector2(_graphRoom.Position.x, -_graphRoom.Position.y) * 512;

            // Fill in all the door slots
            SetDoor(_northDoorSpawn, _graphRoom.NorthDoor);
            SetDoor(_southDoorSpawn, _graphRoom.SouthDoor);
            SetDoor(_eastDoorSpawn, _graphRoom.EastDoor);
            SetDoor(_westDoorSpawn, _graphRoom.WestDoor);

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

        private void SetDoor(Node2D spawn, DungeonGraphDoor graphDoor)
        {
            // If the door doesn't go anywhere, just put a wall here.
            if (graphDoor.Destination == null)
            {
                Create<Node2D>(spawn, DoorWallPrefab);
                return;
            }

            var warp = Create<DoorWarp>(spawn, DoorWarpPrefab);
            warp.SetGraphDoor(graphDoor);

            // Spawn a lock, if the door is locked
            if (graphDoor.IsLocked)
            {
                var doorLock = Create<DoorLock>(spawn, DoorLockPrefab);
                doorLock.KeyId = graphDoor.LockId;
            }
        }

        private T Create<T>(Node2D parent, PackedScene prefab) where T : Node2D
        {
            var node = prefab.Instance<T>();
            parent.AddChild(node);
            return node;
        }

        private Color GetBackgroundColor(float alpha)
        {
            var c = (Color)ProjectSettings.GetSetting("rendering/environment/default_clear_color");
            c.a = alpha;

            return c;
        }
    }
}






