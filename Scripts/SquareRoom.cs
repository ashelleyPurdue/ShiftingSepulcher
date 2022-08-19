using System;
using Godot;
using RandomDungeons.DungeonGraphs;

namespace RandomDungeons
{
    public class SquareRoom : Node2D
    {
        public Door GetDoor(CardinalDirection dir)
        {
            switch (dir)
            {
                case CardinalDirection.North: return GetNode<Door>("%NorthDoor");
                case CardinalDirection.South: return GetNode<Door>("%SouthDoor");
                case CardinalDirection.East: return GetNode<Door>("%EastDoor");
                case CardinalDirection.West: return GetNode<Door>("%WestDoor");
            }

            throw new Exception("There are only four door directions.");
        }
    }

}
