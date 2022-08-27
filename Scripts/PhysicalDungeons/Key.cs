using System;
using System.Collections.Generic;
using Godot;

namespace RandomDungeons
{
    public class Key : Node2D
    {
        private static readonly IReadOnlyList<Color> _keyIdColors = new[]
        {
            new Color(0, 0, 0),
            new Color(214, 214, 214),
            new Color(255, 0, 0),
            new Color(0, 255, 0),
            new Color(0, 0, 255),
            new Color(255, 255, 0),
            new Color(0, 255, 255),
            new Color(255, 127, 0)
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



