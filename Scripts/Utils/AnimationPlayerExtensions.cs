using Godot;

namespace ShiftingSepulcher
{
    public static class AnimationPlayerExtensions
    {
        public static void Reset(this AnimationPlayer anim)
        {
            anim.Play("RESET");
            anim.Advance(0);
        }

        public static void PlayAndAdvance(
            this AnimationPlayer anim,
            string name,
            float customBlend = -1,
            float customSpeed = 1,
            bool fromEnd = false
        )
        {
            anim.Play(name, customBlend, customSpeed, fromEnd);
            anim.Advance(0);
        }

        public static void ResetAndPlay(
            this AnimationPlayer anim,
            string name,
            float customBlend = -1,
            float customSpeed = 1,
            bool fromEnd = false
        )
        {
            anim.Reset();
            anim.PlayAndAdvance(name, customBlend, customSpeed, fromEnd);
        }
    }
}
