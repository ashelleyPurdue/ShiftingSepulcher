using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomDungeons.Utils
{
    public static class RandomExtensions
    {
        public static T PickFrom<T>(this Random rng, IEnumerable<T> options)
        {
            T[] optionsArray = options.ToArray();
            int i = rng.Next(0, optionsArray.Length);
            return optionsArray[i];
        }
    }
}
