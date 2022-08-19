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
                DungeonRoom parentRoom = dungeon.GetRoom(parentCoords);
                DungeonRoom childRoom = parentRoom.AddNeighbor(dir);
                childRoom.RoomSeed = rng.Next();
            }

            return dungeon;
        }

        public static DungeonGraph GenerateUsingRuns(
            int seed,
            int minRunSize,
            int maxRunSize,
            int numRuns
        )
        {
            var rng = new Random(seed);
            var dungeon = new DungeonGraph();
            dungeon.CreateRoom(RoomCoordinates.Origin);

            for (int run = 0; run < numRuns; run++)
            {
                DungeonRoom startingRoom = ChooseRandomStartRoom();
                int runLength = rng.Next(0, maxRunSize + 1);

                GenerateRun(startingRoom, runLength);
            }

            return dungeon;

            DungeonRoom ChooseRandomStartRoom()
            {
                var possibleStartingRooms = dungeon
                    .AllRoomCoordinates()
                    .Select(c => dungeon.GetRoom(c))
                    .Where(r => r.CanAddAnyRooms())
                    .ToArray();

                int index = rng.Next(0, possibleStartingRooms.Length);
                return possibleStartingRooms[index];
            }

            void GenerateRun(DungeonRoom startingRoom, int runLength)
            {
                if (runLength == 0)
                    return;

                if (!startingRoom.CanAddAnyRooms())
                    return;

                // Pick a direction.
                var directions = startingRoom.UnusedDoors();
                int index = rng.Next(0, directions.Length);
                CardinalDirection dir = directions[index];

                // Create a new room in that direction
                DungeonRoom nextRoom = startingRoom.AddNeighbor(dir);

                // Continue the run, recursively.
                // Because I just felt like using recursion today.
                GenerateRun(nextRoom, runLength - 1);
            }
        }
    }
}
