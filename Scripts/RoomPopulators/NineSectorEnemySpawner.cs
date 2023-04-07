using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    public class NineSectorEnemySpawner : Node2D, IRoomPopulator
    {
        [Export] public int MinEnemies = 1;
        [Export] public int MaxEnemies = 9;
        [Export] NodePath SpawnTable;
        [Export] NodePath SpawnPoints;

        private SpawnTable _spawnTable => GetNode<SpawnTable>(SpawnTable);

        private enum Sector
        {
            NorthWest,
            North,
            NorthEast,
            West,
            Center,
            East,
            SouthWest,
            South,
            SouthEast
        }

        public void Populate(DungeonTreeRoom graphRoom, Random rng)
        {
            // HACK: Get the layout room by traversing the tree.
            // We need the layout room so we can tell if a sector has a door
            // in it or not (thus making that sector ineligible as a spawn point).
            // TODO: Make it so the layout room is what's passed in the first place
            var layoutRoom = this.FindAncestor<IDungeonRoom>().LayoutRoom;
            var room2D = this.GetRoom();

            Node2D[] shuffledSpawnPoints = rng.Shuffle(AllEligibleSectors(layoutRoom))
                .Select(SpawnPointAtSector)
                .ToArray();

            int enemyCount = rng.Next(MinEnemies, MaxEnemies + 1);
            if (enemyCount > shuffledSpawnPoints.Length)
                enemyCount = shuffledSpawnPoints.Length;

            for (int i = 0; i < enemyCount; i++)
            {
                var spawnPoint = shuffledSpawnPoints[i];
                var enemy = _spawnTable.Spawn<Node2D>(rng);
                enemy.Position = spawnPoint.GetPosRelativeToAncestor(room2D);

                room2D.AddChild(enemy);
            }
        }

        private Node2D SpawnPointAtSector(Sector s)
        {
            return GetNode<Node2D>($"{SpawnPoints}/{s}");
        }

        private IEnumerable<Sector> AllEligibleSectors(DungeonLayoutRoom layoutRoom)
        {
            for (int i = 0; i < 9; i++)
            {
                var sector = (Sector)i;

                if (IsSectorEligible(sector, layoutRoom))
                    yield return sector;
            }
        }

        private bool IsSectorEligible(Sector s, DungeonLayoutRoom layoutRoom)
        {
            switch (s)
            {
                case Sector.North: return !layoutRoom.HasDoorAtDirection(CardinalDirection.North);
                case Sector.South: return !layoutRoom.HasDoorAtDirection(CardinalDirection.South);
                case Sector.East: return !layoutRoom.HasDoorAtDirection(CardinalDirection.East);
                case Sector.West: return !layoutRoom.HasDoorAtDirection(CardinalDirection.West);
                default: return true;
            }
        }
    }
}
