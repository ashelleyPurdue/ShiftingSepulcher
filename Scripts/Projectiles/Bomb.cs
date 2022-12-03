using Godot;

namespace RandomDungeons
{
    public class Bomb : KinematicBody2D
    {
        [Export] public PackedScene ExplosionPrefab;
        [Export] public float FuseDuration = 3;

        private bool _isFuseLit = false;
        private float _fuseTimer;

        public override void _PhysicsProcess(float delta)
        {
            if (!_isFuseLit)
                return;

            _fuseTimer -= delta;

            if (_fuseTimer <= 0)
                Detonate();
        }

        public void LightFuse()
        {
            _fuseTimer = FuseDuration;
            _isFuseLit = true;
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
