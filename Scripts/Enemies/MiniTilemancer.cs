using Godot;

namespace ShiftingSepulcher
{
    public class MiniTilemancer : KinematicBody2D
    {
        [Export] public float TileSpawnRadius = 32 * 2;
        [Export] public float WanderSpeed = 32;
        [Export] public float MinWanderDuration = 0.25f;
        [Export] public float MaxWanderDuration = 1.25f;
        [Export] public float TileThrowSpeed = 32 * 19;
        [Export] public PackedScene TilePrefab;

        private Node2D _target;

        private KnockbackableVelocityComponent _body => this.GetComponent<KnockbackableVelocityComponent>();
        private EnemyComponent _enemy => this.GetComponent<EnemyComponent>();

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        private TilemancerTile _currentTile = null;

        public override void _EnterTree()
        {
            _target = GetTree().FindPlayer();
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

            // Choose a random duration to wander for
            // The wandering part of the animation is exactly 1 second, so we
            // just need to set the animation speed to 1 / (duration) while
            // wandering.
            float wanderDuration = (float)GD.RandRange(
                MinWanderDuration,
                MaxWanderDuration
            );
            _animator.PlaybackSpeed = 1f / wanderDuration;
        }

        public void StopWandering()
        {
            _body.WalkVelocity = Vector2.Zero;
            _animator.PlaybackSpeed = 1;
        }

        public void SummonTile()
        {
            // Failsafe: immediately throw the existing tile, if it's already there
            if (IsInstanceValid(_currentTile))
            {
                GD.Print("Already have a summoned tile.  Throwing it now.");
                ThrowTile();
            }

            _currentTile = TilePrefab.Instance<TilemancerTile>();
            _enemy.Connect(
                signal: nameof(EnemyComponent.Dead),
                target: _currentTile,
                method: nameof(_currentTile.Shatter),
                flags: (int)(ConnectFlags.ReferenceCounted | ConnectFlags.Oneshot)
            );
            _enemy.Connect(
                signal: nameof(EnemyComponent.Respawning),
                target: _currentTile,
                method: "queue_free",
                flags: (int)(ConnectFlags.ReferenceCounted | ConnectFlags.Oneshot)
            );


            this.GetRoom().AddChild(_currentTile);
            _currentTile.GlobalPosition = RandomTileSpawnPos();
        }

        public void ThrowTile()
        {
            // Don't do anything if the tile has already been destroyed
            if (!IsInstanceValid(_currentTile))
            {
                _currentTile = null;
                return;
            }

            // Throw it!
            _currentTile.Throw(TileThrowSpeed);
            _currentTile = null;
        }

        public void OnDying()
        {
            StopWandering();
            _currentTile = null;

            _animator.Play("Death");
            _animator.PlaybackSpeed = 1;
        }

        private Vector2 RandomTileSpawnPos()
        {
            float angleDeg = Mathf.Rad2Deg(GetAngleTo(_target.GlobalPosition));
            angleDeg += (GD.Randf() * 180) - 90;

            float angle = Mathf.Deg2Rad(angleDeg);
            var dir = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            );

            return GlobalPosition + (dir * TileSpawnRadius);
        }
    }
}
