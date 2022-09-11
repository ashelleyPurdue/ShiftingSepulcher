using System;
using System.Linq;
using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.Nodes.Elements;
using RandomDungeons.Utils;

namespace RandomDungeons.PhysicalDungeons
{
    public class LegacyDungeonRoom : IDungeonRoom
    {
        public override event Action<CardinalDirection> DoorUsed;

        [Export] public PackedScene DoorWallPrefab;
        [Export] public PackedScene DoorLockPrefab;
        [Export] public PackedScene DoorWarpPrefab;
        [Export] public PackedScene DoorBarsPrefab;

        [Export] public PackedScene KeyPrefab;
        [Export] public PackedScene LightsOutPuzzlePrefab;
        [Export] public PackedScene SlidingIcePuzzlePrefab;
        [Export] public PackedScene VictoryChestPrefab;

        [Export] public PackedScene ObliviousZombiePrefab;

        public override DungeonGraphRoom GraphRoom {get; protected set;}
        private IDungeonRoomChallenge _challenge;

        private Node2D _contentSpawn => GetNode<Node2D>("%ContentSpawn");

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

        public void SetGraphRoom(DungeonGraphRoom graphRoom)
        {
            GraphRoom = graphRoom;

            // Fill in all the door slots
            SetDoor(CardinalDirection.North);
            SetDoor(CardinalDirection.South);
            SetDoor(CardinalDirection.East);
            SetDoor(CardinalDirection.West);

            // Spawn the room's content
            switch (GraphRoom.ChallengeType)
            {
                case ChallengeType.Puzzle:
                {
                    _challenge = GeneratePuzzle();
                    break;
                }

                case ChallengeType.Combat:
                {
                    // TODO: Spawn a random assortment of enemies, and a random
                    // assortment of walls/pits/whatnot.
                    Create<Node2D>(_contentSpawn, ObliviousZombiePrefab);
                    break;
                }

                case ChallengeType.Loot:
                {
                    if (GraphRoom.HasKey)
                    {
                        var key = Create<Key>(_contentSpawn, KeyPrefab);
                        key.KeyId = GraphRoom.KeyId;
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

            // Spawn the bars on the doors, if this is a challenge room
            if (_challenge != null)
            {
                SpawnDoorBars(CardinalDirection.North);
                SpawnDoorBars(CardinalDirection.South);
                SpawnDoorBars(CardinalDirection.East);
                SpawnDoorBars(CardinalDirection.West);
            }
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

        private IDungeonRoomChallenge GeneratePuzzle()
        {
            // TODO: This sucks.  Do something less tedious.
            var rng = new Random(GraphRoom.RoomSeed);
            var puzzlePrefab = rng.PickFromWeighted(
                (SlidingIcePuzzlePrefab, 3),
                (LightsOutPuzzlePrefab, 1)
            );

            if (puzzlePrefab == SlidingIcePuzzlePrefab)
            {
                var graph = SlidingIceGraph.Generate(
                    seed: GraphRoom.RoomSeed,
                    width: 10,
                    height: 10,
                    numPushes: 5,
                    numRedHerringRocks: 3
                );
                var puzzle = Create<SlidingIcePuzzle>(_contentSpawn, SlidingIcePuzzlePrefab);
                puzzle.SetGraph(graph);
                return puzzle;
            }
            else if (puzzlePrefab == LightsOutPuzzlePrefab)
            {
                var graph = LightsOutGraph.Generate(
                    seed: GraphRoom.RoomSeed,
                    width: 4,
                    height: 4,
                    numFlips: 3
                );
                var puzzle = Create<LightsOutPuzzle>(_contentSpawn, LightsOutPuzzlePrefab);
                puzzle.SetGraph(graph);
                return puzzle;
            }

            throw new Exception("Somehow, a third prefab was chosen.");
        }

        private void SpawnDoorBars(CardinalDirection dir)
        {
            var parent = GetDoorSpawn(dir);
            var graphDoor = GraphRoom.GetDoor(dir);

            // Don't spawn bars on plain walls
            if (graphDoor.Destination == null)
                return;

            var bars = Create<DoorBars>(parent, DoorBarsPrefab);
            bars.Challenge = _challenge;
        }

        private Color GetBackgroundColor(float alpha)
        {
            var c = (Color)ProjectSettings.GetSetting("rendering/environment/default_clear_color");
            c.a = alpha;

            return c;
        }
    }
}






