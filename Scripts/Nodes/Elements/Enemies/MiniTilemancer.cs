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
        [Export] public float TileThrowSpeed = 32 * 19;
        [Export] public PackedScene TilePrefab;

        private Node2D _target => GetTree().FindPlayer();
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private TilemancerTile _currentTile = null;

        private Vector2 _walkVelocity;
        private bool _isDead = false;

        public override void _PhysicsProcess(float delta)
        {
            MoveAndSlide(_walkVelocity);

            if (Health <= 0 && !_isDead)
                Die();
        }

        public void OnTookDamage(HitBox hitBox)
        {
            Health -= hitBox.Damage;
        }

        public void StartWandering()
        {
            float angle = Mathf.Deg2Rad(GD.Randf() * 360);
            _walkVelocity = WanderSpeed * new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            );
        }

        public void StopWandering()
        {
            _walkVelocity = Vector2.Zero;
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

            if (IsInstanceValid(_currentTile))
                _currentTile.Shatter();

            _isDead = true;
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
