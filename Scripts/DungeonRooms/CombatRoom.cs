using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class CombatRoom : SimpleDungeonRoom
    {
        private bool _isSolved = false;

        private IEnumerable<IEnemy> _enemies;

        public override void Populate(DungeonGraphRoom graphRoom)
        {
            base.Populate(graphRoom);
            _enemies = this.AllDescendantsOfType<IEnemy>();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_isSolved)
                return;

            _isSolved = _enemies.All(e => e.IsDead);
        }

        public override bool IsChallengeSolved()
        {
            return _isSolved;
        }
    }
}
