using Godot;

namespace RandomDungeons
{
    [Tool]
    public class AutoResetAnimationPlayer : AnimationPlayer
    {
        [Export] public NodePath TargetAnimationPlayer
        {
            get => _targetAnimationPlayer;
            set
            {
                _targetAnimationPlayer = value;
                RefreshAnimations();
            }
        }
        private NodePath _targetAnimationPlayer;

        [Export] public bool Refresh
        {
            get => false;
            set => RefreshAnimations();
        }

        public override void _EnterTree()
        {
            // Set it up to refresh when the scene is saved
            if (Engine.EditorHint)
            {
                var editorNode = GetTree().Root.GetNode("EditorNode");
                if (!editorNode.IsConnected("scene_saved", this, nameof(OnSceneSaved)))
                    editorNode.Connect("scene_saved", this, nameof(OnSceneSaved));
            }
        }

        public override void _ExitTree()
        {
            // Stop listening to the scene being saved
            if (Engine.EditorHint)
            {
                var editorNode = GetTree().Root.GetNode("EditorNode");
                if (editorNode.IsConnected("scene_saved", this, nameof(OnSceneSaved)))
                    editorNode.Disconnect("scene_saved", this, nameof(OnSceneSaved));
            }
        }

        private void OnSceneSaved(object idk)
        {
            GD.Print($"Scene saved.  Refreshing {Name}'s animations");
            RefreshAnimations();
        }

        private void RefreshAnimations()
        {
            if (!Engine.EditorHint)
                return;

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

                const float timeOffset = 0.00001f;

                var sourceAnim = targetPlayer.GetAnimation(animName);
                var mirrorAnim = new Animation();
                mirrorAnim.Length = sourceAnim.Length + timeOffset;
                mirrorAnim.Loop = sourceAnim.Loop;
                mirrorAnim.Step = sourceAnim.Step;

                int trackId = mirrorAnim.AddTrack(Animation.TrackType.Animation);
                mirrorAnim.TrackSetPath(trackId, GetParent().GetPathTo(targetPlayer));
                mirrorAnim.AnimationTrackInsertKey(trackId, 0, "RESET");
                mirrorAnim.AnimationTrackInsertKey(trackId, timeOffset, animName);

                AddAnimation(animName, mirrorAnim);
            }
        }
    }
}
