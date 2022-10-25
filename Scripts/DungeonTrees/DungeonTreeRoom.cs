using System;
using System.Collections.Generic;

namespace RandomDungeons
{
    public class DungeonTreeRoom
    {
        public int RoomSeed;
        public ChallengeType ChallengeType;
        public int KeyId;

        public DungeonTreeRoom Parent {get; private set;}

        public IReadOnlyList<IDungeonTreeDoor> ChildDoors => _childDoors;
        public IEnumerable<DungeonTreeRoom> OutgoingShortcuts => _outgoingShortcuts;
        public IEnumerable<DungeonTreeRoom> IncomingShortcuts => _incomingShortcuts;

        private List<IDungeonTreeDoor> _childDoors = new List<IDungeonTreeDoor>();
        private HashSet<DungeonTreeRoom> _incomingShortcuts = new HashSet<DungeonTreeRoom>();
        private HashSet<DungeonTreeRoom> _outgoingShortcuts = new HashSet<DungeonTreeRoom>();

        public void AddChallengeDoor(DungeonTreeRoom room)
        {
            if (room.Parent != null)
                throw new DungeonTreeException("That room already has a parent");

            room.Parent = this;

            var door = new ChallengeDoor();
            door.Destination = room;
            _childDoors.Add(door);
        }

        public void AddLockedDoor(DungeonTreeRoom room, int keyId)
        {
            if (room.Parent != null)
                throw new DungeonTreeException("That room already has a parent");

            room.Parent = this;

            var door = new LockedDoor
            {
                Destination = room,
                KeyId = keyId
            };
            _childDoors.Add(door);
        }

        public void AddOutgoingShortcut(DungeonTreeRoom shortcutDest)
        {
            _outgoingShortcuts.Add(shortcutDest);
            shortcutDest._incomingShortcuts.Add(this);
        }

        /// <summary>
        /// Yields this room's parent, and its parent's parent, and so on.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DungeonTreeRoom> AllAncestors()
        {
            var currentRoom = Parent;

            while (currentRoom != null)
            {
                yield return currentRoom;
                currentRoom = currentRoom.Parent;
            }
        }

        /// <summary>
        /// Yields itself and all descendants in a depth-first search
        /// <returns></returns>
        public IEnumerable<DungeonTreeRoom> AllDescendants()
        {
            yield return this;

            foreach (var childDoor in ChildDoors)
            {
                foreach (var descendant in childDoor.Destination.AllDescendants())
                {
                    yield return descendant;
                }
            }
        }
    }
}
