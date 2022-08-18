using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomDungeons.DungeonGraphs
{
    public static class DungeonGenerator
    {
        public static DungeonRoom GenerateGraph(int seed, int numRooms)
        {
            if (numRooms < 1)
                throw new Exception("You can't have a zero-room dungeon.");

            var root = new DungeonRoom();
            var rng = new Random(seed);

            var allRooms = new Dictionary<(int x, int y), DungeonRoom>();
            allRooms[(0, 0)] = root;

            for (int i = 0; i < numRooms; i++)
            {
                // Pick a random unused door and add a room to it.
                var unusedDoors = UnusedDoors(allRooms.Values).ToArray();
                int doorIndex = rng.Next(0, unusedDoors.Length);
                var door = unusedDoors[doorIndex];

                var newRoom = new DungeonRoom();
                newRoom.RoomSeed = rng.Next();

                newRoom.Doors[OppositeDir(door.dir)] = door.room;
                door.room.Doors[door.dir] = newRoom;
            }

            return root;
        }

        private static IEnumerable<(DungeonRoom room, DoorDirection dir)> UnusedDoors(
            IEnumerable<DungeonRoom> rooms
        )
        {
            foreach (var room in rooms)
            {
                foreach (var dir in AllDirections())
                {
                    if (!room.Doors.ContainsKey(dir))
                        yield return (room, dir);
                }
            }
        }

        private static IEnumerable<DoorDirection> AllDirections()
        {
            var uncasted = Enum.GetValues(typeof(DoorDirection));

            foreach (var dir in uncasted)
            {
                yield return (DoorDirection)dir;
            }
        }

        private static DoorDirection OppositeDir(DoorDirection dir)
        {
            switch (dir)
            {
                case DoorDirection.North: return DoorDirection.South;
                case DoorDirection.South: return DoorDirection.North;
                case DoorDirection.East: return DoorDirection.West;
                case DoorDirection.West: return DoorDirection.East;
            }

            throw new Exception("There are only four cardinal directions, dude.");
        }
    }
}