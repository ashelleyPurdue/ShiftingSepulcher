using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomDungeons.DungeonGraphs
{
    public static class DungeonGenerator
    {
        public static DungeonGraph GenerateKeylessGraph(int seed, int numRooms)
        {
            if (numRooms < 1)
                throw new Exception("You can't have a zero-room dungeon.");

            var rng = new Random(seed);
            var dungeon = new DungeonGraph();
            dungeon.CreateRoom(RoomCoordinates.Origin);

            for (int i = 0; i < numRooms; i++)
            {
                // Pick a random unused door and add a room to it.
                var unusedDoors = dungeon.UnusedDoors().ToArray();
                int doorIndex = rng.Next(0, unusedDoors.Length);

                (RoomCoordinates parentCoords, CardinalDirection dir) = unusedDoors[doorIndex];
                RoomCoordinates childCoords = parentCoords.Adjacent(dir);

                DungeonRoom childRoom = dungeon.CreateRoom(childCoords);
                dungeon.JoinAdjacentRooms(parentCoords, childCoords);

                childRoom.RoomSeed = rng.Next();
            }

            return dungeon;
        }
    }
}
