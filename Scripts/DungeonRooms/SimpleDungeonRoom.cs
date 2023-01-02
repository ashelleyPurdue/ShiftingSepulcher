using System;
using System.Linq;
using System.Collections.Generic;

using Godot;


namespace RandomDungeons
{
    public class SimpleDungeonRoom : Room2D, IDungeonRoom
    {
        public Node2D Node => this;

        public event Action<CardinalDirection> DoorUsed;

        [Export] public DoorPrefabCollection DoorPrefabs;

        public DungeonGraphRoom GraphRoom {get; protected set;}

        private IChallenge[] _challenges;

        public virtual Node2D GetDoorSpawn(CardinalDirection dir)
        {
            return GetNode<Node2D>($"%DoorSpawns/{dir}");
        }

        public override void _Process(float deltaTime)
        {
            // Open all door bars if the the challenge has been solved
            if (IsChallengeSolved())
            {
                foreach (var bars in this.AllDescendantsOfType<DoorBars>())
                {
                    bars.IsOpened = true;
                }
            }
        }

        public virtual void Populate(DungeonGraphRoom graphRoom)
        {
            GraphRoom = graphRoom;

            var rng = new Random(graphRoom.RoomSeed);
            foreach (var populator in this.AllDescendantsOfType<IRoomPopulator>())
            {
                populator.Populate(graphRoom, rng);
            }

            // Gather up all IChallenge nodes, so we don't need to do a full
            // traversal every frame
            _challenges = this.AllDescendantsOfType<IChallenge>().ToArray();
        }

        public void ConnectDoors(
            Dictionary<DungeonGraphRoom, Room2D> graphRoomToRealRoom,
            Dictionary<OneWayClosedSideGraphDoor, OneWayDoorClosedSide> closedSideGraphToClosedSideReal,
            Dictionary<OneWayDoorClosedSide, OneWayOpenSideGraphDoor> closedSideRealToOpenSideGraph,
            Dictionary<OneWayOpenSideGraphDoor, OneWayDoorOpenSide> openSideGraphToOpenSideReal
        )
        {
            // Fill in all the door slots
            SetDoor(
                CardinalDirection.North,
                graphRoomToRealRoom,
                closedSideGraphToClosedSideReal,
                closedSideRealToOpenSideGraph,
                openSideGraphToOpenSideReal
            );
            SetDoor(
                CardinalDirection.South,
                graphRoomToRealRoom,
                closedSideGraphToClosedSideReal,
                closedSideRealToOpenSideGraph,
                openSideGraphToOpenSideReal
            );
            SetDoor(
                CardinalDirection.East,
                graphRoomToRealRoom,
                closedSideGraphToClosedSideReal,
                closedSideRealToOpenSideGraph,
                openSideGraphToOpenSideReal
            );
            SetDoor(
                CardinalDirection.West,
                graphRoomToRealRoom,
                closedSideGraphToClosedSideReal,
                closedSideRealToOpenSideGraph,
                openSideGraphToOpenSideReal
            );

            // TODO: Connect all the one-way doors to each other
        }

        private void SetDoor(
            CardinalDirection dir,
            Dictionary<DungeonGraphRoom, Room2D> graphRoomToRealRoom,
            Dictionary<OneWayClosedSideGraphDoor, OneWayDoorClosedSide> closedSideGraphToClosedSideReal,
            Dictionary<OneWayDoorClosedSide, OneWayOpenSideGraphDoor> closedSideRealToOpenSideGraph,
            Dictionary<OneWayOpenSideGraphDoor, OneWayDoorOpenSide> openSideGraphToOpenSideReal
        )
        {
            var spawn = GetDoorSpawn(dir);
            var graphDoor = GraphRoom.GetDoor(dir);

            // If the door doesn't go anywhere, just put a wall here.
            if (graphDoor.Destination == null)
            {
                Create<Node2D>(spawn, DoorPrefabs.Wall);
                return;
            }

            // Set up the warp
            var warp = Create<WarpTrigger>(spawn, DoorPrefabs.Warp);
            DungeonGraphRoom targetGraphRoom = GraphRoom
                .GetDoor(dir)
                .Destination;

            var targetRoom = graphRoomToRealRoom[targetGraphRoom];
            warp.TargetEntrance = targetRoom.GetEntrance(dir.Opposite().ToString());

            // Spawn the correct kind of door
            if (graphDoor is KeyDungeonGraphDoor lockedDoor)
            {
                var doorLock = Create<DoorLock>(spawn, DoorPrefabs.Lock);
                doorLock.KeyId = lockedDoor.KeyId;
            }
            else if (graphDoor is ChallengeDungeonGraphDoor challengeDoor)
            {
                Create<DoorBars>(spawn, DoorPrefabs.Bars);
            }
            else if (graphDoor is OneWayClosedSideGraphDoor closedSideGraphDoor)
            {
                var door = Create<OneWayDoorClosedSide>(spawn, DoorPrefabs.OneWayClosedSide);
                closedSideGraphToClosedSideReal[closedSideGraphDoor] = door;
                closedSideRealToOpenSideGraph[door] = closedSideGraphDoor.OtherSide;
            }
            else if (graphDoor is OneWayOpenSideGraphDoor openSideGraphDoor)
            {
                var door = Create<OneWayDoorOpenSide>(spawn, DoorPrefabs.OneWayOpenSide);
                openSideGraphToOpenSideReal[openSideGraphDoor] = door;
            }
        }

        protected T Create<T>(Node2D parent, PackedScene prefab) where T : Node2D
        {
            var node = prefab.Instance<T>();
            parent.AddChild(node);
            return node;
        }

        public virtual bool IsChallengeSolved()
        {
            if (_challenges.Length == 0)
                return true;

            return _challenges.All(c => c.IsSolved());
        }
    }
}
