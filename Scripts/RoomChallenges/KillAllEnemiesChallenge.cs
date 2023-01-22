using System;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class KillAllEnemiesChallenge : Node, IChallenge
    {
        private IEnemy[] _enemies;

        public override void _EnterTree()
        {
            _enemies = this.GetRoom()
                .AllDescendantsOfType<IEnemy>()
                .ToArray();
        }

        public bool IsSolved()
        {
            return _enemies.All(e => e.IsDead);
        }
    }
}
