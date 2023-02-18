using System;
using System.Collections.Generic;
using System.Linq;

namespace ShiftingSepulcher
{
    public readonly struct DungeonLayoutRoom
    {
        public readonly DungeonLayout Layout;
        public readonly Vector3i Position;
        public readonly DungeonTreeRoom TreeRoom;

        public DungeonLayoutRoom(
            DungeonLayout layout,
            Vector3i position,
            DungeonTreeRoom treeRoom
        )
        {
            Layout = layout;
            Position = position;
            TreeRoom = treeRoom;
        }

        /// <summary>
        /// Returns the door that goes in the given direction, or null if it's
        /// a wall
        /// </summary>
        /// <returns></returns>
        public IDungeonTreeDoor DoorAtDirection(CardinalDirection dir)
        {
            var destPos = Position + dir.ToVector2i().ToVector3i();

            if (!Layout.HasRoomAt(destPos))
                return null;

            var dest = Layout.RoomAt(destPos);

            return TreeRoom
                .AllDoors()
                .FirstOrDefault(d => d.Destination == dest.TreeRoom);
        }
    }
}
