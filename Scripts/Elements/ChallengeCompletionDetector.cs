using System;
using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    public class ChallengeCompletionDetector : Node, IOnRoomReady
    {
        /// <summary>
        /// Emitted once, when there are no more unsolved challenges in the room.
        /// </summary>
        [Signal] public delegate void ChallengeSolved();

        /// <summary>
        /// Emitted once when all challenges in the room are solved, if there
        /// is at least one challenge in the room.
        /// </summary>
        [Signal] public delegate void ChallengeSolvedChimeShouldPlay();

        private IChallenge[] _challenges;
        private bool _sentChallengeSolvedSignal = false;

        public void OnRoomReady()
        {
            // Gather up all IChallenge nodes, so we don't need to do a full
            // traversal every frame
            _challenges = this.GetRoom().AllDescendantsOfType<IChallenge>().ToArray();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (!_sentChallengeSolvedSignal && IsChallengeSolved())
            {
                _sentChallengeSolvedSignal = true;
                EmitSignal(nameof(ChallengeSolved));

                if (_challenges.Any())
                    EmitSignal(nameof(ChallengeSolvedChimeShouldPlay));
            }
        }

        private bool IsChallengeSolved()
        {
            if (_challenges.Length == 0)
                return true;

            return _challenges.All(c => c.IsSolved());
        }
    }
}
