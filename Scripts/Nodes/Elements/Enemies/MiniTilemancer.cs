using Godot;
using RandomDungeons.Nodes.Components;
using RandomDungeons.Nodes.Elements.Projectiles;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.Elements.Enemies
{
    public class MiniTilemancer : KinematicBody2D
    {
        [Export] public int Health = 2;
        [Export] public float TileSpawnRadius = 32 * 2;
        [Export] public float WanderSpeed = 32;
        [Export] public float MinWanderDuration = 0.25f;
        [Export] public float MaxWanderDuration = 1.25f;
        [Export] public float TileThrowSpeed = 32 * 19;
        [Export] public PackedScene TilePrefab;

        private Node2D _target => GetTree().FindPlayer();
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private HurtFlasher _hurtFlasher => GetNode<HurtFlasher>("%HurtFlasher");

        private TilemancerTile _currentTile = null;

        private Vector2 _walkVelocity;
        private Vector2 _knockbackVelocity;
        private const float KnockbackFriction = 500;
        private const float MinSpeedForCollisionDamage = 90;

        private bool _isDead = false;

        public override void _PhysicsProcess(float delta)
        {
            // Move
            Vector2 prevKnockbackVel = _knockbackVelocity;
            Vector2 totalVelocity = _walkVelocity + _knockbackVelocity;
            totalVelocity = MoveAndSlide(totalVelocity);
            _knockbackVelocity = totalVelocity - _walkVelocity;
            _knockbackVelocity = _knockbackVelocity.MoveToward(
                Vector2.Zero,
                KnockbackFriction * delta
            );

            // Take damage upon hitting a wall too hard
            bool hitWall = GetSlideCount() > 0;
            bool fastEnough = prevKnockbackVel.Length() > MinSpeedForCollisionDamage;
            if (hitWall && fastEnough)
            {
                Health--;
                _hurtFlasher.Flash();
            }

            if (Health <= 0 && !_isDead)
                Die();
        }

        public void OnTookDamage(HitBox hitBox)
        {
            Health -= hitBox.Damage;
            _knockbackVelocity = hitBox.GetKnockbackVelocity(this);
            _hurtFlasher.Flash();
        }

        public void StartWandering()
        {
            // Choose a random direction to walk in
            float angle = Mathf.Deg2Rad(GD.Randf() * 360);
            _walkVelocity = WanderSpeed * new Vector2(
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
            _walkVelocity = Vector2.Zero;
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
            GetParent().AddChild(_currentTile);
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

        private void Die()
        {
            StopWandering();
            _knockbackVelocity = Vector2.Zero;

            if (IsInstanceValid(_currentTile))
                _currentTile.Shatter();

            _isDead = true;
            _hurtFlasher.Cancel();
            _animator.CurrentAnimation = "Death";
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
