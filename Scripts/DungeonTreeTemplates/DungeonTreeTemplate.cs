using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    [Tool]
    public class DungeonTreeTemplate : Node
    {
        [Export] public bool RefreshPreview
        {
            get => false;
            set
            {
                RefreshPreviewInEditor();
            }
        }

        private DungeonTreeTemplateRoom _root => GetNode<DungeonTreeTemplateRoom>("Root");
        private PackedScene _minimapPrefab => GD.Load<PackedScene>("res://Scenes/Prefabs/UI/Widgets/Minimap/Minimap.tscn");
        private Minimap _minimapInEditor;

        public void RefreshPreviewInEditor()
        {
            if (!Engine.EditorHint)
                return;

            if (_minimapInEditor == null)
            {
                _minimapInEditor = _minimapPrefab.Instance<Minimap>();
                AddChild(_minimapInEditor);
            }

            try
            {
                var tree = ToDungeonTree(new Random(1337));
                var layout = DungeonLayoutBuilder.LayoutFromTree(tree);

                _minimapInEditor.SetLayout(layout);
            }
            catch (DungeonGenerationException e)
            {
                GD.PrintErr(e);
                CleanUpAfterError();
            }
            catch (Exception e)
            {
                GD.PrintErr("Unexpected exception trying to preview this tree:");
                GD.PrintErr(e);
                CleanUpAfterError();
            }

            void CleanUpAfterError()
            {
                RemoveChild(_minimapInEditor);
                _minimapInEditor.QueueFree();
                _minimapInEditor = null;
            }
        }

        public DungeonTreeRoom ToDungeonTree(Random rng)
        {
            var nodeToRoom = new Dictionary<DungeonTreeTemplateRoom, DungeonTreeRoom>();
            var treeRoot = TreeFromNode(rng, _root, nodeToRoom);
            ConnectShortcuts(_root, nodeToRoom);

            return treeRoot;
        }

        private DungeonTreeRoom TreeFromNode(
            Random rng,
            DungeonTreeTemplateRoom roomNode,
            Dictionary<DungeonTreeTemplateRoom, DungeonTreeRoom> nodeToRoom
        )
        {
            var room = new DungeonTreeRoom();
            nodeToRoom[roomNode] = room;

            room.RoomSeed = rng.Next();
            room.KeyId = roomNode.KeyId;
            room.ChallengeType = roomNode.ChallengeType;

            foreach (var child in ChildRoomNodes(roomNode))
            {
                var childRoom = TreeFromNode(rng, child, nodeToRoom);

                if (child.LockId > 0)
                    room.AddLockedDoor(childRoom, child.LockId);
                else
                    room.AddChallengeDoor(childRoom);
            }

            return room;
        }

        private void ConnectShortcuts(
            DungeonTreeTemplateRoom roomNode,
            Dictionary<DungeonTreeTemplateRoom, DungeonTreeRoom> nodeToRoom
        )
        {
            var room = nodeToRoom[roomNode];

            foreach (var shortcutPath in roomNode.Shortcuts)
            {
                var shortcutNode = roomNode.GetNode(shortcutPath);

                if (!IsAParentOf(shortcutNode))
                {
                    string msg =
                        $"Shortcut target {shortcutNode.Name} is not a child " +
                        $"of {Name}";

                    throw new DungeonTreeException(msg);
                }

                if (!(shortcutNode is DungeonTreeTemplateRoom))
                {
                    string msg =
                        $"Shortcut target {shortcutNode.Name} is a " +
                        $"{shortcutNode.GetType()}, not a DungeonTreeTemplateRoom";

                    throw new DungeonTreeException(msg);
                }

                var shortcutRoom = nodeToRoom[(DungeonTreeTemplateRoom)shortcutNode];
                room.AddOutgoingShortcut(shortcutRoom);
            }

            foreach (var child in ChildRoomNodes(roomNode))
            {
                ConnectShortcuts(child, nodeToRoom);
            }
        }

        private IEnumerable<DungeonTreeTemplateRoom> ChildRoomNodes(DungeonTreeTemplateRoom roomNode)
        {
            return roomNode
                .EnumerateChildren()
                .Cast<DungeonTreeTemplateRoom>();
        }
    }
}
