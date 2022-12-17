using Godot;

namespace RandomDungeons
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
            var throwable = this.SingleChildOfType<ThrowableParentKinematic>();

            if (throwable.IsFlying)
                Detonate();
        }

        public void Detonate()
        {
            var parent = GetParent<Node2D>();
            var explosion = ExplosionPrefab.Instance<Node2D>();
            parent.AddChild(explosion);
            explosion.Position = Position;

            QueueFree();
        }
    }
}
