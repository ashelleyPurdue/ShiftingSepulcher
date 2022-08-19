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
            int numRooms
        )
        {
            var rng = new Random(seed);
            var dungeon = new DungeonGraph();
            dungeon.CreateRoom(RoomCoordinates.Origin);

            int currentKey = 0;

            while (dungeon.RoomCount < numRooms)
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
                DungeonRoom currentRoom = startingRoom;

                for (int i = 0; i < runLength; i++)
                {
                    if (dungeon.RoomCount >= numRooms)
                        break;

                    if (!currentRoom.CanAddAnyRooms())
                        break;

                    // Pick a direction.
                    var directions = currentRoom.UnusedDoors();
                    int index = rng.Next(0, directions.Length);
                    CardinalDirection dir = directions[index];

                    // Lock the door, if it's the start of the run
                    if (i == 0)
                    {
                        currentRoom.GetDoor(dir).LockId = currentKey;
                    }

                    // Create a new room in that direction
                    currentRoom = currentRoom.AddNeighbor(dir);
                }

                // Place a key at the end of this run
                currentKey++;
                currentRoom.KeyId = currentKey;
            }
        }
    }
}
