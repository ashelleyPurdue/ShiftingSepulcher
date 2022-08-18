using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomDungeons.DungeonGraphs
{
    public static class DungeonGenerator
    {
        public struct RoomCoordinates
        {
            public int X;
            public int Y;

            public static RoomCoordinates Origin => new RoomCoordinates
            {
                X = 0,
                Y = 0
            };

            public RoomCoordinates Adjacent(DoorDirection dir)
            {
                RoomCoordinates clone = this;

                switch (dir)
                {
                    case DoorDirection.North: clone.Y += 1; break;
                    case DoorDirection.South: clone.Y -= 1; break;
                    case DoorDirection.East: clone.X += 1; break;
                    case DoorDirection.West: clone.X -= 1; break;
                }

                return clone;
            }
        }

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
                (RoomCoordinates parentCoords, DoorDirection dir) = unusedDoors[doorIndex];

                RoomCoordinates childCoords = parentCoords.Adjacent(dir);

                DungeonRoom parentRoom = allRooms[parentCoords];
                DungeonRoom childRoom = new DungeonRoom();
                childRoom.RoomSeed = rng.Next();

                parentRoom.Doors[dir] = childRoom;
                childRoom.Doors[OppositeDir(dir)] = parentRoom;
                allRooms[childCoords] = childRoom;
            }

            return allRooms;
        }

        private static IEnumerable<(RoomCoordinates parentCoords, DoorDirection dir)> UnusedDoors(
            Dictionary<RoomCoordinates, DungeonRoom> allRooms
        )
        {
            foreach (var roomCoords in allRooms.Keys)
            {
                foreach (var dir in AllDirections())
                {
                    if (!allRooms.ContainsKey(roomCoords.Adjacent(dir)))
                        yield return (roomCoords, dir);
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
