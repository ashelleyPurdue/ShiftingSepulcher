using System;

namespace RandomDungeons.DungeonTrees
{
    public class DungeonTreeException : DungeonGenerationException
    {
        public DungeonTreeException(string msg) : base(msg) {}
    }
}
