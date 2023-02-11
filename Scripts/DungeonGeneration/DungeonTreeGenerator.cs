using System;
using System.Linq;

namespace ShiftingSepulcher
{
    public static class DungeonTreeGenerator
    {
        public static DungeonTreeRoom GenerateUsingRuns(
            int seed,
            DungeonGeneratorParams genParams = null
        )
        {
            if (genParams == null)
                genParams = new DungeonGeneratorParams();

            var rng = new Random(seed);

            DungeonTreeRoom root = new DungeonTreeRoom();
            root.ChallengeType = ChallengeType.None;

            GenerateCriticalPath();
            GenerateOptionalPaths();

            return root;

            void GenerateCriticalPath()
            {
                DungeonTreeRoom prevRunRoot = root;
                for (int runNumber = 0; runNumber < genParams.CriticalPathRuns; runNumber++)
                {
                    // Generate a run
                    int runLength = rng.Next(genParams.MinRunLength, genParams.MaxRunLength);
                    var runRoot = GenerateRun(runLength);

                    // Hide a key in the last room of the run
                    var runEndRoom = runRoot
                        .AllDescendants()
                        .Last();

                    runEndRoom.KeyId = runNumber + 1;
                    runEndRoom.ChallengeType = ChallengeType.Key;

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
            }

            void GenerateOptionalPaths()
            {
                DungeonTreeRoom[] criticalPathRooms = root
                    .AllDescendants()
                    .ToArray();

                DungeonTreeRoom prevRunRoot = root;
                for (int runNumber = 0; runNumber < genParams.OptionalRuns; runNumber++)
                {
                    // Generate a run
                    int runLength = rng.Next(genParams.MinRunLength, genParams.MaxRunLength);
                    var runRoot = GenerateRun(runLength);

                    // TODO: Hide treasure in the final room of the run.

                    // Pick a random room to start this run in.
                    // Only choose rooms from the critical path, to prevent
                    // long chains of optional rooms that don't lead to a key
                    var availableRooms = criticalPathRooms
                        .Where(r => r.ChildDoors.Count < 3);
                    var runStartRoom = rng.PickFrom(availableRooms);

                    runStartRoom.AddChallengeDoor(runRoot);
                    prevRunRoot = runRoot;
                }
            }

            DungeonTreeRoom GenerateRun(int runLength)
            {
                DungeonTreeRoom runRoot = null;
                DungeonTreeRoom prevRoom = null;

                for (int i = 0; i < runLength; i++)
                {
                    var room = new DungeonTreeRoom();
                    room.RoomSeed = rng.Next();
                    room.ChallengeType = rng.PickFromWeighted(
                        (ChallengeType.None, genParams.FillerRoomWeight),
                        (ChallengeType.Combat, genParams.CombatRoomWeight),
                        (ChallengeType.Puzzle, genParams.PuzzleRoomWeight)
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
