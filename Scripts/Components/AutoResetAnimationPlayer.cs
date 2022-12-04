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

                const float timeOffset = 0.000001f;

                var mirrorAnim = new Animation();
                mirrorAnim.Length = targetPlayer.GetAnimation(animName).Length + timeOffset;

                int trackId = mirrorAnim.AddTrack(Animation.TrackType.Animation);
                mirrorAnim.TrackSetPath(trackId, TargetAnimationPlayer);
                mirrorAnim.AnimationTrackInsertKey(trackId, 0, "RESET");
                mirrorAnim.AnimationTrackInsertKey(trackId, timeOffset, animName);

                AddAnimation(animName, mirrorAnim);
            }
        }
    }
}
