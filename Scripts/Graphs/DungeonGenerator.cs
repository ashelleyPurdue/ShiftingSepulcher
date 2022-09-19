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
            int currentKey = 0;
            int sequenceNum = 0;

            var lastCreatedRoom = dungeon.CreateRoom(Vector2i.Zero, sequenceNum);
            sequenceNum++;

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

            // Demolish walls and build one-way doors to create shortcuts from
            // late rooms to early rooms.
            foreach (var pos in dungeon.AllRoomCoordinates())
            {
                var room = dungeon.GetRoom(pos);
                foreach (var dir in room.PotentialOneWayDoors())
                {
                    room.AddOneWayDoor(dir);
                }
            }

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

            IEnumerable<(CardinalDirection dir, int weight)> NextRoomDirectionWeights(DungeonGraphRoom room)
            {
                foreach (var dir in room.UnusedDoors())
                {
                    Vector2i pos = room.Position.Adjacent(dir);

                    if (dungeon.CoordinatesInUse(pos))
                        continue;

                    yield return (dir, dungeon.SurroundingRoomCount(pos));
                }
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
                    CardinalDirection dir = rng.PickFromWeighted(
                        NextRoomDirectionWeights(currentRoom).ToArray()
                    );

                    // Lock the door, if it's the start of the run
                    if (i == 0 && currentKey != 0)
                    {
                        currentRoom.SetDoor(
                            dir,
                            new KeyDungeonGraphDoor(currentKey)
                        );
                    }

                    // Create a new room in that direction
                    currentRoom = currentRoom.AddNeighbor(dir, sequenceNum);
                    lastCreatedRoom = currentRoom;
                    sequenceNum++;

                    // Choose a random challenge type for this room
                    // TODO: Don't hardcode these probabilities
                    currentRoom.ChallengeType = rng.PickFromWeighted(
                        (ChallengeType.None, 1),
                        (ChallengeType.Combat, 1),
                        (ChallengeType.Puzzle, 1)
                    );
                    currentRoom.RoomSeed = rng.Next();
                }

                // Place a key at the end of this run
                currentKey++;
                currentRoom.KeyId = currentKey;
                currentRoom.ChallengeType = ChallengeType.Loot;
            }
        }
    }
}
