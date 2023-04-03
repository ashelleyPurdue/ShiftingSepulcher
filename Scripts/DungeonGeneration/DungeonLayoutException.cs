using System;

namespace ShiftingSepulcher
{
    public class DungeonLayoutException : DungeonGenerationException
    {
        public DungeonLayoutException(string msg) : base(msg) {}
    }
}
