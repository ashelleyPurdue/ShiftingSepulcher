using System;
using System.Collections.Generic;
using System.Linq;

using RandomDungeons.Utils;

namespace RandomDungeons.Graphs
{
    public class DungeonGraph
    {
        public int RoomCount => _rooms.Count;
        public DungeonGraphRoom StartRoom => _rooms[Vector2i.Zero];

        private Dictionary<Vector2i, DungeonGraphRoom> _rooms
            = new Dictionary<Vector2i, DungeonGraphRoom>();

        public IEnumerable<Vector2i> AllRoomCoordinates()
        {
            return _rooms.Keys;
        }

        public DungeonGraphRoom GetRoom(Vector2i coords)
        {
            return _rooms[coords];
        }

        public DungeonGraphRoom CreateRoom(Vector2i coords, int sequenceNumber)
        {
            if (_rooms.ContainsKey(coords))
                throw new DungeonGraphException($"There's already a room at {coords}");

            var room = new DungeonGraphRoom(this, coords, sequenceNumber);
            _rooms[coords] = room;

            return room;
        }

        public bool CoordinatesInUse(Vector2i c)
        {
            return _rooms.ContainsKey(c);
        }

        /// <summary>
        /// Returns the number of rooms surrounding the given coordinates,
        /// regardless of if they're connected by doors or not.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int SurroundingRoomCount(Vector2i pos)
        {
            return CardinalDirectionUtils.All()
                .Where(dir => CoordinatesInUse(pos.Adjacent(dir)))
                .Count();
        }
    }
}
