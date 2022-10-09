using System;
using System.Collections.Generic;

namespace RandomDungeons.DungeonTrees
{
    public class DungeonTreeRoom
    {
        public int RoomSeed;
        public Graphs.ChallengeType ChallengeType;
        public List<IDungeonTreeDoor> ChildDoors = new List<IDungeonTreeDoor>();

        public void AddChallengeDoor(DungeonTreeRoom room)
        {
            var door = new ChallengeDoor();
            door.Destination = room;
            ChildDoors.Add(door);
        }
    }
}