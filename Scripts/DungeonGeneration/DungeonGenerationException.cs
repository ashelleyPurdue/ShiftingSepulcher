using System;

namespace RandomDungeons
{
    public class DungeonGenerationException : Exception
    {
        public DungeonGenerationException(string msg) : base(msg) {}
    }
}
