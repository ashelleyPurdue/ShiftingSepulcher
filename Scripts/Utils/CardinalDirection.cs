using System;
using System.Collections.Generic;
using Godot;

namespace RandomDungeons.Utils
{
    public enum CardinalDirection
    {
        North,
        South,
        East,
        West
    }

    public static class CardinalDirectionExtensions
    {
        public static CardinalDirection Opposite(this CardinalDirection dir)
        {
            switch (dir)
            {
                case CardinalDirection.North: return CardinalDirection.South;
                case CardinalDirection.South: return CardinalDirection.North;
                case CardinalDirection.East: return CardinalDirection.West;
                case CardinalDirection.West: return CardinalDirection.East;
            }

            throw new Exception("There are only four cardinal directions, dude.");
        }

        public static Vector2 ToVector2(this CardinalDirection dir)
        {
            switch (dir)
            {
                case CardinalDirection.North: return Vector2.Up;
                case CardinalDirection.South: return Vector2.Down;
                case CardinalDirection.East: return Vector2.Right;
                case CardinalDirection.West: return Vector2.Left;
                default: throw new Exception($"Unexpected cardinal direction {dir}");
            }
        }
    }

    public static class CardinalDirectionUtils
    {
        public static IEnumerable<CardinalDirection> All()
        {
            var uncasted = Enum.GetValues(typeof(CardinalDirection));

            foreach (var dir in uncasted)
            {
                yield return (CardinalDirection)dir;
            }
        }
    }
}
