using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using RandomDungeons.DungeonTrees;

namespace RandomDungeons.Nodes.TreeTemplates
{
    public class DungeonTreeTemplateRoom : Node
    {
        [Export] public Graphs.ChallengeType ChallengeType;
        [Export] public int KeyId;
        [Export] public int LockId;
        [Export] public NodePath[] Shortcuts = new NodePath[0];

        public DungeonTreeRoom ToDungeonTreeRoom(Random rng)
        {
            // Problem: we need to convert the NodePaths in Shortcuts into
            // DunegonTreeRooms, but the node the path points to may not yet
            // have had its DungeonTreeRoom created yet.
            //
            // Solution: Do two passes.  The first pass creates all the
            // DungeonTreeRooms and associates them with their respective
            // DungeonTreeTemplateRoom.  The second pass uses the resulting
            // dictionary to convert all the NodePaths to DungeonTreeRooms.
            var nodeToRoom = new Dictionary<DungeonTreeTemplateRoom, DungeonTreeRoom>();
            var room = ToDungeonTreeRoom(rng, nodeToRoom);

            ConnectShortcuts(nodeToRoom);
            return room;
        }

        private DungeonTreeRoom ToDungeonTreeRoom(
            Random rng,
            Dictionary<DungeonTreeTemplateRoom, DungeonTreeRoom> nodeToRoom
        )
        {
            var room = new DungeonTreeRoom();
            nodeToRoom[this] = room;

            room.RoomSeed = rng.Next();
            room.KeyId = KeyId;
            room.ChallengeType = ChallengeType;

            foreach (var child in ChildRoomNodes())
            {
                var childRoom = child.ToDungeonTreeRoom(rng, nodeToRoom);

                if (child.LockId > 0)
                    room.AddLockedDoor(childRoom, child.LockId);
                else
                    room.AddChallengeDoor(childRoom);
            }

            return room;
        }

        private void ConnectShortcuts(Dictionary<DungeonTreeTemplateRoom, DungeonTreeRoom> nodeToRoom)
        {
            var room = nodeToRoom[this];

            foreach (var shortcutPath in Shortcuts)
            {
                var shortcutNode = GetNode<DungeonTreeTemplateRoom>(shortcutPath);
                var shortcutRoom = nodeToRoom[shortcutNode];
                room.AddOutgoingShortcut(shortcutRoom);
            }

            foreach (var child in ChildRoomNodes())
            {
                child.ConnectShortcuts(nodeToRoom);
            }
        }

        private IEnumerable<DungeonTreeTemplateRoom> ChildRoomNodes()
        {
            return GetChildren().Cast<DungeonTreeTemplateRoom>();
        }
    }
}
