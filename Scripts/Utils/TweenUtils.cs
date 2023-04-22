using Godot;

namespace ShiftingSepulcher
{
    public static class TweenUtils
    {
        public static float Sinusoidal(float start, float end, float t)
        {
            float stretchedCosine = (-0.5f * Mathf.Cos(Mathf.Pi * t)) + 0.5f;
            return Mathf.Lerp(start, end, stretchedCosine);
        }
    }
}
