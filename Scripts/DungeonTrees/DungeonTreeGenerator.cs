using System;
using RandomDungeons.Utils;

namespace RandomDungeons.DungeonTrees
{
    public static class DungeonTreeGenerator
    {
        public static DungeonTreeRoom GenerateUsingRuns(
            int seed,
            int numRuns = 5,
            int minRunLength = 1,
            int maxRunLength = 3
        )
        {
            var rng = new Random(seed);
            DungeonTreeRoom root = new DungeonTreeRoom();
            root.ChallengeType = Graphs.ChallengeType.None;

            DungeonTreeRoom prevRunRoot = root;
            for (int i = 0; i < numRuns; i++)
            {
                var runRoot = GenerateRun(rng.Next(minRunLength, maxRunLength));
                prevRunRoot.AddChallengeDoor(runRoot);
                prevRunRoot = runRoot;
            }

            return root;

            DungeonTreeRoom GenerateRun(int runLength)
            {
                DungeonTreeRoom runRoot = null;
                DungeonTreeRoom prevRoom = null;

                for (int i = 0; i < runLength; i++)
                {
                    var room = new DungeonTreeRoom();
                    room.RoomSeed = rng.Next();
                    room.ChallengeType = rng.PickFromWeighted(
                        (Graphs.ChallengeType.None, 1),
                        (Graphs.ChallengeType.Combat, 1),
                        (Graphs.ChallengeType.Puzzle, 1)
                    );

                    if (prevRoom == null)
                    {
                        runRoot = room;
                    }
                    else
                    {
                        prevRoom.AddChallengeDoor(room);
                    }

                    prevRoom = room;
                }

                return runRoot;
            }
        }
    }
}