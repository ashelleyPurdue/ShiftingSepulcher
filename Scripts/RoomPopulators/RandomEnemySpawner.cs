using System;
using Godot;

namespace ShiftingSepulcher
{
    public class RandomEnemySpawner : Node2D, IRoomPopulator
    {
        [Export] public SceneSpawnTable SpawnTable;
        [Export] public int MinCount = 1;
        [Export] public int MaxCount = 1;
        [Export] public float SpawnRadius = 0;

        public void Populate(DungeonTreeRoom treeRoom, Random rng)
        {
            int count = rng.Next(MinCount, MaxCount);

            for (int i = 0; i < count; i++)
            {
                var enemy = SpawnTable.Spawn<Node2D>(rng);
                enemy.Position = RandomPos(rng);
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
