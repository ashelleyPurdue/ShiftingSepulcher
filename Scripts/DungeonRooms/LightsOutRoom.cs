using Godot;

namespace RandomDungeons
{
    public class LightsOutRoom : SimpleDungeonRoom
    {
        private LightsOutPuzzle _puzzle => GetNode<LightsOutPuzzle>("%LightsOutPuzzle");

        public override bool IsChallengeSolved()
        {
            return _puzzle.IsSolved();
        }
    }
}
