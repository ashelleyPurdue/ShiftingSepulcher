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
        public bool HasKey => KeyId > 0;

        public IDungeonGraphDoor NorthDoor => GetDoor(CardinalDirection.North);
        public IDungeonGraphDoor SouthDoor => GetDoor(CardinalDirection.South);
        public IDungeonGraphDoor EastDoor => GetDoor(CardinalDirection.East);
        public IDungeonGraphDoor WestDoor => GetDoor(CardinalDirection.West);

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

        public IEnumerable<CardinalDirection> PotentialOneWayDoors()
        {
            // Return all adjacent rooms that are earlier in the sequence
            // but that don't have a door connecting to this room.
            foreach (var dir in CardinalDirectionUtils.All())
            {
                if (!(_doors[dir] is DungeonGraphDoor door))
                    continue;
                
                bool isWall = door.Destination == null;
                if (!isWall)
                    continue;
                
                var neighborPos = Position.Adjacent(dir);
                if (!Graph.CoordinatesInUse(neighborPos))
                    continue;

                var neighbor = Graph.GetRoom(neighborPos);
                if (neighbor.SequenceNumber >= this.SequenceNumber)
                    continue;

                yield return dir;
            }
        }

        public DungeonGraphRoom AddNeighbor(
            CardinalDirection dir,
            int sequenceNumber
        )
        {
            if (!CanAddRoom(dir))
                throw new Exception("Another room is already there.");

            var neighborPos = Position.Adjacent(dir);
            var neighbor = Graph.CreateRoom(neighborPos, SequenceNumber);
            Graph.JoinAdjacentRooms(this.Position, neighbor.Position);

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
