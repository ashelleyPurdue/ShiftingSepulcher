using Godot;

namespace ShiftingSepulcher
{
    public static class AngleMath
    {
        public static float MoveToward(float fromRad, float toRad, float deltaRad)
        {
            float shortestDist = Difference(fromRad, toRad);
            float closestToRad = fromRad + shortestDist;

            return Mathf.MoveToward(fromRad, closestToRad, deltaRad);
        }

        public static float Difference(float fromRad, float toRad)
        {
            const float maxAngle = Mathf.Pi * 2;
            const float halfAngle = Mathf.Pi;

            float diff = ( toRad - fromRad + halfAngle ) % maxAngle - halfAngle;
            return diff < -halfAngle ? diff + maxAngle : diff;
        }
    }
}
