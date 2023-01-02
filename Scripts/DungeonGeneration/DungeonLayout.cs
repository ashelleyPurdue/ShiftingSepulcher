using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomDungeons
{
    public class DungeonLayout
    {
        private Dictionary<Vector2i, DungeonTreeRoom> _coordsToRoom =
            new Dictionary<Vector2i, DungeonTreeRoom>();
        private Dictionary<DungeonTreeRoom, Vector2i> _roomToCoords =
            new Dictionary<DungeonTreeRoom, Vector2i>();

        public bool HasRoomAt(Vector2i coords) => _coordsToRoom.ContainsKey(coords);

        public Vector2i CoordsOf(DungeonTreeRoom room) => _roomToCoords[room];
        public bool IsPlaced(DungeonTreeRoom room) => _roomToCoords.ContainsKey(room);

        public DungeonLayoutRoom RoomAt(Vector2i coords)
        {
            return new DungeonLayoutRoom(this, coords, _coordsToRoom[coords]);
        }

        public IEnumerable<DungeonLayoutRoom> AllRooms()
        {
            return _coordsToRoom.Select(p => new DungeonLayoutRoom(this, p.Key, p.Value));
        }

        public DungeonLayout WithRoomAt(Vector2i coords, DungeonTreeRoom room)
        {
            if (HasRoomAt(coords))
                throw new DungeonLayoutException($"The coordinates {coords} already have a room");

            if (IsPlaced(room))
                throw new DungeonLayoutException("That room is already placed");

            var clone = Clone();
            clone._coordsToRoom[coords] = room;
            clone._roomToCoords[room] = coords;

            return clone;
        }

        public DungeonLayout Clone()
        {
            return new DungeonLayout
            {
                _coordsToRoom = _coordsToRoom.Clone(),
                _roomToCoords = _roomToCoords.Clone()
            };
        }
    }
}
