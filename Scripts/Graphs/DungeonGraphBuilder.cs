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
            var coordsToRoom = LayoutFromTree(root);
            var roomToCoords = ReverseDict(coordsToRoom);

            var graph = new DungeonGraph();
            foreach (var pair in coordsToRoom)
            {
                Vector2i coords = pair.Key;
                DungeonTreeRoom treeRoom = pair.Value; 

                var graphRoom = graph.CreateRoom(coords, 0);
                graphRoom.ChallengeType = treeRoom.ChallengeType;
                graphRoom.RoomSeed = treeRoom.RoomSeed;

                // TODO: Set the doors
            }

            return graph;
        }

        private static Dictionary<TValue, TKey> ReverseDict<TKey, TValue>(
            Dictionary<TKey, TValue> dict
        )
        {
            var reversed = new Dictionary<TValue, TKey>();
            foreach (var pair in dict)
            {
                reversed[pair.Value] = pair.Key;
            }
            
            return reversed;
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

                layout[newPos] =  treeRoom;

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