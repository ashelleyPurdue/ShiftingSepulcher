using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public static class RandomExtensions
    {
        public static T[] Shuffle<T>(this Random rng, IEnumerable<T> items)
        {
            var unshuffled = new List<T>(items);
            var shuffled = new List<T>();

            while (unshuffled.Count > 0)
            {
                int i = rng.Next(unshuffled.Count);
                shuffled.Add(unshuffled[i]);
                unshuffled.RemoveAt(i);
            }

            return shuffled.ToArray();
        }

        public static T PickFrom<T>(this Random rng, IEnumerable<T> options)
        {
            T[] optionsArray = options.ToArray();

            if (optionsArray.Length <= 0)
                throw new Exception("Options array is empty");

            int i = rng.Next(0, optionsArray.Length);
            return optionsArray[i];
        }

        public static T PickFromWeighted<T>(
            this Random rng,
            params (T value, int weight)[] weights
        )
        {
            int maxRoll = weights
                .Select(w => w.weight)
                .Sum();

            int roll = rng.Next(maxRoll);

            int sum = 0;
            foreach (var w in weights)
            {
                sum += w.weight;
                if (roll < sum)
                    return w.value;
            }

            throw new Exception("Uhh...I didn't think this through, apparently");
        }

        public static T PickFromWeighted<T>(this Random rng, IDictionary<T, int> weights)
        {
            var convertedWeights = weights
                .Select(kvp => (kvp.Key, kvp.Value))
                .ToArray();

            return rng.PickFromWeighted<T>(convertedWeights);
        }

        public static Vector2 PointInUnitCircle(this Random rng)
        {
            float angle = (float)rng.NextDouble() * Mathf.Deg2Rad(360);
            float length = (float)rng.NextDouble();

            return new Vector2(
                length * Mathf.Cos(angle),
                length * Mathf.Sin(angle)
            );
        }
    }
}
