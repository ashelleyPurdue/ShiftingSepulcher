using System.Collections.Generic;
using System.Linq;

namespace RandomDungeons.DungeonGraphs
{
    public class DungeonRoom
    {
        public readonly DungeonGraph Graph;
        public readonly RoomCoordinates Position;

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

        public DungeonRoom NorthRoom => NeighborRoom(CardinalDirection.North);
        public DungeonRoom SouthRoom => NeighborRoom(CardinalDirection.South);
        public DungeonRoom EastRoom => NeighborRoom(CardinalDirection.East);
        public DungeonRoom WestRoom => NeighborRoom(CardinalDirection.West);

        private readonly Dictionary<CardinalDirection, DungeonDoor> _doors;

        public DungeonRoom(DungeonGraph graph, RoomCoordinates pos)
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

        public CardinalDirection[] UnusedDoors()
        {
            return _doors
                .Keys
                .Where(CanAddRoom)
                .ToArray();
        }

        private DungeonRoom NeighborRoom(CardinalDirection dir)
        {
            if (!_doors.ContainsKey(dir))
                return null;

            return _doors[dir].Destination;
        }
    }
}
