using System;
using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    [CustomNode]
    public class SpawnTable : Node
    {
        public T Spawn<T>(Random rng) where T : class
        {
            var weights = this
                .EnumerateChildren()
                .Cast<SpawnTableEntry>()
                .ToDictionary(c => c, c => c.Weight);

            var choice = rng.PickFromWeighted(weights);

            return choice.Scene.Instance<T>();
        }
    }
}
