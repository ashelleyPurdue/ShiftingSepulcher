using System;
using System.Collections.Generic;

namespace RandomDungeons.DungeonTrees
{
    public class DungeonTreeRoom
    {
        public int RoomSeed;
        public Graphs.ChallengeType ChallengeType;
        public List<IDungeonTreeDoor> ChildDoors = new List<IDungeonTreeDoor>();
    }
}