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
    }
}
