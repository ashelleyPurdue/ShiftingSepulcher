using Godot;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.Elements.Enemies
{
    public class ObliviousZombie : Node
    {
        [Export] public float MinIdleTime = 1f;
        [Export] public float MaxIdleTime = 2f;
        [Export] public float WanderSpeed = 32 * 3;

        private EnemyBody _body => this.FindAncestor<EnemyBody>();
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        public void OnDead()
        {
            _body.WalkVelocity = Vector2.Zero;

            _animator.PlaybackSpeed = 1;
            _animator.Play("Death");
        }

        public void OnRespawning()
        {
            _animator.Play("RESET");
            _animator.Advance(0);
            _animator.Play("Cycle");
        }

        public void StartWandering()
        {
            // Choose a random direction to walk in
            float angle = Mathf.Deg2Rad(GD.Randf() * 360);
            _body.WalkVelocity = WanderSpeed * new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            );

            _animator.PlaybackSpeed = 1;
        }

        public void StartIdling()
        {
            _body.WalkVelocity = Vector2.Zero;

            float idleDuration = (float)GD.RandRange(
                MinIdleTime,
                MaxIdleTime
            );
            _animator.PlaybackSpeed = 1f / idleDuration;
        }
    }
}
