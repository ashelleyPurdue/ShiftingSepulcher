using System;

namespace RandomDungeons
{
    public class DungeonLayoutException : DungeonGenerationException
    {
        public DungeonLayoutException(string msg) : base(msg) {}
    }
}
