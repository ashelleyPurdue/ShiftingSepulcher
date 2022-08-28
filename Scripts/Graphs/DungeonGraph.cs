using System;
using System.Collections.Generic;

using RandomDungeons.Utils;

namespace RandomDungeons.Graphs
{
    public class DungeonGraph
    {
        public int RoomCount => _rooms.Count;
        public DungeonRoom StartRoom => _rooms[Vector2i.Zero];

        private Dictionary<Vector2i, DungeonRoom> _rooms
            = new Dictionary<Vector2i, DungeonRoom>();

        public IEnumerable<Vector2i> AllRoomCoordinates()
        {
            return _rooms.Keys;
        }

        public DungeonRoom GetRoom(Vector2i coords)
        {
            return _rooms[coords];
        }

        public DungeonRoom CreateRoom(Vector2i coords)
        {
            if (_rooms.ContainsKey(coords))
                throw new Exception($"There's already a room at {coords}");

            var room = new DungeonRoom(this, coords);
            _rooms[coords] = room;

            return room;
        }

        public bool CoordinatesInUse(Vector2i c)
        {
            return _rooms.ContainsKey(c);
        }

        public void JoinAdjacentRooms(Vector2i a, Vector2i b)
        {
            if (!a.IsAdjacentTo(b))
                throw new Exception("Those coordinates are not adjacent");

            if (!_rooms.ContainsKey(a))
                throw new Exception($"There is no room at {a}");

            if (!_rooms.ContainsKey(b))
                throw new Exception($"There is no room at {b}");

            _rooms[a].GetDoor(a.AdjacentDirection(b)).Destination = _rooms[b];
            _rooms[b].GetDoor(b.AdjacentDirection(a)).Destination = _rooms[a];
        }

        public IEnumerable<(Vector2i parentCoords, CardinalDirection dir)> UnusedDoors()
        {
            foreach (var roomCoords in _rooms.Keys)
            {
                foreach (var dir in CardinalDirectionUtils.All())
                {
                    if (!_rooms.ContainsKey(roomCoords.Adjacent(dir)))
                        yield return (roomCoords, dir);
                }
            }
        }
    }
}
