using Godot;

using RandomDungeons.Nodes.Components;
using RandomDungeons.StateMachines;
using RandomDungeons.StateMachines.CommonStates;

namespace RandomDungeons.Nodes.Elements.Enemies
{
    public class AnimatedTile : KinematicBody2D
    {
        [Export] public float SpinUpTime = 1;
        [Export] public float HoldTime = 2;
        [Export] public float FlySpeed = 600;
        [Export] public NodePath TargetPath;

        private const float MinRotSpeed = 360;
        private const float MaxRotSpeed = 800;

        private Node2D _tile => GetNode<Node2D>("%Tile");
        private Vector2 _hoverPos => GetNode<Node2D>("%TileHoverPos").Position;
        private Vector2 _restPos => GetNode<Node2D>("%TileRestPos").Position;

        private Vector2 _targetPos => GetNode<Node2D>(TargetPath).Position;

        private float _rotSpeed = 360;

        private StateMachine _sm;

        public override void _Ready()
        {
            DeathAnimation.AnimationTarget = _tile;
            DeathAnimation.AnimationEnded += QueueFree;

            _sm = new StateMachine(this);
            _sm.ChangeState(SpinningUp);
        }

        public override void _Process(float delta)
        {
            _tile.RotationDegrees += _rotSpeed * delta;
        }

        public void OnTookDamage(HitBox h) => Shatter();
        public void OnDealtDamage(HurtBox h) => Shatter();

        private void Shatter()
        {
            _sm.ChangeState(DeathAnimation);
            _rotSpeed = 0;
        }

        private readonly IState SpinningUp = new SpinningUpState();
        private class SpinningUpState : State<AnimatedTile>
        {
            public override void _StateEntered()
            {
                Owner._tile.Position = Owner._restPos;
                Owner._rotSpeed = MinRotSpeed;
            }

            public override void _Process(float delta)
            {
                float spinUpTime = Owner.SpinUpTime;
                float riseSpeed = Owner._restPos.DistanceTo(Owner._hoverPos) / spinUpTime;
                float rotAccel = (MaxRotSpeed - MinRotSpeed) / spinUpTime;

                Owner._tile.Position = Owner._tile.Position.MoveToward(
                    Owner._hoverPos,
                    riseSpeed * delta
                );

                Owner._rotSpeed = Mathf.MoveToward(
                    Owner._rotSpeed,
                    MaxRotSpeed, rotAccel * delta
                );
            }

            public override void _PhysicsProcess(float delta)
            {
                if (Owner._rotSpeed == MaxRotSpeed)
                    ChangeState(Owner.Holding);
            }

            public override void _StateExited()
            {
                Owner._tile.Position = Owner._hoverPos;
                Owner._rotSpeed = MaxRotSpeed;
            }
        }

        private readonly IState Holding = new HoldingState();
        private class HoldingState : State<AnimatedTile>
        {
            private float _timer;

            public override void _StateEntered()
            {
                _timer = Owner.HoldTime;
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= delta)
                    ChangeState(Owner.Flying);
            }
        }

        private readonly IState Flying = new FlyingState();
        private class FlyingState : State<AnimatedTile>
        {
            private Vector2 _velocity;

            public override void _StateEntered()
            {
                Vector2 dir = (Owner._targetPos - Owner.Position).Normalized();
                _velocity = dir * Owner.FlySpeed;
            }

            public override void _PhysicsProcess(float delta)
            {
                var collision = Owner.MoveAndCollide(_velocity * delta);

                if (collision != null)
                    Owner.Shatter();
            }
        }

        private readonly DeathAnimationState DeathAnimation = new DeathAnimationState();
    }
}
