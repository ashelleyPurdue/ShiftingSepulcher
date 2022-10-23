using System;
using System.Linq;
using Godot;

using RandomDungeons.Nodes.Elements.Enemies;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.DungeonRooms
{
    public class CorridorRoom : SimpleDungeonRoom
    {
        public override Node2D GetDoorSpawn(CardinalDirection dir)
        {
            return GetNode<Node2D>($"%Corridors/{dir}/DoorSpawn");
        }
    }
}