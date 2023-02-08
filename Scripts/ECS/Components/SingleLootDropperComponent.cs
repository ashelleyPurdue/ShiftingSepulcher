using System;
using Godot;

namespace RandomDungeons
{
    [CustomNode]
    public class SingleLootDropperComponent : BaseComponent<Node2D>, ILootDropperComponent
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
            this.GetRoom().AddChild(lootItem);
            lootItem.GlobalPosition = Entity.GlobalPosition;
        }
    }
}
