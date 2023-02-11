using System;

namespace ShiftingSepulcher
{
    public interface IRoomPopulator
    {
        void Populate(DungeonTreeRoom graphRoom, Random rng);
    }
}
