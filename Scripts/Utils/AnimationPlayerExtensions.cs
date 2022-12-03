using Godot;

namespace RandomDungeons
{
    public static class AnimationPlayerExtensions
    {
        public static void Reset(this AnimationPlayer anim)
        {
            anim.Play("RESET");
            anim.Advance(0);
        }

        public static void PlayAndAdvance(this AnimationPlayer anim, string animationName)
        {
            anim.Play(animationName);
            anim.Advance(0);
        }

        public static void ResetAndPlay(this AnimationPlayer anim, string animationName)
        {
            anim.Reset();
            anim.PlayAndAdvance(animationName);
        }
    }
}
