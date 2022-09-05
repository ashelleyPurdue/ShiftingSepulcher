using Godot;

using RandomDungeons.Nodes.Components;
using RandomDungeons.StateMachines;
using RandomDungeons.StateMachines.CommonStates;

namespace RandomDungeons.Nodes.Elements.Enemies
{
    public class ObliviousZombie : KinematicBody2D, IStateMachine
    {
        [Export] public int Health = 5;

        [Export] public float MinIdleTime = 1f;
        [Export] public float MaxIdleTime = 2f;
        [Export] public float WanderTime = 3;
        [Export] public float WanderSpeed = 32;

        private IState _currentState;
        private float _hurtboxCooldownTimer;

        public override void _Ready()
        {
            KnockedBack.HitWall += OnHitWall;
            KnockedBack.StoppedMoving += OnKnockbackFinished;

            DeathAnimation.AnimationEnded += OnDeathAnimationFinished;
            DeathAnimation.AnimationTarget = GetNode<Node2D>("%Visuals");

            ChangeState(Idle);
        }

        public void ChangeState(IState state)
        {
            state.Owner = this;

            var prevState = _currentState;
            _currentState = state;

            prevState?._StateExited();
            _currentState._StateEntered();
        }

        public override void _Process(float delta)
        {
            _currentState?._Process(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            _hurtboxCooldownTimer -= delta;
            _currentState?._PhysicsProcess(delta);

            if (Health <= 0 && _currentState != DeathAnimation)
            {
                ChangeState(DeathAnimation);
            }
        }

        public void OnTookDamage(HitBox hitBox)
        {
            if (_hurtboxCooldownTimer <= 0)
            {
                Health -= hitBox.Damage;
                _hurtboxCooldownTimer = hitBox.InvlunerabilityTime;
                KnockedBack.Velocity = hitBox.KnockbackVelocity;

                ChangeState(KnockedBack);
            }
        }

        public void OnHitWall()
        {
            ChangeState(Idle);
            Health--;
        }

        public void OnKnockbackFinished()
        {
            ChangeState(Idle);
        }

        public void OnDeathAnimationFinished()
        {
            QueueFree();
        }

        private readonly IdleState Idle = new IdleState();
        private class IdleState : State<ObliviousZombie>
        {
            private float _timer;

            public override void _StateEntered()
            {
                _timer = (float)GD.RandRange(
                    Owner.MinIdleTime,
                    Owner.MaxIdleTime
                );
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.Wander);
            }
        }

        private readonly WanderState Wander = new WanderState();
        private class WanderState : State<ObliviousZombie>
        {
            private float _timer;
            private Vector2 _velocity;

            public override void _StateEntered()
            {
                _timer = Owner.WanderTime;

                // Choose a random direction
                float angle = (float)GD.RandRange(0, 360);
                var dir = new Vector2(
                    Mathf.Cos(Mathf.Deg2Rad(angle)),
                    Mathf.Sin(Mathf.Deg2Rad(angle))
                );

                // Start walking in that direction, at the appropriate speed
                _velocity = Owner.WanderSpeed * dir;
            }

            public override void _PhysicsProcess(float delta)
            {
                Owner.MoveAndSlide(_velocity);

                _timer -= delta;
                if (_timer <= 0)
                    ChangeState(Owner.Idle);
            }
        }

        private readonly KnockedBackState<ObliviousZombie> KnockedBack = new KnockedBackState<ObliviousZombie>();
        private readonly DeathAnimationState DeathAnimation = new DeathAnimationState();
    }
}
