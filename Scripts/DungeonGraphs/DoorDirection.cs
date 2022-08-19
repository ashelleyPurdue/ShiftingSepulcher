using System;
using System.Collections.Generic;

namespace RandomDungeons.DungeonGraphs
{
    public enum DoorDirection
    {
        North,
        South,
        East,
        West
    }

    public static class DoorDirectionExtensions
    {
        public static DoorDirection Opposite(this DoorDirection dir)
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

    public static class DoorDirectionUtils
    {
        public static IEnumerable<DoorDirection> All()
        {
            var uncasted = Enum.GetValues(typeof(DoorDirection));

            foreach (var dir in uncasted)
            {
                yield return (DoorDirection)dir;
            }
        }
    }
}
