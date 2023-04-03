using System;
using Godot;

namespace ShiftingSepulcher
{
    public class RandomEnemySpawner : Node2D, IRoomPopulator
    {
        [Export] public int MinCount = 1;
        [Export] public int MaxCount = 1;
        [Export] public float SpawnRadius = 0;

        private SpawnTable _spawnTable => GetNode<SpawnTable>("%SpawnTable");

        public void Populate(DungeonTreeRoom treeRoom, Random rng)
        {
            var room2D = this.GetRoom();
            Vector2 spawnPointRoomPos = this.GetPosRelativeToAncestor(room2D);

            int count = rng.Next(MinCount, MaxCount);

            for (int i = 0; i < count; i++)
            {
                var enemy = _spawnTable.Spawn<Node2D>(rng);
                enemy.Position = RandomPos(rng) + spawnPointRoomPos;
                AddChild(enemy);
            }
        }

        private Vector2 RandomPos(Random rng)
        {
            float radius = (float)rng.NextDouble() * SpawnRadius;
            float angle = (float)rng.NextDouble() * Mathf.Deg2Rad(360);

            return Mathf.Polar2Cartesian(radius, angle);
        }
    }
}
