using System;

namespace RandomDungeons
{
    public class DungeonGraphException : DungeonGenerationException
    {
        public DungeonGraphException(string msg) : base(msg) {}
    }
}
