using System;
using Godot;

namespace ShiftingSepulcher
{
    public static class Vector3iExtensions
    {
        /// <summary>
        /// Converts to a Vector2i, discarding the z coordinate
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2i FlattenToVector2i(this Vector3i v)
        {
            return new Vector2i(v.x, v.y);
        }
    }
}
