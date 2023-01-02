using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomDungeons
{
    public readonly struct DungeonLayoutRoom
    {
        public readonly DungeonLayout Layout;
        public readonly Vector2i Position;
        public DungeonTreeRoom TreeRoom => Layout.RoomAt(Position);

        public DungeonLayoutRoom(DungeonLayout layout, Vector2i position)
        {
            Layout = layout;
            Position = position;
        }

        /// <summary>
        /// Returns the door that goes in the given direction, or null if it's
        /// a wall
        /// </summary>
        /// <returns></returns>
        public IDungeonTreeDoor DoorAtDirection(CardinalDirection dir)
        {
            var destPos = Position.Adjacent(dir);

            if (!Layout.HasRoomAt(destPos))
                return null;

            var dest = Layout.RoomAt(destPos);

            return TreeRoom
                .AllDoors()
                .FirstOrDefault(d => d.Destination == dest);
        }
    }
}
