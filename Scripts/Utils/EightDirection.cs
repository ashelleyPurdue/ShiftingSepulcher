using Godot;

namespace RandomDungeons.Utils
{
    public enum EightDirection
    {
        North = 0,
        NorthEast = 1,
        East = 2,
        SouthEast = 3,
        South = 4,
        SouthWest = 5,
        West = 6,
        NorthWest = 7
    }

    public static class EightDirectionExtensions
    {
        public static Vector2 ToVector2(this EightDirection dir)
        {
            switch (dir)
            {
                case EightDirection.North: return Vector2.Up;
                case EightDirection.NorthEast: return (Vector2.Up + Vector2.Right).Normalized();
                case EightDirection.East: return Vector2.Right;
                case EightDirection.SouthEast: return (Vector2.Down + Vector2.Right).Normalized();
                case EightDirection.South: return Vector2.Down;
                case EightDirection.SouthWest: return (Vector2.Down + Vector2.Left).Normalized();
                case EightDirection.West: return Vector2.Left;
                case EightDirection.NorthWest: return (Vector2.Up + Vector2.Left).Normalized();

                default: return Vector2.Zero;
            }
        }
    }
}
