using System;
using System.Collections.Generic;
using RandomDungeons.DungeonTrees;
using RandomDungeons.Utils;

namespace RandomDungeons.Graphs
{
    public static class DungeonGraphBuilder
    {
        public static DungeonGraph BuildFromTree(DungeonTreeRoom root)
        {
            var cardDirs = new[]
            {
                CardinalDirection.South,
                CardinalDirection.East
            };

            var graph = new DungeonGraph();
            var startRoom = graph.CreateRoom(Vector2i.Zero, 0);
            CreateGraphRoom(root, startRoom, cardDirs[0]);

            return graph;

            void CreateGraphRoom(
                DungeonTreeRoom treeRoom,
                DungeonGraphRoom parent,
                CardinalDirection dir)
            {
                var graphRoom = parent.CreateNeighbor(dir, 0);
                graphRoom.RoomSeed      = treeRoom.RoomSeed;
                graphRoom.ChallengeType = treeRoom.ChallengeType;

                for (int i = 0; i < treeRoom.ChildDoors.Count; i++)
                {
                    var childDir = cardDirs[i];
                    DungeonTreeRoom childTreeRoom = treeRoom.ChildDoors[i].Destination;

                    CreateGraphRoom(
                        childTreeRoom,
                        graphRoom,
                        childDir
                    );
                }
            }
        }

        private static Dictionary<Vector2i, DungeonTreeRoom> LayoutFromTree(DungeonTreeRoom root)
        {
            var cardDirs = new[]
            {
                CardinalDirection.South,
                CardinalDirection.East
            };

            var layout = new Dictionary<Vector2i, DungeonTreeRoom>();
            var startRoom = new DungeonTreeRoom();
            layout[Vector2i.Zero] = startRoom;

            startRoom.RoomSeed = 0;
            startRoom.ChallengeType = ChallengeType.None;
            startRoom.AddChallengeDoor(root);

            AddToLayout(root, Vector2i.Zero, cardDirs[0]);
            return layout;

            void AddToLayout(
                DungeonTreeRoom treeRoom,
                Vector2i parentPos,
                CardinalDirection dir)
            {
                var newPos = parentPos.Adjacent(dir);
                if (layout.ContainsKey(newPos))
                    throw new Exception("Layout: there is already a room there");

                for (int i = 0; i < treeRoom.ChildDoors.Count; i++)
                {
                    var childDir = cardDirs[i];
                    DungeonTreeRoom childTreeRoom = treeRoom.ChildDoors[i].Destination;

                    AddToLayout(
                        childTreeRoom,
                        newPos,
                        childDir
                    );
                }
            }
        }
    }
}