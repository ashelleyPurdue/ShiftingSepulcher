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

        public bool HasDoorAtDirection(CardinalDirection dir)
        {
            return DoorAtDirection(dir) != null;
        }

        /// <summary>
        /// Returns the door that goes in the given direction, or null if it's
        /// a wall
        /// </summary>
        /// <returns></returns>
        public IDungeonTreeDoor DoorAtDirection(CardinalDirection dir)
        {
            foreach (var door in TreeRoom.AllDoors())
            {
                if (!Layout.IsPlaced(door.Destination))
                    continue;

                var neighborPos = Layout
                    .CoordsOf(door.Destination)
                    .FlattenToVector2i();

                var adjacentDir = Position
                    .FlattenToVector2i()
                    .AdjacentDirection(neighborPos);

                if (adjacentDir == dir)
                    return door;
            }

            return null;
        }
    }
}
