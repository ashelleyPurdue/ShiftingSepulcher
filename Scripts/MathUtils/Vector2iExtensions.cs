using System;

namespace RandomDungeons.MathUtils
{
    public static class Vector2iExtensions
    {
        public static Vector2i Adjacent(this Vector2i v, CardinalDirection dir)
        {
            switch (dir)
            {
                case CardinalDirection.North: v.y += 1; break;
                case CardinalDirection.South: v.y -= 1; break;
                case CardinalDirection.East: v.x += 1; break;
                case CardinalDirection.West: v.x -= 1; break;
            }

            return v;
        }

        public static bool IsAdjacentTo(this Vector2i v, Vector2i other)
        {
            int xDiff = Math.Abs(v.x - other.x);
            int yDiff = Math.Abs(v.y - other.y);

            return (xDiff == 1 && yDiff == 0) || (xDiff == 0 && yDiff == 1);
        }

        /// <summary>
        /// If the given coordinates are adjacent to this one, returns which
        /// direction you'd need to walk to go from this one to the other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public static CardinalDirection AdjacentDirection(this Vector2i v, Vector2i other)
        {
            if (!other.IsAdjacentTo(v))
                throw new Exception($"{v} is not adjacent to {other}");

            if (other.x > v.x)
                return CardinalDirection.East;
            else if (other.x < v.x)
                return CardinalDirection.West;
            else if (other.y > v.y)
                return CardinalDirection.North;
            else
                return CardinalDirection.South;
        }
    }
}