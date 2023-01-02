using System;

namespace RandomDungeons
{
    public interface IRoomPopulator
    {
        void Populate(DungeonTreeRoom graphRoom, Random rng);
    }
}
