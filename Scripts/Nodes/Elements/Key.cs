using System;
using System.Collections.Generic;
using Godot;

namespace RandomDungeons.Prefabs.Elements
{
    public class Key : Node2D
    {
        private static readonly IReadOnlyList<Color> _keyIdColors = new[]
        {
            new Color(0, 0, 0),
            new Color(0.84f, 0.84f, 0.84f),
            new Color(1, 0, 0),
            new Color(0, 1, 0),
            new Color(0, 0, 1),
            new Color(1, 1, 0),
            new Color(0, 1, 1),
            new Color(1, 0.5f, 0)
        };
        public static Color ColorForId(int keyId)
        {
            if (keyId < _keyIdColors.Count)
            {
                return _keyIdColors[keyId];
            }

            // There won't be any collisions.  Trust me.
            var rng = new Random(keyId);
            return new Color(rng.Next(256), rng.Next(256), rng.Next(256));
        }

        [Export] public int KeyId;

        public override void _Ready()
        {
            // Color the key according to its id
            Modulate = ColorForId(KeyId);
        }

        private void BodyEntered(object body)
        {
            if (body is Player)
            {
                Global.CollectKey(KeyId);
                QueueFree();
            }
        }
    }
}



