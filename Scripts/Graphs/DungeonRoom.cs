using System;
using System.Collections.Generic;
using System.Linq;

using RandomDungeons.Utils;

namespace RandomDungeons.Graphs
{
    public class DungeonRoom
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
        public bool IsBossRoom;

        public int KeyId = 0;
        public bool HasKey => KeyId > 0;

        public DungeonDoor NorthDoor => GetDoor(CardinalDirection.North);
        public DungeonDoor SouthDoor => GetDoor(CardinalDirection.South);
        public DungeonDoor EastDoor => GetDoor(CardinalDirection.East);
        public DungeonDoor WestDoor => GetDoor(CardinalDirection.West);

        private readonly Dictionary<CardinalDirection, DungeonDoor> _doors;

        public DungeonRoom(DungeonGraph graph, Vector2i pos)
        {
            Graph = graph;
            Position = pos;

            _doors = new Dictionary<CardinalDirection, DungeonDoor>
            {
                {CardinalDirection.North, new DungeonDoor()},
                {CardinalDirection.South, new DungeonDoor()},
                {CardinalDirection.East, new DungeonDoor()},
                {CardinalDirection.West, new DungeonDoor()}
            };
        }

        public DungeonDoor GetDoor(CardinalDirection dir)
        {
            return _doors[dir];
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

        public DungeonRoom AddNeighbor(CardinalDirection dir)
        {
            if (!CanAddRoom(dir))
                throw new Exception("Another room is already there.");

            var neighborPos = Position.Adjacent(dir);
            var neighbor = Graph.CreateRoom(neighborPos);
            Graph.JoinAdjacentRooms(this.Position, neighbor.Position);

            return neighbor;
        }
    }
}
