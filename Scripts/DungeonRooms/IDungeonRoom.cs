using System;
using System.Collections.Generic;
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

        void ConnectDoors(Dictionary<DungeonGraphRoom, Room2D> graphRoomToRealRoom);

        bool IsChallengeSolved();
    }
}
