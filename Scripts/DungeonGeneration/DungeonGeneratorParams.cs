namespace ShiftingSepulcher
{
    public class DungeonGeneratorParams
    {
        public int CriticalPathRuns = 6;

        public int MinCriticalPathRunLength = 3;
        public int MaxCriticalPathRunLength = 5;

        public int OptionalRuns = 6;
        public int MinOptionalRunLength = 1;
        public int MaxOptionalRunLength = 1;

        public int FillerRoomWeight = 1;
        public int CombatRoomWeight = 3;
        public int PuzzleRoomWeight = 2;
    }
}
