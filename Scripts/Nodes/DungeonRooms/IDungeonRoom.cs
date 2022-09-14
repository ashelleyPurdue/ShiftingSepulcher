using System;
using Godot;

using RandomDungeons.Utils;
using RandomDungeons.Graphs;

namespace RandomDungeons.Nodes.DungeonRooms
{
    public interface IDungeonRoom
    {
        Node2D Node {get;}

        event Action<CardinalDirection> DoorUsed;

        DungeonGraphRoom GraphRoom {get;}
        float FadePercent {get; set;}

        Node2D GetDoorSpawn(CardinalDirection dir);

        void Populate(DungeonGraphRoom graphRoom);
    }
}
