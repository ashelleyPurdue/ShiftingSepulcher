using System.Collections.Generic;

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

        public Dictionary<CardinalDirection, DungeonRoom> Doors = new Dictionary<CardinalDirection, DungeonRoom>();
        public bool IsBossRoom;

        public DungeonRoom(DungeonGraph graph, RoomCoordinates pos)
        {
            Graph = graph;
            Position = pos;
        }
    }
}
