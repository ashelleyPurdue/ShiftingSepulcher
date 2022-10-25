using System;

namespace RandomDungeons
{
    public interface IRoomPopulator
    {
        void Populate(DungeonGraphRoom graphRoom, Random rng);
    }
}
