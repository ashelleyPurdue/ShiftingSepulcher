using System;
using Godot;

namespace RandomDungeons
{
    public interface IDungeonRoom
    {
        Node2D Node {get;}

        event Action<CardinalDirection> DoorUsed;

        DungeonGraphRoom GraphRoom {get;}

        Node2D GetDoorSpawn(CardinalDirection dir);

        void Populate(DungeonGraphRoom graphRoom);

        bool IsChallengeSolved();
    }
}
