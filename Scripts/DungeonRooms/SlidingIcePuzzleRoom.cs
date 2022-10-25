using Godot;

namespace RandomDungeons
{
    public class SlidingIcePuzzleRoom : SimpleDungeonRoom
    {
        private Node2D _contentSpawn => GetNode<Node2D>("%ContentSpawn");
        private SlidingIcePuzzle _puzzle => GetNode<SlidingIcePuzzle>("%SlidingIcePuzzle");

        public override bool IsChallengeSolved()
        {
            return _puzzle.IsSolved();
        }
    }
}
