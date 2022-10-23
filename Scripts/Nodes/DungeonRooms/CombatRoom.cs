using System;
using System.Linq;
using Godot;

using RandomDungeons.Nodes.Elements.Enemies;
namespace RandomDungeons.Nodes.DungeonRooms
{
    public class CombatRoom : SimpleDungeonRoom
    {
        [Export] public NodePath[] RequiredEnemies = new NodePath[0];

        private bool _isSolved = false;

        public override void _PhysicsProcess(float delta)
        {
            if (_isSolved)
                return;
            
            _isSolved = RequiredEnemies
                .Select(p => GetNode<IEnemy>(p))
                .All(e => e.IsDead);
        }

        public override bool IsChallengeSolved()
        {
            return _isSolved;
        }
    }
}