using System;
using Godot;
using RandomDungeons.DungeonGraphs;

namespace RandomDungeons
{
    public class SquareRoom : Node2D
    {
        public Door GetDoor(DoorDirection dir)
        {
            switch (dir)
            {
                case DoorDirection.North: return GetNode<Door>("%NorthDoor");
                case DoorDirection.South: return GetNode<Door>("%SouthDoor");
                case DoorDirection.East: return GetNode<Door>("%EastDoor");
                case DoorDirection.West: return GetNode<Door>("%WestDoor");
            }

            throw new Exception("There are only four door directions.");
        }
    }

}