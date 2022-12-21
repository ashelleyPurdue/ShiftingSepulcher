using System;
using Godot;

namespace RandomDungeons
{
    public class SingleLootDropper : Node2D, ILootDropper
    {
        [Export] public PackedScene LootPrefab;

        /// <summary>
        /// The probability that _anything_ will drop from this thing _at all_.
        /// If this roll passes, then something from the drop table will
        /// _definitely_ spawn.  Otherwise, nothing will spawn.
        /// </summary>
        [Export] public float ActivationChance = 1;

        public void DropLoot()
        {
            var rng = new Random((int)GD.Randi());

            if (rng.NextDouble() > ActivationChance)
                return;

            var lootItem = LootPrefab.Instance<Node2D>();
            GetParent().GetParent<Node2D>().AddChild(lootItem);
            lootItem.GlobalPosition = GlobalPosition;
        }
    }
}
