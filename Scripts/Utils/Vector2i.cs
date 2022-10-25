using System;

namespace RandomDungeons
{
    public struct Vector2i
    {
        public int x;
        public int y;

        public static Vector2i Zero => new Vector2i(0, 0);
        public static Vector2i One => new Vector2i(1, 1);

        public static Vector2i Up => new Vector2i(0, 1);
        public static Vector2i Down => new Vector2i(0, -1);
        public static Vector2i Left => new Vector2i(-1, 0);
        public static Vector2i Right => new Vector2i(1, 0);

        public Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Vector2i a, Vector2i b)
        {
            return (a.x == b.x) && (a.y == b.y);
        }

        public static bool operator !=(Vector2i a, Vector2i b)
        {
            return !(a == b);
        }

        public static Vector2i operator +(Vector2i a, Vector2i b)
        {
            return new Vector2i(
                a.x + b.x,
                a.y + b.y
            );
        }

        public static Vector2i operator -(Vector2i a, Vector2i b)
        {
            return a + (b * -1);
        }

        public static Vector2i operator *(Vector2i v, int scalar)
        {
            return new Vector2i(
                v.x * scalar,
                v.y * scalar
            );
        }

        public static Vector2i operator *(int scalar, Vector2i v)
        {
            return v * scalar;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2i other))
                return false;

            return this == other;
        }

        public override int GetHashCode()
        {
            return (x, y).GetHashCode();
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}
