using System;
using System.Linq;
using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.Nodes.Elements;
using RandomDungeons.Utils;
using RandomDungeons.PhysicalDungeons;

namespace RandomDungeons.Nodes.DungeonRooms
{
    public class SimpleDungeonRoom : IDungeonRoom
    {
        public override event Action<CardinalDirection> DoorUsed;

        [Export] public PackedScene DoorWallPrefab;
        [Export] public PackedScene DoorLockPrefab;
        [Export] public PackedScene DoorWarpPrefab;

        public override DungeonGraphRoom GraphRoom {get; protected set;}

        public override float FadePercent {get; set;}

        public override Node2D GetDoorSpawn(CardinalDirection dir)
        {
            return GetNode<Node2D>($"%DoorSpawns/{dir}");
        }

        public override void _Process(float deltaTime)
        {
            // Adjust the fade curtain's transparency
            var curtain = GetNode<Polygon2D>("%FadeCurtain");

            curtain.Visible = FadePercent > 0;
            curtain.Modulate = GetBackgroundColor(1 - FadePercent);
        }

        public override void SetGraphRoom(DungeonGraphRoom graphRoom)
        {
            GraphRoom = graphRoom;

            // Fill in all the door slots
            SetDoor(CardinalDirection.North);
            SetDoor(CardinalDirection.South);
            SetDoor(CardinalDirection.East);
            SetDoor(CardinalDirection.West);
        }

        private void SetDoor(CardinalDirection dir)
        {
            var spawn = GetDoorSpawn(dir);
            var graphDoor = GraphRoom.GetDoor(dir);

            // If the door doesn't go anywhere, just put a wall here.
            if (graphDoor.Destination == null)
            {
                Create<Node2D>(spawn, DoorWallPrefab);
                return;
            }

            var warp = Create<DoorWarp>(spawn, DoorWarpPrefab);
            warp.DoorUsed += () => DoorUsed?.Invoke(dir);

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
