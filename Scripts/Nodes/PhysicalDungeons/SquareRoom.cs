using System;
using System.Linq;
using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.Nodes.Elements;
using RandomDungeons.Utils;

namespace RandomDungeons.PhysicalDungeons
{
    public class SquareRoom : Node2D
    {
        [Export] public PackedScene DoorWallPrefab;
        [Export] public PackedScene DoorLockPrefab;
        [Export] public PackedScene DoorWarpPrefab;

        [Export] public PackedScene KeyPrefab;
        [Export] public PackedScene LightsOutPuzzlePrefab;
        [Export] public PackedScene SlidingIcePuzzlePrefab;
        [Export] public PackedScene VictoryChestPrefab;

        private DungeonGraphRoom _graphRoom;

        private Node2D _northDoorSpawn => GetNode<Node2D>("%NorthDoorSpawn");
        private Node2D _southDoorSpawn => GetNode<Node2D>("%SouthDoorSpawn");
        private Node2D _eastDoorSpawn => GetNode<Node2D>("%EastDoorSpawn");
        private Node2D _westDoorSpawn => GetNode<Node2D>("%WestDoorSpawn");

        private Node2D _contentSpawn => GetNode<Node2D>("%ContentSpawn");

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

            // Spawn the room's content
            switch (_graphRoom.ChallengeType)
            {
                case ChallengeType.Puzzle:
                {
                    GeneratePuzzle();
                    break;
                }

                case ChallengeType.Loot:
                {
                    if (_graphRoom.HasKey)
                    {
                        var key = Create<Key>(_contentSpawn, KeyPrefab);
                        key.KeyId = _graphRoom.KeyId;
                    }

                    break;
                }

                case ChallengeType.Boss:
                {
                    // TODO: Put a boss here instead of a chest.  That'll teach
                    // 'em!
                    Create<Node2D>(_contentSpawn, VictoryChestPrefab);
                    break;
                }
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

        private void GeneratePuzzle()
        {
            // TODO: This sucks.  Do something less tedious.
            var rng = new Random(_graphRoom.RoomSeed);
            var puzzlePrefab = rng.PickFromWeighted(
                (SlidingIcePuzzlePrefab, 3),
                (LightsOutPuzzlePrefab, 1)
            );

            if (puzzlePrefab == SlidingIcePuzzlePrefab)
            {
                var graph = SlidingIceGraph.Generate(
                    seed: _graphRoom.RoomSeed,
                    width: 10,
                    height: 10,
                    numPushes: 5,
                    numRedHerringRocks: 3
                );
                var puzzle = Create<SlidingIcePuzzle>(_contentSpawn, SlidingIcePuzzlePrefab);
                puzzle.SetGraph(graph);
            }
            else if (puzzlePrefab == LightsOutPuzzlePrefab)
            {
                var graph = LightsOutGraph.Generate(
                    seed: _graphRoom.RoomSeed,
                    width: 4,
                    height: 4,
                    numFlips: 3
                );
                var puzzle = Create<LightsOutPuzzle>(_contentSpawn, LightsOutPuzzlePrefab);
                puzzle.SetGraph(graph);
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






