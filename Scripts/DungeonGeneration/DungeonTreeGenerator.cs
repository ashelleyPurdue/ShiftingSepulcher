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
            for (int runNumber = 0; runNumber < numRuns; runNumber++)
            {
                int runLength = rng.Next(minRunLength, maxRunLength);
                var runRoot = GenerateRun(runNumber, runLength);

                // Pick a random room to start this run in.
                // Don't pick the room where we hid this run's key, because that
                // would result in a key being in the same room as the door it
                // unlocks, which would look silly.
                // TODO: Bias it toward rooms that are close to the previous
                // run's root, to minimize backtracking
                var availableRooms = root
                    .AllDescendants()
                    .Where(r => r.ChildDoors.Count < 3)
                    .Where(r => runNumber == 0 || r.KeyId != runNumber);
                var runStartRoom = rng.PickFrom(availableRooms);

                // Lock the door to this run, unless it's the first run of the
                // dungeon
                if (runNumber != 0)
                    runStartRoom.AddLockedDoor(runRoot, runNumber);
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
                        (ChallengeType.None, 1),
                        (ChallengeType.Combat, 3),
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
