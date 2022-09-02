using System;
using System.Collections.Generic;
using System.Linq;

using RandomDungeons.Utils;

namespace RandomDungeons.Graphs
{
    public static class DungeonGenerator
    {
        public static DungeonGraph GenerateUsingRuns(
            int seed,
            int minRunSize,
            int maxRunSize,
            int numRooms
        )
        {
            var rng = new Random(seed);
            var dungeon = new DungeonGraph();
            var lastCreatedRoom = dungeon.CreateRoom(Vector2i.Zero);

            int currentKey = 0;

            while (dungeon.RoomCount < numRooms)
            {
                DungeonGraphRoom runStartRoom = ChooseRandomStartRoom();
                int runLength = rng.Next(minRunSize, maxRunSize + 1);

                GenerateRun(runStartRoom, runLength);
            }

            // Mark the last-created room as a boss room.
            // If it has a key, remove it, since we won't have created the lock
            // that goes with it.
            lastCreatedRoom.ChallengeType = ChallengeType.Boss;
            lastCreatedRoom.KeyId = 0;

            return dungeon;

            DungeonGraphRoom ChooseRandomStartRoom()
            {
                var possibleStartingRooms = dungeon
                    .AllRoomCoordinates()
                    .Select(c => dungeon.GetRoom(c))
                    .Where(r => r.CanAddAnyRooms())
                    .ToArray();

                int index = rng.Next(0, possibleStartingRooms.Length);
                return possibleStartingRooms[index];
            }

            void GenerateRun(DungeonGraphRoom startingRoom, int runLength)
            {
                DungeonGraphRoom currentRoom = startingRoom;

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
                    lastCreatedRoom = currentRoom;
                    currentRoom.RoomSeed = rng.Next();
                }

                // Place a key at the end of this run
                currentKey++;
                currentRoom.KeyId = currentKey;
            }
        }
    }
}
