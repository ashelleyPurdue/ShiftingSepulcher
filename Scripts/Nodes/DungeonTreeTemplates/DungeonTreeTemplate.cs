using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using RandomDungeons.DungeonTrees;
using RandomDungeons.DungeonLayouts;
using RandomDungeons.Graphs;
using RandomDungeons.Nodes.UI.Widgets.Minimap;

namespace RandomDungeons.Nodes.TreeTemplates
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

            var tree = ToDungeonTree(new Random(1337));
            var layout = DungeonLayoutBuilder.LayoutFromTree(tree);
            var graph = DungeonGraphBuilder.BuildFromLayout(layout);

            _minimapInEditor.SetGraph(graph);
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
                var shortcutNode = roomNode.GetNode<DungeonTreeTemplateRoom>(shortcutPath);
                var shortcutRoom = nodeToRoom[shortcutNode];
                room.AddOutgoingShortcut(shortcutRoom);
            }

            foreach (var child in ChildRoomNodes(roomNode))
            {
                ConnectShortcuts(child, nodeToRoom);
            }
        }

        private IEnumerable<DungeonTreeTemplateRoom> ChildRoomNodes(DungeonTreeTemplateRoom roomNode)
        {
            return roomNode.GetChildren().Cast<DungeonTreeTemplateRoom>();
        }
    }
}
