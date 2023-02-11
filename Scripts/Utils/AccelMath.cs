using Godot;

namespace ShiftingSepulcher
{
    public static class AccelMath
    {
        /// <summary>
        /// Returns the speed that would be required to send an object a given
        /// distance with a given amount of friction, using a timestep of 1/60.
        ///
        /// The answer is only an estimate, because some combinations of
        /// distances/frictions are impossible to hit exactly with a fixed
        /// timestep.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="friction"></param>
        /// <returns></returns>
        public static float SpeedNeededForDistance(float distance, float friction)
        {
            float tolerance = 0.1f;
            float speed = 1;
            float predictedDist = CalculateDistance(speed);

            while(Mathf.Abs(predictedDist - distance) > tolerance)
            {
                float mult = predictedDist > distance
                    ? 0.75f
                    : 2f;

                speed *= mult;
                predictedDist = CalculateDistance(speed);
            }

            return speed;

            float CalculateDistance(float s)
            {
                float delta = 1f / 60;
                float v = s;
                float d = 0;

                while (v > 0)
                {
                    d += v * delta;
                    v = Mathf.MoveToward(v, 0, friction * delta);
                }

                return d;
            }
        }

        public static (float speed, float friction) SpeedAndFrictionNeededForDistanceAndTime(
            float distance,
            float time
        )
        {
            float friction = (2 * distance) / (time * time);
            float speed = SpeedNeededForDistance(distance, friction);

            return (speed, friction);
        }
    }
}
