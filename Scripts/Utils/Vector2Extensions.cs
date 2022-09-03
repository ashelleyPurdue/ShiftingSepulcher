using Godot;

namespace RandomDungeons.Utils
{
    public static class Vector2Extensions
    {
        public static EightDirection ToNearestEightDirection(this Vector2 v)
        {
            v = v.Normalized();
            float angleDeg = Mathf.Rad2Deg(v.Angle());

            int dir = (Mathf.RoundToInt(angleDeg / 45));
            dir += 2;
            dir = Mathf.Wrap(dir, 0, 8);

            return (EightDirection)dir;
        }
    }
}
