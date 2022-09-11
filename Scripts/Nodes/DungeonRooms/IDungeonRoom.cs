using System;
using Godot;

using RandomDungeons.Utils;
using RandomDungeons.Graphs;

namespace RandomDungeons.Nodes.DungeonRooms
{
    // I know, I know.  This is an abstract class, but I prefixed its name with
    // "I".  I _really_ wanted this to be an interface, but I couldn't because
    // it needs to inherit from Node2D.
    public abstract class IDungeonRoom : Node2D
    {
        public abstract event Action<CardinalDirection> DoorUsed;

        public abstract DungeonGraphRoom GraphRoom {get; protected set;}
        public abstract float FadePercent {get; set;}

        public abstract Node2D GetDoorSpawn(CardinalDirection dir);
    }
}
