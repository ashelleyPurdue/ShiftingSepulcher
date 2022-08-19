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

        public RoomCoordinates Adjacent(DoorDirection dir)
        {
            RoomCoordinates clone = this;

            switch (dir)
            {
                case DoorDirection.North: clone.Y += 1; break;
                case DoorDirection.South: clone.Y -= 1; break;
                case DoorDirection.East: clone.X += 1; break;
                case DoorDirection.West: clone.X -= 1; break;
            }

            return clone;
        }
    }
}
