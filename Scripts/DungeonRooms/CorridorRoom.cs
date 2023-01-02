using System;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class CorridorRoom : SimpleDungeonRoom
    {
        public override Node2D GetDoorSpawn(CardinalDirection dir)
        {
            string path = LayoutRoom.DoorAtDirection(dir)?.Destination == null
                ? $"%Corridors/{dir}/ShortenedDoorSpawn"
                : $"%Corridors/{dir}/DoorSpawn";

            return GetNode<Node2D>(path);
        }

        public override void Populate(DungeonLayoutRoom layoutRoom)
        {
            base.Populate(layoutRoom);

            foreach (var dir in CardinalDirectionUtils.All())
            {
                var treeDoor = layoutRoom.DoorAtDirection(dir);
                var corridor = GetNode<Node2D>($"%Corridors/{dir}/Visuals");
                corridor.Visible = treeDoor?.Destination != null;
            }
        }
    }
}
