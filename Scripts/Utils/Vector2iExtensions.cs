using System;
using Godot;

namespace ShiftingSepulcher
{
    public static class Vector2iExtensions
    {
        public static Vector2i Adjacent(this Vector2i v, CardinalDirection dir)
        {
            return v + dir.ToVector2i();
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

        public static Vector2 ToVector2(this Vector2i v)
        {
            return new Vector2((float)v.x, (float)v.y);
        }

        public static Vector3i ToVector3i(this Vector2i v)
        {
            return new Vector3i(v.x, v.y, 0);
        }

        public static Vector3 ToVector3(this Vector2i v)
        {
            return new Vector3((float)v.x, (float)v.y, 0);
        }
    }
}
