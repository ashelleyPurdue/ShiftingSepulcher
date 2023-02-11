using System;
using System.Linq;
using System.Collections.Generic;

using Godot;


namespace ShiftingSepulcher
{
    public class SimpleDungeonRoom : Room2D, IDungeonRoom
    {
        public Node2D Node => this;

        public event Action<CardinalDirection> DoorUsed;

        [Signal] public delegate void ChallengeSolved();

        [Export] public DoorPrefabCollection DoorPrefabs;

        public DungeonLayoutRoom LayoutRoom {get; protected set;}

        private IChallenge[] _challenges;
        private bool _sentChallengeSolvedSignal = false;

        public virtual Node2D GetDoorSpawn(CardinalDirection dir)
        {
            return GetNode<Node2D>($"%DoorSpawns/{dir}");
        }

        public override void _PhysicsProcess(float deltaTime)
        {
            // Open all door bars if the the challenge has been solved
            if (!_sentChallengeSolvedSignal && IsChallengeSolved())
            {
                _sentChallengeSolvedSignal = true;

                if (_challenges.Any())
                {
                    EmitSignal(nameof(ChallengeSolved));
                }

                foreach (var bars in this.AllDescendantsOfType<DoorBars>())
                {
                    bars.IsOpened = true;
                }
            }
        }

        public virtual void Populate(DungeonLayoutRoom layoutRoom)
        {
            LayoutRoom = layoutRoom;

            var rng = new Random(layoutRoom.TreeRoom.RoomSeed);
            foreach (var populator in this.AllDescendantsOfType<IRoomPopulator>())
            {
                populator.Populate(layoutRoom.TreeRoom, rng);
            }

            // Gather up all IChallenge nodes, so we don't need to do a full
            // traversal every frame
            _challenges = this.AllDescendantsOfType<IChallenge>().ToArray();
        }

        public void ConnectDoors(
            Dictionary<DungeonTreeRoom, Room2D> treeRoomToRealRoom,
            ShortcutDoorMap shortcutDoorMap
        )
        {
            // Fill in all the door slots
            SetDoor(CardinalDirection.North, treeRoomToRealRoom, shortcutDoorMap);
            SetDoor(CardinalDirection.South, treeRoomToRealRoom, shortcutDoorMap);
            SetDoor(CardinalDirection.East, treeRoomToRealRoom, shortcutDoorMap);
            SetDoor(CardinalDirection.West, treeRoomToRealRoom, shortcutDoorMap);
        }

        private void SetDoor(
            CardinalDirection dir,
            Dictionary<DungeonTreeRoom, Room2D> treeRoomToRealRoom,
            ShortcutDoorMap shortcutDoorMap
        )
        {
            var spawn = GetDoorSpawn(dir);
            var treeDoor = LayoutRoom.DoorAtDirection(dir);

            // If the door doesn't go anywhere, just put a wall here.
            if (treeDoor?.Destination == null)
            {
                Create<Node2D>(spawn, DoorPrefabs.Wall);
                return;
            }

            // Set up the warp
            var warp = Create<WarpTrigger>(spawn, DoorPrefabs.Warp);
            var targetRoom = treeRoomToRealRoom[treeDoor.Destination];
            warp.TargetEntrance = targetRoom.GetEntrance(dir.Opposite().ToString());

            // Spawn the correct kind of door
            if (treeDoor is LockedDoor lockedDoor)
            {
                var doorLock = Create<DoorLock>(spawn, DoorPrefabs.Lock);
                doorLock.KeyId = lockedDoor.KeyId;
            }
            else if (treeDoor is ChallengeDoor challengeDoor)
            {
                Create<DoorBars>(spawn, DoorPrefabs.Bars);
            }
            else if (treeDoor is IncomingShortcutDoor incomingDoor)
            {
                var door = Create<OneWayDoorClosedSide>(spawn, DoorPrefabs.OneWayClosedSide);
                shortcutDoorMap.IncomingFakeToReal[incomingDoor] = door;
            }
            else if (treeDoor is OutgoingShortcutDoor outgoingDoor)
            {
                var door = Create<OneWayDoorOpenSide>(spawn, DoorPrefabs.OneWayOpenSide);
                shortcutDoorMap.OutgoingFakeToReal[outgoingDoor] = door;
            }

            // HACK: Close the door behind the player in boss rooms until the
            // boss is dead.
            // Well, technically, this closes _every_ door, but boss rooms only
            // ever have one door anyway.
            if (LayoutRoom.TreeRoom.ChallengeType == ChallengeType.Boss)
                Create<DoorBars>(spawn, DoorPrefabs.Bars);
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
