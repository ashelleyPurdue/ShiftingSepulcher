using System;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class HandCraftedDungeonInstantiator : Node2D
    {
        [Export] public NodePath StartRoomPath;

        private RoomTransitionManager _transitionManager => GetNode<RoomTransitionManager>("%RoomTransitionManager");

        public override void _Ready()
        {
            // Connect all the doors
            foreach (var door in this.AllDescendantsOfType<WarpTrigger>())
            {
                if (door.TargetRoomPath == null)
                    throw new Exception($"Door has no target room: {door.GetPath()}");

                door.TargetRoom = door.GetNode<Room2D>(door.TargetRoomPath);
            }

            // Remove all the rooms from the scene tree, and then start the
            // dungeon
            var startRoom = GetNode<Room2D>(StartRoomPath);
            var allRooms = this.AllDescendantsOfType<Room2D>().ToArray();

            foreach (var room in allRooms)
            {
                RemoveChild(room);
            }

            _transitionManager.StartDungeon(startRoom, allRooms);
        }
    }
}
