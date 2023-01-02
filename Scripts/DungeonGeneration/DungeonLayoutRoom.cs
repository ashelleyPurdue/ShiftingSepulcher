namespace RandomDungeons
{
    public readonly struct DungeonLayoutRoom
    {
        public readonly DungeonLayout Layout;
        public readonly Vector2i Position;
        public DungeonTreeRoom TreeRoom => Layout.RoomAt(Position);

        public DungeonLayoutRoom(DungeonLayout layout, Vector2i position)
        {
            Layout = layout;
            Position = position;
        }
    }
}
