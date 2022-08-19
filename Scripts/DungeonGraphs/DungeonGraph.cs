using System;
using System.Collections.Generic;

namespace RandomDungeons.DungeonGraphs
{
    public class DungeonGraph
    {
        private Dictionary<RoomCoordinates, DungeonRoom> _rooms
            = new Dictionary<RoomCoordinates, DungeonRoom>();

        public DungeonRoom GetRoom(RoomCoordinates coords)
        {
            return _rooms[coords];
        }

        public DungeonRoom CreateRoom(RoomCoordinates coords)
        {
            if (_rooms.ContainsKey(coords))
                throw new Exception($"There's already a room at {coords}");

            var room = new DungeonRoom();
            _rooms[coords] = room;

            return room;
        }

        public void JoinAdjacentRooms(RoomCoordinates a, RoomCoordinates b)
        {
            if (!a.IsAdjacentTo(b))
                throw new Exception("Those coordinates are not adjacent");

            if (!_rooms.ContainsKey(a))
                throw new Exception($"There is no room at {a}");

            if (!_rooms.ContainsKey(b))
                throw new Exception($"There is no room at {b}");

            _rooms[a].Doors[a.AdjacentDirection(b)] = _rooms[b];
            _rooms[b].Doors[b.AdjacentDirection(a)] = _rooms[a];
        }

        public IEnumerable<(RoomCoordinates parentCoords, CardinalDirection dir)> UnusedDoors()
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
