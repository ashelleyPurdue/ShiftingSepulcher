namespace ShiftingSepulcher
{
    public class DungeonGeneratorParams
    {
        public int CriticalPathRuns = 6;
        public int OptionalRuns = 6;

        public int MinRunLength = 3;
        public int MaxRunLength = 5;

        public int FillerRoomWeight = 1;
        public int CombatRoomWeight = 3;
        public int PuzzleRoomWeight = 2;
    }
}
