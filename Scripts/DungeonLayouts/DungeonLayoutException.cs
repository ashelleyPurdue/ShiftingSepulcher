using System;

namespace RandomDungeons.DungeonLayouts
{
    public class DungeonLayoutException : DungeonGenerationException
    {
        public DungeonLayoutException(string msg) : base(msg) {}
    }
}
