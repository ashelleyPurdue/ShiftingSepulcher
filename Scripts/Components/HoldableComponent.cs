using System;
using Godot;

namespace RandomDungeons
{
    [CustomNode]
    public class HoldableComponent : BaseComponent<KinematicBody2D>
    {
        [Signal] public delegate void PickedUp();
        [Signal] public delegate void Released();
        [Signal] public delegate void HitWall();
        [Signal] public delegate void HitFloor();
        [Signal] public delegate void Thrown(Vector2 direction);

        [Export] public bool RotatesWhileHeld {get; set;} = false;


        [Export] public float ThrowDistance = 32 * 5;
        [Export] public float ThrowSpeed = 32 * 20;
        [Export] public float ArcHeight = 32;
        [Export] public NodePath Visuals;
        [Export] public NodePath Shadow;

        public bool IsBeingHeld {get; private set;} = false;
        public bool IsFlying {get; private set;}


        private Node2D _visuals => GetNode<Node2D>(Visuals);
        private Node2D _shadow => GetNode<Node2D>(Shadow);
        private CollisionShape2D _solidCollider => Entity.SingleChildOfType<CollisionShape2D>();

        private Vector2 _hVelocity;
        private float _vSpeed;
        private float _gravity;
        private float _initialVisualsY;
        private float _throwTimer;


        public override void _EntityReady()
        {
            if (this.HasComponent<InteractableComponent>(out var i))
            {
                i.Connect(
                    signal: nameof(InteractableComponent.Interacted),
                    target: this,
                    method: nameof(OnInteracted)
                );

                i.PromptText = "Pick up";
            }
        }

        public override void _Process(float delta)
        {
            if (_shadow != null)
                _shadow.Visible = !IsBeingHeld;
        }

        public override void _PhysicsProcess(float delta)
        {
            // Don't stop when hitting the player, since the player is the
            // one who threw it.
            Entity.CollisionLayer = IsFlying
                ? (uint)CollisionLayerBits.StopsEnemiesOnly
                : (uint)CollisionLayerBits.Walls;

            // Become intangible while being carried by the player
            _solidCollider.Disabled = IsBeingHeld;

            if (!IsFlying)
                return;

            // Animate the visuals going up and down
            var pos = _visuals.Position;
            pos.y -= _vSpeed * delta;
            _visuals.Position = pos;

            _vSpeed -= _gravity * delta;

            // Stop when hitting a wall
            var collision = Entity.MoveAndCollide(_hVelocity * delta);
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

        public void OnInteracted()
        {
            GetTree().FindPlayer().PickUp(this);
        }

        public void PickUp()
        {
            if (IsBeingHeld)
                throw new InvalidOperationException($"{Entity.Name} is already being held");

            IsBeingHeld = true;
            EmitSignal(nameof(PickedUp));
        }

        public void Release()
        {
            if (!IsBeingHeld)
                throw new InvalidOperationException($"{Entity.Name} is not being held");

            IsBeingHeld = false;
            EmitSignal(nameof(Released));
        }

        public void Throw(Vector2 direction)
        {
            Release();
            EmitSignal(nameof(Thrown), direction);
            StartFlying(direction);
        }

        private void StartFlying(Vector2 direction)
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
