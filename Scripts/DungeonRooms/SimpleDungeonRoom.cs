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

        private KeyChest _keyChest = null;

        public virtual Node2D GetDoorSpawn(CardinalDirection dir)
        {
            return GetNode<Node2D>($"%DoorSpawns/{dir}");
        }

        public override void _PhysicsProcess(float deltaTime)
        {
            // Detect when the challenge is solved
            if (!_sentChallengeSolvedSignal && IsChallengeSolved())
            {
                _sentChallengeSolvedSignal = true;

                if (_challenges.Any())
                {
                    EmitSignal(nameof(ChallengeSolved));
                }

                OnChallengeSolved();
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

            // Create (but do not reveal) a key chest, if there is a key in
            // this room.  The chest will be revealed when the room's challenge
            // is solved.
            if (layoutRoom.TreeRoom.KeyId > 0)
            {
                _keyChest = DoorPrefabs.KeyChest.Instance<KeyChest>();
                _keyChest.KeyId = layoutRoom.TreeRoom.KeyId;

                // Choose a random location for the chest, from a set of
                // potential spawn points
                var spawnPoints = GetNode("%ChestSpawns")
                    .EnumerateChildren()
                    .Cast<Node2D>()
                    .Select(n => n.Position);

                _keyChest.Position = rng.PickFrom(spawnPoints);
            }
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

            var targetTreeRoom = treeDoor?.Destination;
            var targetLayoutRoom = LayoutRoom.Layout.GetLayoutRoom(targetTreeRoom);
            var targetRoom2D = treeRoomToRealRoom[targetTreeRoom];

            // Set up the warp
            var warp = Create<WarpTrigger>(spawn, DoorPrefabs.Warp);
            warp.TargetEntrance = targetRoom2D.GetEntrance(dir.Opposite().ToString());

            // Use a stairs transition if the rooms are on different floors
            if (LayoutRoom.Position.z > targetLayoutRoom.Position.z)
            {
                warp.TransitionAnimation = RoomTransitionAnimation.StairsDown;
                Create<Node2D>(spawn, DoorPrefabs.StairsDownModel);
            }
            else if (LayoutRoom.Position.z < targetLayoutRoom.Position.z)
            {
                warp.TransitionAnimation = RoomTransitionAnimation.StairsUp;
                Create<Node2D>(spawn, DoorPrefabs.StairsUpModel);
            }

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

        protected T Create<T>(Node2D spawnPoint, PackedScene prefab) where T : Node2D
        {
            var node = prefab.Instance<T>();
            AddChild(node);
            node.Transform = spawnPoint.GetTransformRelativeToAncestor(this);

            return node;
        }

        public virtual bool IsChallengeSolved()
        {
            if (_challenges.Length == 0)
                return true;

            return _challenges.All(c => c.IsSolved());
        }

        private void OnChallengeSolved()
        {
            // Open all barred doors
            foreach (var bars in this.AllDescendantsOfType<DoorBars>())
            {
                bars.IsOpened = true;
            }

            // Reveal the key chest, if there is a chest in this room
            if (_keyChest != null)
            {
                AddChild(_keyChest);
            }
        }
    }
}
