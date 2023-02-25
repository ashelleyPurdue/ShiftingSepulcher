using Godot;
namespace ShiftingSepulcher
{
    public class ChestModel : Node2D
    {
        [Export] public float OpenPercent = 0;

        public override void _Process(float delta)
        {
            var animator = GetNode<AnimationPlayer>("%OpenAnim");

            float pos = animator.CurrentAnimationPosition;
            if (pos > OpenPercent)
            {
                animator.Stop(true);
                animator.Advance(OpenPercent);
            }
            else
            {
                float diff = OpenPercent - pos;
                animator.Advance(diff);
            }
        }
    }
}
