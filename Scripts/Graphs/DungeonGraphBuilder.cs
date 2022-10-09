using System;
using RandomDungeons.DungeonTrees;
using RandomDungeons.Utils;

namespace RandomDungeons.Graphs
{
    public static class DungeonGraphBuilder
    {
        public static DungeonGraph BuildFromTree(DungeonTreeRoom root)
        {
            var graph = new DungeonGraph();
            var startRoom = graph.CreateRoom(Vector2i.Zero, 0);
            CreateGraphRoom(root, startRoom, (CardinalDirection)0);

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
                    var childDir = (CardinalDirection)i;
                    DungeonTreeRoom childTreeRoom = treeRoom.ChildDoors[i].Destination;

                    CreateGraphRoom(
                        childTreeRoom,
                        graphRoom,
                        childDir
                    );
                }
            }
        }
    }
}