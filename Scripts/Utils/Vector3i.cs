using System;

namespace ShiftingSepulcher
{
    public struct Vector3i
    {
        public int x;
        public int y;
        public int z;

        public static Vector3i Zero => new Vector3i(0, 0, 0);
        public static Vector3i One => new Vector3i(1, 1, 1);

        // TODO: "Up" and "Down" are currently backwards.
        // Need to figure out if it's safe to fix this.
        public static Vector3i Up => new Vector3i(0, 1, 0);
        public static Vector3i Down => new Vector3i(0, -1, 0);
        public static Vector3i Left => new Vector3i(-1, 0, 0);
        public static Vector3i Right => new Vector3i(1, 0, 0);
        public static Vector3i Forward => new Vector3i(0, 0, -1);
        public static Vector3i Back => new Vector3i(0, 0, 1);

        public Vector3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static bool operator ==(Vector3i a, Vector3i b)
        {
            return
                (a.x == b.x) &&
                (a.y == b.y) &&
                (a.z == b.z);
        }

        public static bool operator !=(Vector3i a, Vector3i b)
        {
            return !(a == b);
        }

        public static Vector3i operator +(Vector3i a, Vector3i b)
        {
            return new Vector3i(
                a.x + b.x,
                a.y + b.y,
                a.z + b.z
            );
        }

        public static Vector3i operator -(Vector3i a, Vector3i b)
        {
            return a + (b * -1);
        }

        public static Vector3i operator *(Vector3i v, int scalar)
        {
            return new Vector3i(
                v.x * scalar,
                v.y * scalar,
                v.z * scalar
            );
        }

        public static Vector3i operator *(int scalar, Vector3i v)
        {
            return v * scalar;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3i other))
                return false;

            return this == other;
        }

        public override int GetHashCode()
        {
            return (x, y, z).GetHashCode();
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }
    }
}
