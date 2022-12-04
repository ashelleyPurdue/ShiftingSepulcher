using Godot;

namespace RandomDungeons
{
    [Tool]
    public class AutoResetAnimationPlayer : AnimationPlayer
    {
        [Export] public NodePath TargetAnimationPlayer;
        [Export] public bool Refresh
        {
            get => false;
            set => RefreshAnimations();
        }

        private void RefreshAnimations()
        {
            // Clear out existing animations
            foreach (string animName in GetAnimationList())
            {
                RemoveAnimation(animName);
            }

            // Don't do anything if a valid animation player isn't selected
            if (!(GetNode(TargetAnimationPlayer) is AnimationPlayer targetPlayer))
                return;

            // Don't do anything if there is no RESET track
            if (!targetPlayer.HasAnimation("RESET"))
                return;

            // Create mirror animation for one in the target animator, with a
            // call to its "RESET" track at the start.
            foreach (string animName in targetPlayer.GetAnimationList())
            {
                if (animName == "RESET")
                    continue;

                var mirrorAnim = new Animation();
                AddAnimation(animName, mirrorAnim);

                int trackId = mirrorAnim.AddTrack(Animation.TrackType.Animation);
                mirrorAnim.AnimationTrackInsertKey(trackId, 0, "RESET");
                mirrorAnim.AnimationTrackInsertKey(trackId, 0.000001f, animName);
            }
        }
    }
}
