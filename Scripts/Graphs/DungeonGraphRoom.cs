using System;
using System.Collections.Generic;
using System.Linq;

using RandomDungeons.Utils;

namespace RandomDungeons.Graphs
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

        public void SetDoor(CardinalDirection dir, IDungeonGraphDoor door)
        {
            _doors[dir] = door;
        }

        public bool CanAddRoom(CardinalDirection dir)
        {
            var newCoords = Position.Adjacent(dir);
            return !Graph.CoordinatesInUse(newCoords);
        }

        public bool CanAddAnyRooms()
        {
            return
                CanAddRoom(CardinalDirection.North) ||
                CanAddRoom(CardinalDirection.South) ||
                CanAddRoom(CardinalDirection.East) ||
                CanAddRoom(CardinalDirection.West);
        }

        public CardinalDirection[] UnusedDoors()
        {
            return _doors
                .Keys
                .Where(CanAddRoom)
                .ToArray();
        }

        public IEnumerable<CardinalDirection> AllWalls()
        {
            return CardinalDirectionUtils.All()
                .Where(dir => _doors[dir] is DungeonGraphDoor)
                .Where(dir => _doors[dir].Destination == null);
        }

        public DungeonGraphRoom AddNeighbor(
            CardinalDirection dir,
            int sequenceNum
        )
        {
            if (!CanAddRoom(dir))
                throw new Exception("Another room is already there.");

            var neighborPos = Position.Adjacent(dir);
            var neighbor = Graph.CreateRoom(neighborPos, sequenceNum);

            GetDoor(dir).Destination = neighbor;
            neighbor.GetDoor(dir.Opposite()).Destination = this;

            return neighbor;
        }

        public void AddOneWayDoor(CardinalDirection dir)
        {
            var neighborPos = Position.Adjacent(dir);
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
