using System.Collections.Generic;

namespace RandomDungeons.DungeonGraphs
{
    public class DungeonRoom
    {
        /// <summary>
        /// Seed used to generate this room's content.
        /// This is a separate seed from the one used to lay out all the rooms.
        /// That way, changing the algorithm that populates a room doesn't
        /// cause the overall layout of the dungeon to change.
        /// </summary>
        public int RoomSeed;

        public Dictionary<DoorDirection, DungeonRoom> Doors = new Dictionary<DoorDirection, DungeonRoom>();
        public bool IsBossRoom;
    }
}
