using System;
using System.Linq;

namespace RandomDungeons
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
            root.ChallengeType = ChallengeType.None;

            DungeonTreeRoom prevRunRoot = root;
            for (int i = 0; i < numRuns; i++)
            {
                int runLength = rng.Next(minRunLength, maxRunLength);
                var runRoot = GenerateRun(i, runLength);

                // Pick a random room to start this run in
                // TODO: Bias it toward rooms that are close to the previous
                // run's root, to minimize backtracking
                var availableRooms = root
                    .AllDescendants()
                    .Where(r => r.ChildDoors.Count < 3);
                var runStartRoom = rng.PickFrom(availableRooms);

                // Lock the door to this run, unless it's the first run of the
                // dungeon
                if (i != 0)
                    runStartRoom.AddLockedDoor(runRoot, i);
                else
                    runStartRoom.AddChallengeDoor(runRoot);

                prevRunRoot = runRoot;
            }

            // Mark the final room of the final run as the boss room
            var bossRoom = prevRunRoot
                .AllDescendants()
                .Last();

            bossRoom.ChallengeType = ChallengeType.Boss;
            bossRoom.KeyId = 0;

            return root;

            DungeonTreeRoom GenerateRun(int runNumber, int runLength)
            {
                DungeonTreeRoom runRoot = null;
                DungeonTreeRoom prevRoom = null;

                for (int i = 0; i < runLength; i++)
                {
                    var room = new DungeonTreeRoom();
                    room.RoomSeed = rng.Next();
                    room.ChallengeType = rng.PickFromWeighted(
                        (ChallengeType.None, 2),
                        (ChallengeType.Combat, 1),
                        (ChallengeType.Puzzle, 2)
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

                // Place a key in the last room of the run
                prevRoom.KeyId = runNumber + 1;
                prevRoom.ChallengeType = ChallengeType.Loot;

                return runRoot;
            }
        }
    }
}
