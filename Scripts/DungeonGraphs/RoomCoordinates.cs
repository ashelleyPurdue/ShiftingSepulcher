using System;

namespace RandomDungeons.DungeonGraphs
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

        public RoomCoordinates Adjacent(CardinalDirection dir)
        {
            RoomCoordinates clone = this;

            switch (dir)
            {
                case CardinalDirection.North: clone.Y += 1; break;
                case CardinalDirection.South: clone.Y -= 1; break;
                case CardinalDirection.East: clone.X += 1; break;
                case CardinalDirection.West: clone.X -= 1; break;
            }

            return clone;
        }

        public bool IsAdjacentTo(RoomCoordinates other)
        {
            int xDiff = Math.Abs(this.X - other.X);
            int yDiff = Math.Abs(this.Y - other.Y);

            return (xDiff == 1 && yDiff == 0) || (xDiff == 0 && yDiff == 1);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
