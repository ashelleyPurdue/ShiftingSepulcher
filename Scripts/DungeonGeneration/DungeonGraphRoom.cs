using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomDungeons
{
    public class DungeonGraphRoom
    {
        public readonly DungeonGraph Graph;
        public readonly Vector2i Position;

        /// <summary>
        /// Seed used to generate this room's content.
        /// This is a separate seed from the one used to lay out all the rooms.
        /// That way, changing the algorithm that populates a room doesn't
        /// cause the overall layout of the dungeon to change.
        /// </summary>
        public int RoomSeed;
        public ChallengeType ChallengeType;

        public readonly int SequenceNumber;
        public int KeyId = 0;

        private readonly Dictionary<CardinalDirection, IDungeonGraphDoor> _doors;

        public DungeonGraphRoom(
            DungeonGraph graph,
            Vector2i pos,
            int sequenceNumber
        )
        {
            Graph = graph;
            Position = pos;
            SequenceNumber = sequenceNumber;

            _doors = new Dictionary<CardinalDirection, IDungeonGraphDoor>
            {
                {CardinalDirection.North, new DungeonGraphDoor()},
                {CardinalDirection.South, new DungeonGraphDoor()},
                {CardinalDirection.East, new DungeonGraphDoor()},
                {CardinalDirection.West, new DungeonGraphDoor()}
            };
        }

        public IDungeonGraphDoor GetDoor(CardinalDirection dir)
        {
            return _doors[dir];
        }

        /// <summary>
        /// Replaces the given door with a locked one, using the given keyId.
        /// Preserves the old door's <see cref="IDungeonGraphDoor.Destination"/>
        /// value.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="keyId"></param>
        public void LockDoor(CardinalDirection dir, int keyId)
        {
            var oldDoor = GetDoor(dir);
            var newDoor = new KeyDungeonGraphDoor(keyId);
            newDoor.Destination = oldDoor.Destination;

            _doors[dir] = newDoor;
        }

        /// <summary>
        /// Replaces the given door with a challenge door, which should open
        /// when that room's challenge is completed.
        /// Preserves the old door's <see cref="IDungeonGraphDoor.Destination"/>
        /// value.
        /// </summary>
        /// <param name="dir"></param>
        public void SetChallengeDoor(CardinalDirection dir)
        {
            var oldDoor = GetDoor(dir);
            var newDoor = new ChallengeDungeonGraphDoor();
            newDoor.Destination = oldDoor.Destination;

            _doors[dir] = newDoor;
        }

        /// <summary>
        /// Drills a hole in the given wall, and then fills it with a one-way
        /// door.
        ///
        /// Fails if there is no existing room behind the chosen wall.
        /// </summary>
        /// <param name="dir"></param>
        public void DrillOneWayDoor(CardinalDirection dir)
        {
            Vector2i neighborPos = Position.Adjacent(dir);

            if (!Graph.CoordinatesInUse(neighborPos))
                throw new DungeonGraphException("Tried to drill a hole into nowhere");

            var neighbor = Graph.GetRoom(neighborPos);

            var openSideDoor = new OneWayOpenSideGraphDoor();
            var closedSideDoor = new OneWayClosedSideGraphDoor();

            _doors[dir] = openSideDoor;
            neighbor._doors[dir.Opposite()] = closedSideDoor;

            openSideDoor.Destination = neighbor;
            closedSideDoor.Destination = this;

            openSideDoor.OtherSide = closedSideDoor;
            closedSideDoor.OtherSide = openSideDoor;
        }
    }
}
