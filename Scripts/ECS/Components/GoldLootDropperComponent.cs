using System;
using Godot;

namespace RandomDungeons
{
    public class GoldLootDropperComponent : BaseComponent<Node2D>, ILootDropperComponent
    {
        [Export] public PackedScene CoinPrefab;
        [Export] public int MinGold = 1;
        [Export] public int MaxGold = 21;
        [Export] public float ActivationChance = 0.5f;

        private bool _alreadyDropped = false;

        public void DropLoot()
        {
            if (_alreadyDropped)
                return;
            _alreadyDropped = true;

            var rng = new Random((int)GD.Randi());

            if (rng.NextDouble() > ActivationChance)
                return;

            int amount = rng.Next(MinGold, MaxGold + 1);

            for (int i = 0; i < amount; i++)
            {
                var coin = CoinPrefab.Instance<Node2D>();
                this.GetRoom().AddChild(coin);

                var offset = rng.PointInUnitCircle() * 16;
                coin.GlobalPosition = Entity.GlobalPosition + offset;
            }
        }
    }

}
