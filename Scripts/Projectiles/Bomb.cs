using Godot;

namespace ShiftingSepulcher
{
    public class Bomb : KinematicBody2D
    {
        [Export] public PackedScene ExplosionPrefab;
        [Export] public float FuseDuration = 5;

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        public void LightFuse()
        {
            _animator.Play("FuseRunning");
            _animator.PlaybackSpeed = _animator.CurrentAnimationLength / FuseDuration;
        }

        public void OnHitInstantDetonateTrigger()
        {
            var throwable = this.GetComponent<HoldableComponent>();

            if (throwable.IsFlying)
                Detonate();
        }

        public void Detonate()
        {
            var explosion = ExplosionPrefab.Instance<Node2D>();
            this.GetRoom().AddChild(explosion);
            explosion.Position = Position;

            QueueFree();
        }
    }
}
