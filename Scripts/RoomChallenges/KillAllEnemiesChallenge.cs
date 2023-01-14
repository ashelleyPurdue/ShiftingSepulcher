using System;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class KillAllEnemiesChallenge : Node, IChallenge
    {
        public bool IsSolved()
        {
            var enemies = this.GetRoom().AllDescendantsOfType<IEnemy>();
            return enemies.All(e => e.IsDead);
        }
    }
}
