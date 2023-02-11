using System;
using System.Collections.Generic;

namespace ShiftingSepulcher
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TValue, TKey> Invert<TKey, TValue>(
            this Dictionary<TKey, TValue> dict
        )
        {
            var reversed = new Dictionary<TValue, TKey>();
            foreach (var pair in dict)
            {
                reversed[pair.Value] = pair.Key;
            }

            return reversed;
        }

        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(
            this Dictionary<TKey, TValue> dict
        )
        {
            var clone = new Dictionary<TKey, TValue>();

            foreach (var pair in dict)
                clone[pair.Key] = pair.Value;

            return clone;
        }
    }
}
