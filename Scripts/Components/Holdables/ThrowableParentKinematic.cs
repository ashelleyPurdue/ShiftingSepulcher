using Godot;

namespace RandomDungeons
{
    public class ThrowableParentKinematic : Node
    {
        [Signal] public delegate void HitWall();
        [Signal] public delegate void HitFloor();

        [Export] public float ThrowDistance = 32 * 5;
        [Export] public float ThrowSpeed = 32 * 20;
        [Export] public float ArcHeight = 32;
        [Export] public NodePath Visuals;

        public bool IsFlying {get; private set;}

        private KinematicBody2D _parent => GetParent<KinematicBody2D>();
        private Node2D _visuals => GetNode<Node2D>(Visuals);

        private Vector2 _hVelocity;
        private float _vSpeed;
        private float _gravity;
        private float _initialVisualsY;
        private float _throwTimer;

        public override void _PhysicsProcess(float delta)
        {
            // Don't stop when hitting the player, since the player is the
            // one who threw it.
            _parent.CollisionLayer = IsFlying
                ? (uint)CollisionLayerBits.StopsEnemiesOnly
                : (uint)CollisionLayerBits.Walls;

            if (!IsFlying)
                return;

            // Animate the visuals going up and down
            var pos = _visuals.Position;
            pos.y -= _vSpeed * delta;
            _visuals.Position = pos;

            _vSpeed -= _gravity * delta;

            // Stop when hitting a wall
            var collision = _parent.MoveAndCollide(_hVelocity * delta);
            if (collision != null)
            {
                StopFlying();
                EmitSignal(nameof(HitWall));
                return;
            }

            // Stop when hitting the ground
            _throwTimer -= delta;
            if (_throwTimer <= 0)
            {
                StopFlying();
                EmitSignal(nameof(HitFloor));
                return;
            }
        }

        public void OnThrown(Vector2 direction)
        {
            IsFlying = true;
            _hVelocity = direction * ThrowSpeed;
            _throwTimer = ThrowDistance / ThrowSpeed;
            _initialVisualsY = _visuals.Position.y;

            (_vSpeed, _gravity) = AccelMath.SpeedAndFrictionNeededForDistanceAndTime(
                distance: ArcHeight / 2,
                time: _throwTimer / 2
            );
        }

        private void StopFlying()
        {
            IsFlying = false;
            _hVelocity = Vector2.Zero;
            _vSpeed = 0;
            _throwTimer = 0;

            // Reset the throwing animation
            var pos = _visuals.Position;
            pos.y = _initialVisualsY;
            _visuals.Position = pos;
        }
    }
}
