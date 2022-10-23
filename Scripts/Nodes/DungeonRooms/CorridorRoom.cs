using System;
using System.Linq;
using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.DungeonRooms
{
    public class CorridorRoom : SimpleDungeonRoom
    {
        public override Node2D GetDoorSpawn(CardinalDirection dir)
        {
            string path = GraphRoom.GetDoor(dir).Destination == null
                ? $"%Corridors/{dir}/ShortenedDoorSpawn"
                : $"%Corridors/{dir}/DoorSpawn";

            return GetNode<Node2D>(path);
        }
    }
}