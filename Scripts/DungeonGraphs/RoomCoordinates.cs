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

        /// <summary>
        /// If the given coordinates are adjacent to this one, returns which
        /// direction you'd need to walk to go from this one to the other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public CardinalDirection AdjacentDirection(RoomCoordinates other)
        {
            if (!other.IsAdjacentTo(this))
                throw new Exception($"{this} is not adjacent to {other}");

            if (other.X > this.X)
                return CardinalDirection.East;
            else if (other.X < this.X)
                return CardinalDirection.West;
            else if (other.Y > this.Y)
                return CardinalDirection.North;
            else
                return CardinalDirection.South;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
