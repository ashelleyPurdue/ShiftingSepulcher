using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomDungeons.DungeonGraphs
{
    public static class DungeonGenerator
    {
        public static Dictionary<RoomCoordinates, DungeonRoom> GenerateGraph(int seed, int numRooms)
        {
            if (numRooms < 1)
                throw new Exception("You can't have a zero-room dungeon.");

            var root = new DungeonRoom();
            var rng = new Random(seed);

            var allRooms = new Dictionary<RoomCoordinates, DungeonRoom>();
            allRooms[RoomCoordinates.Origin] = root;

            for (int i = 0; i < numRooms; i++)
            {
                // Pick a random unused door and add a room to it.
                var unusedDoors = UnusedDoors(allRooms).ToArray();
                int doorIndex = rng.Next(0, unusedDoors.Length);
                (RoomCoordinates parentCoords, CardinalDirection dir) = unusedDoors[doorIndex];

                RoomCoordinates childCoords = parentCoords.Adjacent(dir);

                DungeonRoom parentRoom = allRooms[parentCoords];
                DungeonRoom childRoom = new DungeonRoom();
                childRoom.RoomSeed = rng.Next();

                parentRoom.Doors[dir] = childRoom;
                childRoom.Doors[dir.Opposite()] = parentRoom;
                allRooms[childCoords] = childRoom;
            }

            return allRooms;
        }

        private static IEnumerable<(RoomCoordinates parentCoords, CardinalDirection dir)> UnusedDoors(
            Dictionary<RoomCoordinates, DungeonRoom> allRooms
        )
        {
            foreach (var roomCoords in allRooms.Keys)
            {
                foreach (var dir in CardinalDirectionUtils.All())
                {
                    if (!allRooms.ContainsKey(roomCoords.Adjacent(dir)))
                        yield return (roomCoords, dir);
                }
            }
        }
    }
}
