using System;
using System.Collections.Generic;
using Godot;

namespace RandomDungeons
{
    public class SceneSpawnTable : Resource
    {
        [Export] public string BasePath = "res://";
        [Export] public Dictionary<string, int> Weights = new Dictionary<string, int>();

        public T Spawn<T>(Random rng) where T : Node
        {
            string path = $"{BasePath}/{rng.PickFromWeighted(Weights)}.tscn";
            var prefab = GD.Load<PackedScene>(path);

            return prefab.Instance<T>();
        }
    }
}
