using Godot;

using RandomDungeons.Nodes.Components;

namespace RandomDungeons.Nodes.Elements.Enemies
{
    public class ObliviousZombie : KinematicBody2D
    {
        [Export] public int Health = 5;

        [Export] public float MinIdleTime = 1f;
        [Export] public float MaxIdleTime = 2f;
        [Export] public float WanderTime = 3;
        [Export] public float WanderSpeed = 32;

        private State _currentState;
        private float _hurtboxCooldownTimer;

        public override void _Ready()
        {
            ChangeState(Idle);
        }

        private void ChangeState(State state)
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

            if (Health <= 0)
            {
                // TODO: Play a death animation
                QueueFree();
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

        private abstract class State
        {
            public ObliviousZombie Owner;
            public virtual void _Process(float delta) {}
            public virtual void _PhysicsProcess(float delta) {}
            public virtual void _StateEntered() {}
            public virtual void _StateExited() {}

            protected void ChangeState(State state)
            {
                Owner.ChangeState(state);
            }
        }

        private readonly IdleState Idle = new IdleState();
        private class IdleState : State
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
        private class WanderState : State
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

        private readonly KnockedBackState KnockedBack = new KnockedBackState();
        private class KnockedBackState : State
        {
            private const float Friction = 500;
            private const float MinSpeedForCollisionDamage = 90;

            public Vector2 Velocity;

            public override void _PhysicsProcess(float delta)
            {
                // Move
                var prevVel = Velocity;
                Velocity = Owner.MoveAndSlide(Velocity);
                Velocity = Velocity.MoveToward(Vector2.Zero, Friction * delta);

                // Take damage upon hitting a wall too hard
                bool fastEnough = prevVel.Length() > MinSpeedForCollisionDamage;
                if (Owner.GetSlideCount() > 0 && fastEnough)
                {
                    Owner.Health--;
                }

                // Resume normal behavior after coming to a stop
                if (Velocity == Vector2.Zero)
                    ChangeState(Owner.Idle);
            }
        }
    }
}
