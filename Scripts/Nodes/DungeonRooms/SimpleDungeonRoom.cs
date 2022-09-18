using System;
using System.Linq;
using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.Nodes.Elements;
using RandomDungeons.Utils;
using RandomDungeons.PhysicalDungeons;
using RandomDungeons.Resources;

namespace RandomDungeons.Nodes.DungeonRooms
{
    public class SimpleDungeonRoom : Node2D, IDungeonRoom
    {
        public Node2D Node => this;

        public event Action<CardinalDirection> DoorUsed;

        [Export] public DoorPrefabCollection DoorPrefabs;

        public DungeonGraphRoom GraphRoom {get; protected set;}

        public float FadePercent {get; set;}

        public Node2D GetDoorSpawn(CardinalDirection dir)
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

        public virtual void Populate(DungeonGraphRoom graphRoom)
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
                Create<Node2D>(spawn, DoorPrefabs.Wall);
                return;
            }

            var warp = Create<DoorWarp>(spawn, DoorPrefabs.Warp);
            warp.DoorUsed += () => DoorUsed?.Invoke(dir);

            // Spawn the correct kind of door
            if (graphDoor is KeyDungeonGraphDoor lockedDoor)
            {
                var doorLock = Create<DoorLock>(spawn, DoorPrefabs.Lock);
                doorLock.KeyId = lockedDoor.KeyId;
            }
            else if (graphDoor is OneWayClosedSideGraphDoor closedSideGraphDoor)
            {
                var door = Create<OneWayDoorClosedSide>(spawn, DoorPrefabs.OneWayClosedSide);
                door.SetGraphDoor(closedSideGraphDoor);
            }
            else if (graphDoor is OneWayOpenSideGraphDoor openSideGraphDoor)
            {
                var door = Create<OneWayDoorOpenSide>(spawn, DoorPrefabs.OneWayOpenSide);
                door.SetGraphDoor(openSideGraphDoor);
            }
        }

        protected T Create<T>(Node2D parent, PackedScene prefab) where T : Node2D
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
