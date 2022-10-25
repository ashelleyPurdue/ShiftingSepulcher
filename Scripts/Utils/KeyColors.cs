using System;
using System.Collections.Generic;
using Godot;

namespace RandomDungeons
{
    public static class KeyColors
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
        public static Color ForId(int keyId)
        {
            if (keyId < _keyIdColors.Count)
            {
                return _keyIdColors[keyId];
            }

            // There won't be any collisions.  Trust me.
            var rng = new Random(keyId);
            return new Color(rng.Next(256), rng.Next(256), rng.Next(256));
        }
    }
}
